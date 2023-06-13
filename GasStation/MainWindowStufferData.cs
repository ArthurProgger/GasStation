using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using SWF = System.Windows.Forms;
using System.Linq;
using MahApps.Metro.Controls;
using System.Data;
using System.Data.SqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private void Stuffers_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (stuffers.Visibility == Visibility.Visible)
            {
                try
                {
                    DataTable posts = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_posts", App.SystemConfigs.ConnectionStr)),
                        sexes = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_sex", App.SystemConfigs.ConnectionStr));

                    posts.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => stuffPost.Items.Add(r[0]));
                    sexes.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => stuffSex.Items.Add(r[0]));

                    if (_stuffId != -1)
                    {
                        DataTable stuffInfo = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec [get_stuffers] {_stuffId}", App.SystemConfigs.ConnectionStr));

                        int length = BitConverter.ToInt32(((byte[])stuffInfo.Rows[0][9]).Take(4).ToArray(), 0);
                        stuffFile.Text = Encoding.Default.GetString((byte[])stuffInfo.Rows[0][9], 4, length);

                        stuffLastName.Text = stuffInfo.Rows[0][1].ToString();
                        stuffFirstName.Text = stuffInfo.Rows[0][2].ToString();
                        stuffMiddleName.Text = stuffInfo.Rows[0][3].ToString();
                        stuffBirth.SelectedDate = DateTime.Parse(stuffInfo.Rows[0][4].ToString());
                        stuffSeriesPass.Value = int.Parse(stuffInfo.Rows[0][5].ToString());
                        stuffNumPass.Value = int.Parse(stuffInfo.Rows[0][6].ToString());
                        stuffWhoGived.Text = stuffInfo.Rows[0][7].ToString();
                        stuffSex.Text = stuffInfo.Rows[0][8].ToString();
                        stuffPost.Text = stuffInfo.Rows[0][10].ToString();
                        stuffDepCode.Value = int.Parse(stuffInfo.Rows[0][11].ToString());
                        stuffTakeDate.SelectedDate = DateTime.Parse(stuffInfo.Rows[0][12].ToString());
                    }
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
            else
            {
                _stuffId = -1;
                stuffLastName.Clear();
                stuffFirstName.Clear();
                stuffMiddleName.Clear();
                stuffBirth.SelectedDate = DateTime.Now;
                stuffSeriesPass.Value = 0;
                stuffNumPass.Value = 0;
                stuffWhoGived.Clear();
                stuffSex.Items.Clear();
                stuffPost.Items.Clear();
            }
        }

        private async void SaveStuffer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(stuffFile.Text))
                    throw new Exception("Не выбран файл трудового договора!");

                byte[] buffer = new byte[16 * 1024];
                using (BinaryReader input = new BinaryReader(File.Open(stuffFile.Text, FileMode.Open)))
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, read);

                    SqlParameter p = new SqlParameter("@p", BitConverter.GetBytes(stuffFile.Text.Length).Concat(Encoding.Default.GetBytes(stuffFile.Text).Concat(GetFileType(stuffFile.Text).Concat(ms.ToArray()).ToArray()).ToArray()).ToArray());
                    SqlCommand comm = _stuffId == -1 ? new SqlCommand("INSERT INTO stuffers VALUES (" +
                        $"'{stuffLastName.Text}'," +
                        $"'{stuffFirstName.Text}'," +
                        $"'{stuffMiddleName.Text}'," +
                        $"'{stuffBirth.SelectedDate.Value}'," +
                        $"{stuffSeriesPass.Value.Value}," +
                        $"{stuffNumPass.Value.Value}," +
                        $"'{stuffWhoGived.Text}'," +
                        $"'{stuffSex.Text}'," +
                        $"@p," +
                        $"'{stuffPost.Text}'," +
                        $"{stuffDepCode.Value.Value}," +
                        $"'{stuffTakeDate.SelectedDate.Value}')", GetConnectionObj<SqlConnection>())
                        :
                        new SqlCommand("UPDATE stuffers SET " +
                        $"l_name = '{stuffLastName.Text}'," +
                        $"f_name = '{stuffFirstName.Text}'," +
                        $"m_name = '{stuffMiddleName.Text}'," +
                        $"bitrh = '{stuffBirth.SelectedDate.Value}'," +
                        $"p_series = {stuffSeriesPass.Value.Value}," +
                        $"p_num = {stuffNumPass.Value.Value}," +
                        $"p_gived = '{stuffWhoGived.Text}'," +
                        $"sex = '{stuffSex.Text}'," +
                        $"contract_file = @p," +
                        $"post = '{stuffPost.Text}'," +
                        $"p_code_dep = {stuffDepCode.Value.Value}," +
                        $"p_date = '{stuffTakeDate.SelectedDate.Value}' " +
                        $"WHERE id = {_stuffId}", GetConnectionObj<SqlConnection>());
                    comm.Parameters.Add(p);
                    Query(comm);
                }

                SideMessage.Show(Content as Grid, "Сотрудник сохранен!", SideMessage.Type.Info, Position.Right);
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
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
            }
        }

        private void StuffLoadFile_Click(object sender, RoutedEventArgs e)
        {
            SWF.FileDialog fd = new SWF.OpenFileDialog
            {
                Filter = "Документ Word|*.doc;*.docx"
            };
            fd.ShowDialog();
            stuffFile.Text = fd.FileName;
        }

        private async void StuffersBack1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.SystemConfigs.ConnectionStr = "GasStation";
            if (App.SystemConfigs.UseWin8TilesStyle)
                await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfStuffers<Tile>));
            else
                await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfStuffers<ListViewItem>));
        }
    }
}