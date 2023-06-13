using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;

namespace GasStation
{
    //Страница "Заправка"
    public partial class MainWindow
    {
        private void LoadTilesOfGasStation<T>() where T : ContentControl, new()
        {
            tiles.Items.Clear();
            _tilesCollection.Clear();

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                Content = "Колонки",
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await App.OpenFunction(gasColumnsInfo, tiles, Width - menu.ActualWidth)));
            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                Content = "Резервуары",
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await App.OpenFunction(tanksInfo, tiles, Width - menu.ActualWidth)));

            _tilesCollection.ToList().ForEach((KeyValuePair<ContentControl, MouseButtonEventHandler> a) =>
            {
                tiles.Items.Add(a.Key);
                a.Key.MouseDown += a.Value;
                a.Key.AddHandler(MouseDownEvent, a.Value, true);
            });
        }
    }
}