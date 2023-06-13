using System.Windows;
using System.Collections.Generic;

namespace GasStation
{
    public enum DBMS
    {
        MSSQL, MySQL, PSSQL
    }

    public enum BackupFrequencies
    {
        Everyday, Everyweek, Everymonth
    }

    /// <summary>
    /// Свойства приложения
    /// </summary>
    public partial class App : Application
    {
        private static readonly List<FrameworkElement> _pagesHistory = new List<FrameworkElement>();

        public static int DelayAnimation => 1;
        public static string ConfigsFileName => "Configs.json";
        public static SystemConfigs SystemConfigs { get; set; }
        public static List<FrameworkElement> PagesHistory => _pagesHistory;
    }
}