using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Text;
using System.Threading.Tasks;

namespace GasStation
{
    public partial class MainWindow
    {
        private void LoadTilesOfStuffers<T>() where T : ContentControl, new()
        {
            tiles.Items.Clear();
            _tilesCollection.Clear();

            _tilesCollection.Add(new T
            {
                Width = 120,
                Height = 120,
                Content = "Новый сотрудник"
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await App.OpenFunction(stuffers, menu, Width - menu.ActualWidth)));
            _tilesCollection.Add(new T
            {
                Width = 120,
                Height = 120,
                Content = "База сотрудников"
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                DataTable t = new DataTable();

                switch (App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("SELECT * FROM stuffers", App.SystemConfigs.ConnectionStr));
                            break;
                        }
                }
                await TablePageLoad("stuffers", true, true, false, t, new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    Grid container = (Grid)((Border)((Label)((Grid)((FrameworkElement)obj).Parent).Parent).Parent).Parent;
                    List<string> values = new List<string>();

                    int rIndex = Grid.GetRow((Border)((Label)((Grid)((FrameworkElement)obj).Parent).Parent).Parent);
                    container.Children.Cast<UIElement>().Where(c => !(((Border)c).Child is Label) && Grid.GetRow(c) == rIndex).ToList().ForEach((UIElement c) => values.Add(((TextBlock)((Border)c).Child).Text));

                    _stuffId = int.Parse(values[0]);
                    await App.OpenFunction(stuffers, menu, Width - menu.ActualWidth);
                }));
            }));

            _tilesCollection.ToList().ForEach((KeyValuePair<ContentControl, MouseButtonEventHandler> a) =>
            {
                tiles.Items.Add(a.Key);
                a.Key.MouseDown += a.Value;
                a.Key.AddHandler(MouseDownEvent, a.Value, true);
            });
        }

        private async void StuffersBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.SystemConfigs.ConnectionStr = "GasStationShop";
            if (App.SystemConfigs.UseWin8TilesStyle)
                await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
            else
                await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
        }
    }
}