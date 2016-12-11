using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommandLine {

    [XmlRootAttribute("root", Namespace = "http://www.tei-c.org/ns/1.0", IsNullable = false)]
    public class DocumentCollection {
        public DocumentCollection() {
            Documents = new List<ParsedDocument>();
        }

        public List<ParsedDocument> Documents { get; set; }
    }
}
