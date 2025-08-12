using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Axon.UI.Components.Datagrid
{
    public class EnhancedFillHandleAdorner : Adorner
    {
        private const int HANDLE_SIZE = 8;
        private readonly VisualCollection _visualChildren;
        private readonly Thumb _fillHandle;
        private readonly DataGrid _dataGrid;
        private Rect _selectionRect;
        private Point _dragStartPoint;
        private bool _isDragging;

        public Rect SelectionRect
        {
            get => _selectionRect;
            set
            {
                _selectionRect = value;
                UpdateHandlePosition();
                InvalidateVisual();
            }
        }

        public string CurrentText { get; set; }

        public EnhancedFillHandleAdorner(UIElement adornedElement, Rect selectionRect, string currentText)
            : base(adornedElement)
        {
            _dataGrid = adornedElement as DataGrid;
            _selectionRect = selectionRect;
            CurrentText = currentText;
            _visualChildren = new VisualCollection(this);

            // Crear el handle de llenado
            _fillHandle = new Thumb
            {
                Cursor = Cursors.Cross,
                Width = HANDLE_SIZE,
                Height = HANDLE_SIZE,
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 215)), // Azul de Office
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                Template = CreateHandleTemplate()
            };

            _fillHandle.DragStarted += OnDragStarted;
            _fillHandle.DragDelta += OnDragDelta;
            _fillHandle.DragCompleted += OnDragCompleted;

            _visualChildren.Add(_fillHandle);
            UpdateHandlePosition();
        }

        private ControlTemplate CreateHandleTemplate()
        {
            var template = new ControlTemplate(typeof(Thumb));
            var factory = new FrameworkElementFactory(typeof(Border));

            factory.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0, 120, 215)));
            factory.SetValue(Border.BorderBrushProperty, Brushes.White);
            factory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius(1));

            template.VisualTree = factory;
            return template;
        }

        private void UpdateHandlePosition()
        {
            if (_fillHandle == null) return;

            var handlePosition = new Point(
                _selectionRect.Right - HANDLE_SIZE / 2.0,
                _selectionRect.Bottom - HANDLE_SIZE / 2.0);

            _fillHandle.Arrange(new Rect(handlePosition, new Size(HANDLE_SIZE, HANDLE_SIZE)));
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            _dragStartPoint = Mouse.GetPosition(_dataGrid);
            Mouse.Capture(_fillHandle);
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!_isDragging) return;

            var currentPosition = Mouse.GetPosition(_dataGrid);
            var targetCell = GetCellFromPoint(currentPosition);

            if (targetCell != null)
            {
                var newRect = CalculateNewSelectionRect(targetCell);
                if (newRect.HasValue)
                {
                    SelectionRect = newRect.Value;
                }
            }
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);

            if (_dataGrid.SelectedCells.Count > 0)
            {
                FillSelectedRange();
            }
        }

        private DataGridCell GetCellFromPoint(Point point)
        {
            var hitTest = _dataGrid.InputHitTest(point) as DependencyObject;

            while (hitTest != null)
            {
                if (hitTest is DataGridCell cell)
                    return cell;

                hitTest = VisualTreeHelper.GetParent(hitTest);
            }

            return null;
        }

        private Rect? CalculateNewSelectionRect(DataGridCell targetCell)
        {
            try
            {
                if (_dataGrid.SelectedCells.Count == 0) return null;

                var firstCell = GetCellFromInfo(_dataGrid.SelectedCells[0]);
                if (firstCell == null || targetCell == null) return null;

                var firstTopLeft = _dataGrid.PointFromScreen(firstCell.PointToScreen(new Point(0, 0)));
                var targetBottomRight = _dataGrid.PointFromScreen(targetCell.PointToScreen(
                    new Point(targetCell.ActualWidth, targetCell.ActualHeight)));

                return new Rect(firstTopLeft, targetBottomRight);
            }
            catch
            {
                return null;
            }
        }

        private DataGridCell GetCellFromInfo(DataGridCellInfo cellInfo)
        {
            if (cellInfo.Column == null || cellInfo.Item == null) return null;

            var rowIndex = _dataGrid.Items.IndexOf(cellInfo.Item);
            var columnIndex = cellInfo.Column.DisplayIndex;

            return GetCell(rowIndex, columnIndex);
        }

        private DataGridCell GetCell(int row, int column)
        {
            if (row < 0 || row >= _dataGrid.Items.Count ||
                column < 0 || column >= _dataGrid.Columns.Count)
                return null;

            var rowContainer = (DataGridRow)_dataGrid.ItemContainerGenerator.ContainerFromIndex(row);
            if (rowContainer == null) return null;

            var presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
            if (presenter == null) return null;

            return (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        }

        private void FillSelectedRange()
        {
            try
            {
                if (_dataGrid.SelectedCells.Count < 2) return;

                var sourceCell = _dataGrid.SelectedCells[0];
                var sourceValue = GetCellValue(sourceCell);

                // Aplicar el valor a todas las celdas seleccionadas
                foreach (var cellInfo in _dataGrid.SelectedCells)
                {
                    if (cellInfo.Equals(sourceCell)) continue;

                    SetCellValue(cellInfo, sourceValue);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en FillSelectedRange: {ex.Message}");
            }
        }

        private object GetCellValue(DataGridCellInfo cellInfo)
        {
            try
            {
                if (cellInfo.Column is DataGridBoundColumn boundColumn &&
                    boundColumn.Binding is System.Windows.Data.Binding binding)
                {
                    var property = cellInfo.Item.GetType().GetProperty(binding.Path.Path);
                    return property?.GetValue(cellInfo.Item);
                }
            }
            catch
            {
                // Fallback: obtener del elemento visual
            }

            return CurrentText;
        }

        private void SetCellValue(DataGridCellInfo cellInfo, object value)
        {
            try
            {
                if (cellInfo.Column is DataGridBoundColumn boundColumn &&
                    boundColumn.Binding is System.Windows.Data.Binding binding)
                {
                    var property = cellInfo.Item.GetType().GetProperty(binding.Path.Path);
                    if (property != null && property.CanWrite)
                    {
                        // Convertir el valor al tipo correcto
                        var convertedValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(cellInfo.Item, convertedValue);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al establecer valor de celda: {ex.Message}");
            }
        }

        protected override int VisualChildrenCount => _visualChildren.Count;

        protected override Visual GetVisualChild(int index) => _visualChildren[index];

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Dibujar el borde de selección
            var selectionBrush = new SolidColorBrush(Color.FromArgb(100, 0, 120, 215));
            var borderPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 120, 215)), 2);

            drawingContext.DrawRectangle(selectionBrush, borderPen, _selectionRect);
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < numVisuals; i++)
            {
                var visual = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                child = visual as T ?? GetVisualChild<T>(visual);
                if (child != null) break;
            }

            return child;
        }
    }
}