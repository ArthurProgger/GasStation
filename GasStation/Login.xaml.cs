using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data.Common;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace GasStation
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        //Таблицы должны располагать в иерархической последовательности - сверху вниз (для MSSQL)
        private string[] _tablesGasStation =
        {
            "posts", "sex", "stuffers", "fuel_suppliers", "fuel_types",
            "columns_types", "gas_columns", "fuel_manufacturers", "fuel_using" ,
            "fuels", "fuels_prices", "fuel_coming", "fuel_supplies", "fuel_supplies_content", "gas_columns_fuel_types"
        };
        private string[] _tablesGasStationShop =
{
            "products_types", "products", "sales", "units", "products_units",
            "units_expressions", "sales_content", "products_suppliers", "products_supplies" ,
            "products_supplies_content", "product_prices", "product_screens"
        };
        public Window Context { get; set; }

        public Login()
        {
            InitializeComponent();

            if (!File.Exists(App.ConfigsFileName))
                new FatClient(this).ShowDialog();

            using (StreamReader sr = new StreamReader(File.Open(App.ConfigsFileName, FileMode.Open)))
                App.SystemConfigs = JsonConvert.DeserializeObject<SystemConfigs>(sr.ReadToEnd());
        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Application.Current.Shutdown();
        private void Minimize_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => WindowState = WindowState.Minimized;

        private async void TryEnter_Click(object sender, RoutedEventArgs e)
        {
            enterContent.Visibility = Visibility.Hidden;
            load.Visibility = Visibility.Visible;

            App.SystemConfigs.Login = login.Text;
            App.SystemConfigs.Password = pass.Password;

            bool tryConnRez = false;

            switch (App.SystemConfigs.SelectedDBMS)
            {
                case DBMS.MSSQL:
                    {
                        if (Context is FatClient)
                        {
                            App.SystemConfigs.ConnectionStr = "master";
                            tryConnRez = await TryConnection(new SqlConnection(App.SystemConfigs.ConnectionStr));
                        }
                        else
                        {
                            App.SystemConfigs.ConnectionStr = "GasStation";
                            tryConnRez = await TryConnection(new SqlConnection(App.SystemConfigs.ConnectionStr));
                        }
                        break;
                    }
                case DBMS.MySQL:
                    {
                        if (await TryConnection(new MySqlConnection(App.SystemConfigs.ConnectionStr += "GasStation")))
                            Close();

                        break;
                    }
            }

            if (tryConnRez)
                Close();
        }

        private async Task<bool> TryConnection<T>(T c) where T : DbConnection
        {
            try
            {
                using (c)
                {
                    if (!App.SystemConfigs.ConnectionStr.Contains("GasStation"))
                        switch (App.SystemConfigs.SelectedDBMS)
                        {
                            case DBMS.MSSQL:
                                {
                                    await CreateObjectOfDataBase(new SqlCommand("CREATE DATABASE GasStation", c as SqlConnection));

                                    IEnumerable<PropertyInfo> queries = from q in typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic)
                                                                        where q.Name.Contains("GasStation_")
                                                                        select q;
                                    await CreateObjects(queries, "_Table", _tablesGasStation);
                                    await CreateObjects(queries, "_StoredProcedure", _tablesGasStation);

                                    await CreateObjectOfDataBase(new SqlCommand("CREATE DATABASE GasStationShop", c as SqlConnection));

                                    queries = from q in typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic)
                                                                        where q.Name.Contains("GasStationShop_")
                                                                        select q;

                                    await CreateObjects(queries, "_Table", _tablesGasStation);
                                    await CreateObjects(queries, "_StoredProcedure", _tablesGasStation);

                                    break;
                                }
                            case DBMS.MySQL:
                                {
                                    await CreateObjectOfDataBase(new MySqlCommand("", new MySqlConnection(App.SystemConfigs.ConnectionStr += $"Initial Catalog=GasStation;User ID={login.Text};Password={pass.Password};")));
                                    break;
                                }
                        }
                    else
                    {
                        await c.OpenAsync();
                    }
                }

                return true;
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);

                enterContent.Visibility = Visibility.Visible;
                load.Visibility = Visibility.Hidden;

                return false;
            }
        }

        private async Task CreateObjects(IEnumerable<PropertyInfo> queries, string filter, string[] collection)
        {
            foreach (string tName in collection)
            {
                IEnumerable<PropertyInfo> query = from q in queries
                                                  orderby
                                                  q.Name.Length
                                                  where q.Name.Contains(tName) && q.Name.Contains(filter)
                                                  select q;
                try
                {
                    await CreateObjectOfDataBase(new SqlCommand(query.ToArray()[0].GetValue(typeof(Properties.Resources)).ToString(), new SqlConnection(App.SystemConfigs.ConnectionStr += $"Initial Catalog=GasStation;User ID={login.Text};Password={pass.Password};")));
                    await CreateObjectOfDataBase(new SqlCommand(query.OrderByDescending(q => q.Name.Length).ToArray()[0].GetValue(typeof(Properties.Resources)).ToString(), new SqlConnection(App.SystemConfigs.ConnectionStr += $"Initial Catalog=GasStation;User ID={login.Text};Password={pass.Password};")));
                }
                catch
                {
                    continue;
                }
            }
        }

        private async Task CreateObjectOfDataBase<T>(T comm) where T : DbCommand
        {
            using (comm)
            {
                await comm.Connection.OpenAsync();
                comm.ExecuteNonQuery();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => new MainWindow().Show();
    }
}