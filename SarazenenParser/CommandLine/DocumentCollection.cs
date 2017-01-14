using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommandLine {

    [XmlRootAttribute("teiCorpus", Namespace = "http://www.tei-c.org/ns/1.0", IsNullable = false)]
    public class DocumentCollection {
        public DocumentCollection() {
            Documents = new List<ParsedDocument>();
            //SourceDesc sourceDesc = new SourceDesc("Beschreibender Text");
            //FileDesc fileDesc = new FileDesc("Sarazenen", "Literatursammlung zu Sarazenen der Bonner Mediaevisten", sourceDesc);
            //header = new TeiHeader(fileDesc);
        }

        //[XmlElement(ElementName = "teiHeader")]
        //public TeiHeader header { get; set; }

        //[XmlElement(ElementName = "teiCorpus", Namespace = "http://www.tei-c.org/ns/1.0")]
        [XmlElement(ElementName = "Dokumente")]
        public List<ParsedDocument> Documents { get; set; }
    }
}
