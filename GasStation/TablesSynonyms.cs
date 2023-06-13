using System.Collections.Generic;

namespace GasStation
{
    public class TablesSynonyms
    { 
        public string TableSynonym { get; set; }

        private readonly Dictionary<string, string> _columnsSynonyms = new Dictionary<string, string>();
        public Dictionary<string, string> ColumnsSynonyms => _columnsSynonyms;
    }
}