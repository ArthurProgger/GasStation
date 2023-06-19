using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Data;

namespace GasStation
{
    public partial class MainWindow : Window
    {
        // глобальный контекст MainWindow

        private const string _AppName = @"АИС ""Автозаправочная станция""", _cacheFileName = "CacheFile";

        private int _gasColumnNum = -1, _stuffId = -1;
        private string _tableName, _where, _userFullName;
        private bool _useFT, _isInsert, _editable, _savePdf;
        private object[] _keysValues;
        private string[] _filesTypes = { "png", "jpeg", "jpg", "doc", "docx", "txt" };
        private Button _savePdfButton;
        private DataTable _dataTable;
        private Dictionary<string, RoutedEventHandler> _additionalButtons, _additionalButtonsTable;
        private RoutedEventHandler _editClick;
        private List<string> _productData = new List<string>();
        private List<Process> _openedFiles = new List<Process>();
        private Dictionary<string, Dictionary<UIElement, string>> _rowsOfTables = new Dictionary<string, Dictionary<UIElement, string>>();
        private Dictionary<ContentControl, MouseButtonEventHandler> _tilesCollection = new Dictionary<ContentControl, MouseButtonEventHandler>();
        private Dictionary<string, TablesSynonyms> _tableSynonyms = new Dictionary<string, TablesSynonyms>();
        private Dictionary<string, Columns> _columnsTypes = new Dictionary<string, Columns>();
        private Dictionary<string, string[]> _foreignTables = new Dictionary<string, string[]>();
        private Dictionary<UIElement, byte[]> _binaryDataControls = new Dictionary<UIElement, byte[]>();
    }
}