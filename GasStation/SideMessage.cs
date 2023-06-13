using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace GasStation
{
    public static class SideMessage
    {
        public class Behaviors
        {
            public RoutedEventHandler Yes { get; set; }
            public RoutedEventHandler No { get; set; }
            public RoutedEventHandler Cancel { get; set; }
            public RoutedEventHandler OK { get; set; }
        }
        public enum Type
        {
            Info, Warning, Error
        }

        private static Flyout _sidePanel;
        public static void Show(Grid container, string info, Type t, Position pos, Behaviors bs = null)
        {
            _sidePanel = new Flyout()
            {
                MaxWidth = 350,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                FontSize = 14,
                Background = App.SystemConfigs == null ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#925B21")) : new SolidColorBrush(App.SystemConfigs.FunctionsColor),
                Position = pos,
                Margin = new Thickness(0, 30, 0, 0),
                Content = new Grid()
            };

            switch (t)
            {
                case Type.Info:
                    {
                        _sidePanel.Header = "Информация";
                        break;
                    }
                case Type.Warning:
                    {
                        _sidePanel.Header = "Предупреждение";
                        break;
                    }
                case Type.Error:
                    {
                        _sidePanel.Header = "Ошибка";
                        break;
                    }
            }

            Canvas.SetZIndex(_sidePanel, 3);

            ((Grid)_sidePanel.Content).Children.Add(new TextBox()
            {
                Background = null,
                FontSize = 14,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                BorderThickness = new Thickness(0),
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                Padding = new Thickness(5),
                Margin = new Thickness(18, 0, 17, 38),
                Text = info
            });

            ((Grid)_sidePanel.Content).Children.Add(new ListView()
            {
                Padding = new Thickness(5),
                Background = null,
                VerticalAlignment = VerticalAlignment.Bottom,
            });

            if (bs != null)
            {
                ((ListView)((Grid)_sidePanel.Content).Children[1]).Items.Add(new WrapPanel()
                {
                    Background = null,
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                if (bs.Yes != null)
                {
                    ((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Add(new Button()
                    {
                        Content = "Да",
                        FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                        Width = 80
                    });
                    ((Button)((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children[((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Count - 1]).Click += bs.Yes;
                }
                if (bs.No != null)
                {
                    ((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Add(new Button()
                    {
                        Content = "Нет",
                        FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                        Width = 80
                    });
                    ((Button)((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children[((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Count - 1]).Click += bs.No;
                }
                if (bs.OK != null)
                {
                    ((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Add(new Button()
                    {
                        Content = "Oк",
                        FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                        Width = 80
                    });
                    ((Button)((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children[((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Count - 1]).Click += bs.OK;
                }
                if (bs.Cancel != null)
                {
                    ((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Add(new Button()
                    {
                        Content = "Отмена",
                        FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                        Width = 80
                    });
                    ((Button)((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children[((WrapPanel)((ListView)((Grid)_sidePanel.Content).Children[1]).Items[0]).Children.Count - 1]).Click += bs.Cancel;
                }
            }

            container.Children.Add(_sidePanel);
            _sidePanel.IsOpen = true;
        }
    }
}