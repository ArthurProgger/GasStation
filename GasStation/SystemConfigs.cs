using System;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace GasStation
{
    /// <summary>
    /// Конфиги приложения
    /// </summary>
    public class SystemConfigs
    {
        public class BackupSettings
        {
            public DateTime Time { get; set; }
            public BackupFrequencies Frequency { get; set; }
        }

        public DBMS SelectedDBMS { get; set; }
        public MySqlSslMode SslMode { get; set; }

        public string Server { get; set; }
        public int Port { get; set; }

        private string _connStr;
        public string ConnectionStr
        {
            get => _connStr;
            set
            {
                switch (SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            _connStr = Port == 0 ? $"Data Source={Server};Initial Catalog={value};Persist Security Info=True;User ID={Login};Password={Password};" : $"Data Source={Server},{Port};Initial Catalog={value};Persist Security Info=True;User ID={Login};Password={Password};";
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            _connStr = $"datasource={Server};port={Port};SslMode={SslMode};{value}";
                            break;
                        }
                }
            }
        }
        public string KeyAPI { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Color MainColor { get; set; }
        public Color FunctionsColor { get; set; }

        public bool UseWin8TilesStyle { get; set; }
        public bool HaveShop { get; set; }

        public BackupSettings BS { get; set; }
    }
}