using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private void ProductNewSupp_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (productNewSupp.Visibility == Visibility.Visible)
            {
                try
                {
                    DataTable suppliers = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_suppliers", App.SystemConfigs.ConnectionStr));
                    App.SystemConfigs.ConnectionStr = "GasStation";
                    DataTable stuffers = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_stuffers", App.SystemConfigs.ConnectionStr));
                    App.SystemConfigs.ConnectionStr = "GasStationShop";

                    suppliers.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => productsSuppliers.Items.Add($"{r[0]}, {r[1]}"));
                    stuffers.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => stuffersList.Items.Add($"{r[0]}, {r[1]}"));
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
                }
            }
            else
            {
                productsSuppliers.Items.Clear();
                stuffersList.Items.Clear();
            }
        }

        private void SaveProductSupply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Query(new SqlCommand($"INSERT INTO products_supplies VALUES ('{prodSuppDateTime.SelectedDateTime}', {productsSuppliers.SelectedItem.ToString().Substring(0, productsSuppliers.SelectedItem.ToString().Length - new string(productsSuppliers.SelectedItem.ToString().Reverse().ToArray()).IndexOf(',') - 1)}, {stuffersList.SelectedItem.ToString().Substring(0, stuffersList.SelectedItem.ToString().Length - new string(stuffersList.SelectedItem.ToString().Reverse().ToArray()).IndexOf(',') - 1)})", GetConnectionObj<SqlConnection>()));
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
            }
        }
    }
}