using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine {
    
    /// <summary>
    /// helper class for reading csv files; may be unnecessary as I don't want to create an output file
    /// </summary>
    public class CsvRow {
        public string name { get; set; }
        public string info { get; set; }
        public List<string> alts { get; set; }
    }
}
