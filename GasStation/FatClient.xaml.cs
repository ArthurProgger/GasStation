using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GasStation
{
    /// <summary>
    /// Логика взаимодействия для FatClient.xaml
    /// </summary>
    public partial class FatClient : Window
    {
        private DBMS _dbms;

        private string _server;
        private int _port = 0;

        public FatClient(Login log)
        {
            InitializeComponent();
            menu.SelectedIndex = 1;

            log.Context = this;

            foreach (MySqlSslMode v in Enum.GetValues(typeof(MySqlSslMode)))
                sslModes.Items.Add(v);

            connectionStr.Text = App.SystemConfigs == null ? null : App.SystemConfigs.ConnectionStr;

            mainColor.SelectedColor = App.SystemConfigs == null ? (Color)ColorConverter.ConvertFromString("#E6BC8E") : App.SystemConfigs.MainColor;
            functionsColor.SelectedColor = App.SystemConfigs == null ? (Color)ColorConverter.ConvertFromString("#925B21") : App.SystemConfigs.FunctionsColor;

            useBackup.IsOn = App.SystemConfigs == null ? true : App.SystemConfigs.BS != null;
            backupTime.SelectedDateTime = useBackup.IsOn ? DateTime.Now : App.SystemConfigs.BS.Time;
            freqBackup.SelectedIndex = useBackup.IsOn ? 0 : new List<BackupFrequencies>((BackupFrequencies[])Enum.GetValues(typeof(BackupFrequencies))).IndexOf(App.SystemConfigs.BS.Frequency);

            haveShop.IsOn = App.SystemConfigs == null ? true : App.SystemConfigs.HaveShop;

            setCompanyData.IsOn = App.SystemConfigs == null ? true : !string.IsNullOrEmpty(App.SystemConfigs.KeyAPI);
            keyAPI.Text = App.SystemConfigs == null ? null : App.SystemConfigs.KeyAPI;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e) => Application.Current.Shutdown();

        private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (Window win in Application.Current.Windows)
                win.WindowState = WindowState.Minimized;
        }

        private void UpPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MainColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            windowExample.Background = new SolidColorBrush(mainColor.SelectedColor.Value);

            byte r = mainColor.SelectedColor.Value.R,
                 g = mainColor.SelectedColor.Value.G,
                 b = mainColor.SelectedColor.Value.B;

            upPanelExample.Background = new SolidColorBrush(Color.FromRgb(r -= 30, g -= 30, b -= 30));
            titleExample.Background = upPanelExample.Background;
            App.InverseForeground(titleExample);
        }

        private void FunctionsColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            sidePanelExample.Background = new SolidColorBrush(functionsColor.SelectedColor.Value);
            App.InverseForeground(sidePanelExample);

            win8StylePanel.Background = new SolidColorBrush(functionsColor.SelectedColor.Value);
            win8Style.Foreground = sidePanelExample.Foreground;

            foreach (ListViewItem lvi in tilesExample.Items)
            {
                lvi.Background = new SolidColorBrush(functionsColor.SelectedColor.Value);
                App.InverseForeground(lvi);
            }

            foreach (UIElement el in tableExample.Children)
                if (el.GetType() == typeof(Border) && Grid.GetRow(el) == 0)
                {
                    ((Border)el).Background = new SolidColorBrush(functionsColor.SelectedColor.Value);
                    //App.InverseForeground((Label)el);
                }
        }

        private async void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (menu.SelectedIndex != -1)
            {
                if (menu.SelectedItem.Equals(connectionMenu))
                    await App.OpenFunction(connectionSettings, menu, Width - menu.ActualWidth);
                else if (menu.SelectedItem.Equals(designMenu))
                    await App.OpenFunction(designSettings, menu, Width - menu.ActualWidth);
                else if (menu.SelectedItem.Equals(behaviorSystemMenu))
                    await App.OpenFunction(behaviorSystemSettings, menu, Width - menu.ActualWidth);
                else
                {
                    menu.SelectedIndex = -1;
                    SideMessage.Show(Content as Grid, "Сохранить текущие настройки?", SideMessage.Type.Warning, MahApps.Metro.Controls.Position.Right, new SideMessage.Behaviors()
                    {
                        Yes = (object s, RoutedEventArgs rea) =>
                        {
                            if (FillCheck())
                            {
                                string json = JsonConvert.SerializeObject(new SystemConfigs
                                {
                                    SelectedDBMS = _dbms,
                                    Server = _server,
                                    Port = _port,
                                    SslMode = sslModes.SelectedValue == null ? MySqlSslMode.None : (MySqlSslMode)sslModes.SelectedValue,
                                    MainColor = mainColor.SelectedColor.Value,
                                    FunctionsColor = functionsColor.SelectedColor.Value,
                                    HaveShop = haveShop.IsOn,
                                    UseWin8TilesStyle = win8Style.IsOn,
                                    KeyAPI = keyAPI.Text,
                                    BS = useBackup.IsOn ? new SystemConfigs.BackupSettings
                                    {
                                        Time = backupTime.SelectedDateTime ?? DateTime.Now,
                                        Frequency = freqBackup.SelectedIndex == -1 ? BackupFrequencies.Everyday : ((BackupFrequencies[])Enum.GetValues(typeof(BackupFrequencies)))[freqBackup.SelectedIndex]
                                    } : null
                                });

                                using (FileStream fs = File.Open(App.ConfigsFileName, FileMode.Create))
                                    fs.Write(Encoding.ASCII.GetBytes(json), 0, Encoding.ASCII.GetBytes(json).Length);

                                Close();
                            }
                            else
                                SideMessage.Show(Content as Grid, "Заполнены не все поля!", SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
                        }
                    });
                }
            }
        }

        // Выбор СУБД

        private void MSSQL_Checked(object sender, RoutedEventArgs e) => ChangeDBMS(DBMS.MSSQL);
        private void MySQL_Checked(object sender, RoutedEventArgs e) => ChangeDBMS(DBMS.MySQL);

        private void ChangeDBMS (DBMS dbms)
        {
            _dbms = dbms;
            GenerateConnectionStr();
        }

        private void DatasourceDefaultVal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            datasource.Focus();
            datasourceDefaultVal.Visibility = Visibility.Hidden;
        }

        private void PortDefaultVal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            port.Focus();
            portDefaultVal.Visibility = Visibility.Hidden;
        }

        private void Datasource_LostFocus(object sender, RoutedEventArgs e)
        {
            datasourceDefaultVal.Visibility = string.IsNullOrEmpty(datasource.Text) ? Visibility.Visible : Visibility.Hidden;
            _server = string.IsNullOrEmpty(datasource.Text) ? datasourceDefaultVal.Content.ToString() : datasource.Text;
            GenerateConnectionStr();
        }

        private void Port_LostFocus(object sender, RoutedEventArgs e)
        {
            portDefaultVal.Visibility = string.IsNullOrEmpty(port.Text.Trim()) ? Visibility.Visible : Visibility.Hidden;
            _port = string.Equals(port.Text, port.Mask.Replace('0', port.PromptChar)) ? 0 : int.Parse(port.Text);
            GenerateConnectionStr();
        }

        private void UsePort_Toggled(object sender, RoutedEventArgs e) => GenerateConnectionStr();

        private void GenerateConnectionStr()
        {
            if (IsLoaded)
            {
                if (sslModes.SelectedItem != null)
                    sslModes.Visibility = MSSQL.IsChecked.Value ? Visibility.Hidden : Visibility.Visible;

                switch (_dbms)
                {
                    case DBMS.MSSQL:
                        {
                            connectionStr.Text = usePort.IsOn ? $"Data Source={_server},{_port};Initial Catalog=GasStation;Persist Security Info=True;" : $"Data Source={_server};Initial Catalog=GasStation;Persist Security Info=True;";
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            connectionStr.Text = $"datasource={_server};port={_port};Database=GasStation;SslMode={sslModes.SelectedValue};";
                            break;
                        }
                }
            }
        }

        private void Datasource_TextChanged(object sender, TextChangedEventArgs e)
        {
            _server = string.IsNullOrEmpty(datasource.Text) ? datasourceDefaultVal.Content.ToString() : datasource.Text;
            GenerateConnectionStr();
        }

        private void Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            _port = string.Equals(port.Text, port.Mask.Replace('0', port.PromptChar)) ? 0 : int.Parse(port.Text);
            GenerateConnectionStr();
        }

        private void SslModes_SelectionChanged(object sender, SelectionChangedEventArgs e) => GenerateConnectionStr();

        private ListViewItem _sidePanelExampleLVI;
        private void SidePanelExample_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sidePanelExampleLVI != null)
            {
                _sidePanelExampleLVI.Background = new SolidColorBrush(functionsColor.SelectedColor.Value);
                App.InverseForeground(_sidePanelExampleLVI);
            }

            App.ItemSelectEffect(sidePanelExample);
            _sidePanelExampleLVI = (ListViewItem)sidePanelExample.SelectedItem;
        }

        private void FontSizes_SelectionChanged(object sender, SelectionChangedEventArgs e) => FontSizeChange(windowExample.Children);
        private void FontSizes_TextChanged(object sender, TextChangedEventArgs e) => FontSizeChange(windowExample.Children);

        private void FontSizeChange<T>(T children) where T : ICollection
        {
            double fontSize;

            if (double.TryParse(fontSizes.Text, out fontSize))
                foreach (UIElement el in children)
                {
                    if (el.GetType() == typeof(Grid))
                        FontSizeChange(((Grid)el).Children);
                    else if (el.GetType() == typeof(ListView))
                        FontSizeChange(((ListView)el).Items);
                    else
                        ((Control)el).FontSize = fontSize;
                }
        }

        private void Win8Style_Checked(object sender, RoutedEventArgs e)
        {
            tilesExample.Visibility = win8Style.IsOn ? Visibility.Hidden : Visibility.Visible;
            tilesWin8Example.Visibility = win8Style.IsOn ? Visibility.Visible : Visibility.Hidden;
        }

        private void UseBackup_Checked(object sender, RoutedEventArgs e)
        {
            if (useBackup.IsOn)
                App.CBFolding(backupSettings, double.NaN, 85, Visibility.Visible);
            else
                App.CBFolding(backupSettings, 0, double.NaN, Visibility.Hidden);
        }

        private void SetCompanyData_Checked(object sender, RoutedEventArgs e)
        {
            if (setCompanyData.IsOn)
                App.CBFolding(companyData, double.NaN, 80, Visibility.Visible);
            else
                App.CBFolding(companyData, 0, double.NaN, Visibility.Hidden);
        }

        private void UrlFTS_MouseDown(object sender, MouseButtonEventArgs e) => Process.Start("https://api-fns.ru");
        private bool FillCheck() => !string.IsNullOrEmpty(connectionStr.Text) && mainColor.SelectedColor != null && functionsColor.SelectedColor != null && (!useBackup.IsOn || backupTime.SelectedDateTime != null && freqBackup.SelectedIndex != -1) && (!setCompanyData.IsOn || !string.IsNullOrEmpty(keyAPI.Text));
    }
}