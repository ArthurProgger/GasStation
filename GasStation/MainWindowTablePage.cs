using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data;

namespace GasStation
{
    public partial class MainWindow
    {
        private void TablePage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (table.Visibility == Visibility.Visible)
                title.Content = _tableSynonyms.ContainsKey(_tableName) ? _tableSynonyms[_tableName].TableSynonym : _tableName;
            else
            {
                if (this.content.Children.Contains(_savePdfButton))
                    this.content.Children.Remove(_savePdfButton);

                filterTables.Items.Clear();
                _dataTable = null;
                title.Content = _AppName;
                _columnsTypes.Clear();
                ClearTable(tablePage);
            }
        }

        private void SearchTables_TextChanged(object sender, TextChangedEventArgs e) => SearchTables(tablePage, tableContent, searchTables);
        private void SearchTables(Grid content, ScrollViewer sv, TextBox searchBox)
        {
            DataTable data = new DataTable();
            _dataTable.Columns.Cast<DataColumn>().ToList().ForEach((DataColumn c) => data.Columns.Add(c.ColumnName, c.DataType));
            Grid children = (Grid)((ScrollViewer)((Grid)sv.Content).Children[((Grid)sv.Content).Children.Count - 1]).Content;

            for (int i = 0; i < _dataTable.Rows.Count; i++)
            {
                if (filterTables.Items.Cast<CheckBox>().All(cb => !cb.IsChecked.Value))
                {
                    if (_dataTable.Rows[i].ItemArray.ToList().Exists(o => o.ToString().Contains(searchBox.Text)))
                        data.Rows.Add(_dataTable.Rows[i].ItemArray);
                }
                else if (filterTables.Items.Cast<CheckBox>().Where(cb => cb.IsChecked.Value).Any(cb => _dataTable.Rows[i][filterTables.Items.IndexOf(cb)].ToString().Contains(searchBox.Text)))
                    data.Rows.Add(_dataTable.Rows[i].ItemArray);
            }
            ClearTable(content);
            _columnsTypes.Clear();
            GenerateTable(content, _tableName, new string[0], true, _editable, _savePdf, data);
        }

        private void ClearTable(Grid content)
        {
            content.Children.Clear();
            content.ColumnDefinitions.Clear();
            content.RowDefinitions.Clear();
            _rowsOfTables.Clear();
        }
    }
}