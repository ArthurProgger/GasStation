using System;
using System.IO;
using System.Windows;
using System.Text;
using SWF = System.Windows.Forms;
using System.Windows.Controls;
using System.Linq;
using MahApps.Metro.Controls;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private bool _fuelSuppliers;
        private int _supplierId = -1;
        private void CompaniesPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (companiesPage.Visibility == Visibility.Visible)
            {
                if (_supplierId != -1)
                {
                    DataTable t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"SELECT * FROM suppliers WHERE id = {_supplierId}", App.SystemConfigs.ConnectionStr));
                    compShortName.Text = t.Rows[0][1].ToString();
                    compFullName.Text = t.Rows[0][2].ToString();
                    inn.Value = (long)t.Rows[0][3];
                    ogrn.Value = (long)t.Rows[0][4];

                    int length = BitConverter.ToInt32(((byte[])t.Rows[0][5]).Take(4).ToArray(), 0);
                    contractPath.Text = Encoding.Default.GetString((byte[])t.Rows[0][5], 4, length);
                }
            }
            else
            {
                _supplierId = -1;
                companiesList.Items.Clear();
                ClearCompanyData();
            }
        }

        private void SearchCompanies_Click(object sender, RoutedEventArgs e) => App.SearchCompanies(companiesList, companiesList.Text, App.SystemConfigs.KeyAPI);
        private void CompaniesList_TextChanged(object sender, TextChangedEventArgs e) => ClearCompanyData();

        private void ClearCompanyData() => compData.Children.Cast<UIElement>().Where(uie => uie is TextBox || uie is DatePicker || uie is NumericUpDown).ToList().ForEach((UIElement uie) =>
        {
            compData.IsEnabled = true;
            if (uie is TextBox)
                uie.GetType().GetMethod("Clear").Invoke(uie, new object[0]);
            else if (uie is DatePicker)
                uie.GetType().GetProperty("SelectedDate").SetValue(uie, DateTime.Now);
            else
                uie.GetType().GetProperty("Value").SetValue(uie, 0d);
        });

        private void CompaniesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (companiesList.SelectedIndex != -1)
                App.Companies.ForEach((FTSCompanyData comp) =>
                {
                    if (string.Equals(comp.НаимПолнЮЛ, companiesList.SelectedItem.ToString()))
                    {
                        compFullName.Text = comp.НаимПолнЮЛ;
                        compShortName.Text = comp.НаимСокрЮЛ;
                        inn.Value = double.Parse(comp.ИНН);
                        ogrn.Value = double.Parse(comp.ОГРН);
                        compData.IsEnabled = false;
                    }
                });
        }

        private void SelectContractFile_Click(object sender, RoutedEventArgs e)
        {
            SWF.FileDialog fd = new SWF.OpenFileDialog
            {
                Filter = "Документ Word|*.doc;*.docx;*.exe|Текстовый файл|*.txt"
            };
            fd.ShowDialog();
            contractPath.Text = fd.FileName;
        }

        private void SaveCompData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tableName = "suppliers";
                switch (App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            SqlParameter p;
                            if (File.Exists(contractPath.Text))
                            {
                                byte[] buffer = new byte[16 * 1024];
                                using (BinaryReader input = new BinaryReader(File.Open(contractPath.Text, FileMode.Open)))
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    int read;
                                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                                        ms.Write(buffer, 0, read);

                                    p = new SqlParameter("p", BitConverter.GetBytes(contractPath.Text.Length).Concat(Encoding.Default.GetBytes(contractPath.Text).Concat(GetFileType(contractPath.Text).Concat(ms.ToArray()).ToArray()).ToArray()).ToArray());
                                }
                            }
                            else
                                throw new Exception("Файл договора некорректен!");


                            SqlCommand comm = new SqlCommand($"INSERT INTO {tableName} VALUES ('{compShortName.Text}', '{compFullName.Text}', {inn.Value}, {ogrn.Value}, @p)", GetConnectionObj<SqlConnection>());
                            comm.Parameters.Add(p);
                            Query(comm);

                            break;
                        }
                    case DBMS.MySQL:
                        {
                            Query(new MySqlCommand($"", GetConnectionObj<MySqlConnection>()));
                            break;
                        }
                }
                menu.SelectedIndex = 0;
                SideMessage.Show(Content as Grid, "Поставщик добавлен!", SideMessage.Type.Info, Position.Right);
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
            }
        }
    }
}