using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Axon.Comtrade
{
    public class ThemeManager : INotifyPropertyChanged
    {
        private static ThemeManager _instance;
        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ThemeManager();
                return _instance;
            }
        }

        private string _currentTheme = "Light";
        public string CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ApplyTheme(value);
                OnPropertyChanged();
            }
        }

        private void ApplyTheme(string theme)
        {
            var app = Application.Current;
            var themeDict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/MyApp.Resources;component/Themes/{theme}/Colors.xaml")
            };

            app.Resources.MergedDictionaries.Clear();
            app.Resources.MergedDictionaries.Add(themeDict);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


