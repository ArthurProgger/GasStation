using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using MahApps.Metro.Controls;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private void ProductInfoPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (productInfoPage.Visibility == Visibility.Visible)
            {
                try
                {
                    DataTable t = new DataTable(), prodData = new DataTable();
                    switch (App.SystemConfigs.SelectedDBMS)
                    {
                        case DBMS.MSSQL:
                            {
                                t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_product_screens {_productData[0]}", App.SystemConfigs.ConnectionStr));
                                prodData = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec [get_products] NULL, {_productData[0]}", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                        case DBMS.MySQL:
                            {
                                t = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter($"call get_product_screens ", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                    }

                    productsScreens.BannerText = _productData[1];
                    productDescription.Text = _productData[2];

                    t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                    {
                        int lenght = BitConverter.ToInt32(((byte[])r[0]).Take(4).ToArray(), 0) + 4;
                        using (MemoryStream ms = new MemoryStream((byte[])r[0], lenght + 4, ((byte[])r[0]).Length - (lenght + 4)))
                        {
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.StreamSource = ms;
                            bi.EndInit();
                            productsScreens.Items.Add(new Border
                            {
                                CornerRadius = new CornerRadius(10),
                                Child = new Image
                                {
                                    Source = bi
                                }
                            });
                        }
                    });
                    productCount.Content = prodData.Rows[0][4] == null ? "В наличии: 0" : $"В наличии: {prodData.Rows[0][4]}";
                    productPrice.Content = prodData.Rows[0][6] == null ? "Цена: <Не установлено>" : $"Цена: {decimal.Parse(prodData.Rows[0][6].ToString()).ToString("000000.00")} руб.";
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
            else
            {
                productsScreens.Items.Clear();
                _productData.Clear();
                productDescription.Text = null;
            }
        }
    }
}