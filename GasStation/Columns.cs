using System.Collections.Generic;

namespace GasStation
{
    public class Columns
    {
        private readonly List<string> _strColumns = new List<string>(), _primaryColumns = new List<string>(), _binaryColumns = new List<string>(), _dateTimeColumns = new List<string>(), _dateColumns = new List<string>(), _timeColumns = new List<string>();
        private readonly Dictionary<string, Dictionary<string, string>> _foreignColumns = new Dictionary<string, Dictionary<string, string>>();
        public List<string> StrColumns => _strColumns;
        public List<string> DateTimeColumns => _dateTimeColumns;
        public List<string> DateColumns => _dateColumns;
        public List<string> TimeColumns => _timeColumns;
        public List<string> PrimaryColumns => _primaryColumns;
        public List<string> BinaryColumns => _binaryColumns;
        public Dictionary<string, Dictionary<string, string>> ForeignColumns => _foreignColumns;
    }
}