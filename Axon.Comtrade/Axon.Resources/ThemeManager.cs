using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Resources
{
    // MyApp.Resources/ThemeManager.cs
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    namespace MyApp.Resources
    {
        public enum AppTheme
        {
            Light,
            Dark
        }

        public class ThemeManager : INotifyPropertyChanged
        {
            private static ThemeManager _instance;
            private AppTheme _currentTheme = AppTheme.Dark; // Default theme

            public static ThemeManager Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new ThemeManager();
                    return _instance;
                }
            }

            public AppTheme CurrentTheme
            {
                get => _currentTheme;
                set
                {
                    if (_currentTheme != value)
                    {
                        _currentTheme = value;
                        ApplyTheme(value);
                        OnPropertyChanged();
                        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(value));
                    }
                }
            }

            public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
            public event PropertyChangedEventHandler PropertyChanged;

            private ThemeManager()
            {
                // Initialize with default theme
                ApplyTheme(_currentTheme);
            }

            public void ToggleTheme()
            {
                CurrentTheme = CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            }

            private void ApplyTheme(AppTheme theme)
            {
                try
                {
                    var app = Application.Current;
                    if (app?.Resources == null) return;

                    // Clear existing theme resources
                    var resourcesToRemove = new string[]
                    {
                    "pack://application:,,,/MyApp.Resources;component/Themes/Light/Theme.xaml",
                    "pack://application:,,,/MyApp.Resources;component/Themes/Dark/Theme.xaml"
                    };

                    for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
                    {
                        var dict = app.Resources.MergedDictionaries[i];
                        if (dict.Source != null)
                        {
                            string sourceUri = dict.Source.ToString();
                            foreach (var resource in resourcesToRemove)
                            {
                                if (sourceUri.Contains(resource))
                                {
                                    app.Resources.MergedDictionaries.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }

                    // Add new theme
                    string themeUri;
                    switch (theme)
                    {
                        case AppTheme.Light:
                            themeUri = "pack://application:,,,/MyApp.Resources;component/Themes/Light/Theme.xaml";
                            break;
                        case AppTheme.Dark:
                            themeUri = "pack://application:,,,/MyApp.Resources;component/Themes/Dark/Theme.xaml";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
                    }


                    var themeDict = new ResourceDictionary
                    {
                        Source = new Uri(themeUri, UriKind.Absolute)
                    };

                    app.Resources.MergedDictionaries.Add(themeDict);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
                }
            }

            public void SetThemeFromSystemPreference()
            {
                // This method can be extended to detect system theme preference
                // For now, it defaults to Dark theme
                CurrentTheme = AppTheme.Dark;
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class ThemeChangedEventArgs : EventArgs
        {
            public AppTheme NewTheme { get; }

            public ThemeChangedEventArgs(AppTheme newTheme)
            {
                NewTheme = newTheme;
            }
        }

        // Extension methods for easy theme usage
        public static class ThemeExtensions
        {
            public static string GetThemeName(this AppTheme theme)
            {
                switch (theme)
                {
                    case AppTheme.Light:
                        return "Light";
                    case AppTheme.Dark:
                        return "Dark";
                    default:
                        return "Dark";
                }
            }

            public static bool IsDark(this AppTheme theme)
            {
                return theme == AppTheme.Dark;
            }

            public static bool IsLight(this AppTheme theme)
            {
                return theme == AppTheme.Light;
            }
        }
    }
}
