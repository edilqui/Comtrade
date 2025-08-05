// MyApp.UI.Components/Navigation/SidebarNavigation.xaml.cs
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Axon.UI.Components.Navigation
{
    [ContentProperty("NavigationItems")]
    public partial class SidebarNavigation : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SidebarNavigation),
                new PropertyMetadata("Mi Aplicación"));

        public static readonly DependencyProperty LogoProperty =
            DependencyProperty.Register("Logo", typeof(object), typeof(SidebarNavigation),
                new PropertyMetadata(null));

        public static readonly DependencyProperty NavigationItemsProperty =
            DependencyProperty.Register("NavigationItems", typeof(ObservableCollection<NavigationItemBase>),
                typeof(SidebarNavigation), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(string), typeof(SidebarNavigation),
                new PropertyMetadata(null, OnSelectedItemChanged));

        public static readonly DependencyProperty HeaderActionsProperty =
            DependencyProperty.Register("HeaderActions", typeof(object), typeof(SidebarNavigation),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(SidebarNavigation),
                new PropertyMetadata(null));

        #endregion

        #region Properties

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public object Logo
        {
            get => GetValue(LogoProperty);
            set => SetValue(LogoProperty, value);
        }

        public ObservableCollection<NavigationItemBase> NavigationItems
        {
            get => (ObservableCollection<NavigationItemBase>)GetValue(NavigationItemsProperty);
            set => SetValue(NavigationItemsProperty, value);
        }

        public string SelectedItem
        {
            get => (string)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public object HeaderActions
        {
            get => GetValue(HeaderActionsProperty);
            set => SetValue(HeaderActionsProperty, value);
        }

        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<NavigationItemSelectedEventArgs> ItemSelected;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public SidebarNavigation()
        {
            InitializeComponent();
            NavigationItems = new ObservableCollection<NavigationItemBase>();
            DataContext = this;
        }

        #endregion

        #region Methods

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarNavigation sidebar)
            {
                sidebar.OnItemSelected(e.NewValue?.ToString());
            }
        }

        private void OnItemSelected(string itemId)
        {
            ItemSelected?.Invoke(this, new NavigationItemSelectedEventArgs(itemId));
            OnPropertyChanged(nameof(SelectedItem));
        }

        public void AddNavigationItem(NavigationItem item)
        {
            NavigationItems.Add(item);
        }

        public void AddSeparator()
        {
            NavigationItems.Add(new NavigationSeparator());
        }

        public void AddSectionTitle(string title)
        {
            NavigationItems.Add(new NavigationSectionTitle(title));
        }

        public void RemoveNavigationItem(string itemId)
        {
            for (int i = NavigationItems.Count - 1; i >= 0; i--)
            {
                if (NavigationItems[i] is NavigationItem item && item.Id == itemId)
                {
                    NavigationItems.RemoveAt(i);
                    break;
                }
            }
        }

        public void ClearNavigationItems()
        {
            NavigationItems.Clear();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #region Navigation Item Classes

    public abstract class NavigationItemBase: DependencyObject
    {
        public abstract string Type { get; }
    }

    public class NavigationItem : NavigationItemBase, INotifyPropertyChanged
    {
        public override string Type => "NavigationItem";
        public string Id { get; set; }
        public string Text { get; set; }
        public object Icon { get; set; }
        public object HomeFilledIcon { get; set; }
        public ICommand Command { get; set; }
        public string Tooltip { get; set; }
        public bool IsEnabled { get; set; } = true;
        public object Badge { get; set; }

        public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationItem item)
            {
                item.OnPropertyChanged(nameof(IsSelected));
            }
        }


        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public NavigationItem()
        {
        }

        public NavigationItem(string id, string text, object icon = null, ICommand command = null)
        {
            Id = id;
            Text = text;
            Icon = icon;
            Command = command;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NavigationSeparator : NavigationItemBase
    {
        public override string Type => "Separator";
        public NavigationSeparator()
        {
        }
    }

    public class NavigationSectionTitle : NavigationItemBase
    {
        public override string Type => "SectionTitle";
        public string Text { get; set; }
        public NavigationSectionTitle()
        {
        }

        public NavigationSectionTitle(string text)
        {
            Text = text;
        }
    }

    #endregion

    #region Event Args

    public class NavigationItemSelectedEventArgs : EventArgs
    {
        public string ItemId { get; }

        public NavigationItemSelectedEventArgs(string itemId)
        {
            ItemId = itemId;
        }
    }

    #endregion

    #region Helper Commands

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public static class ButtonExtensions
    {
        public static readonly DependencyProperty SelectedIconProperty =
            DependencyProperty.RegisterAttached(
                "SelectedIcon",
                typeof(object),
                typeof(ButtonExtensions),
                new PropertyMetadata(null));

        public static void SetSelectedIcon(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedIconProperty, value);
        }

        public static object GetSelectedIcon(DependencyObject obj)
        {
            return obj.GetValue(SelectedIconProperty);
        }
    }

    #endregion
}