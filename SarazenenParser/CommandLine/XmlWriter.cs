using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace CommandLine {
    class XmlWriter {
            public XmlWriter() {
            //doc = new ParsedDocument();
        }

        ///<summary>
        ///content of the currently-parsed document; possibly unnecessary
        ///</summary>
        //public ParsedDocument doc { get; set; }

        //ich will nur ein XML erzeugen -> ich brauche eine Datenstruktur mit allen ParsedDocuments
        public void writeXml(DocumentCollection docs) {
            XmlSerializer serializer = new XmlSerializer(typeof(DocumentCollection));
            TextWriter writer = new StreamWriter(@"C:\Users\Amin\Documents\GitHub\sarazenen\Quellen\Auszug\XML\test.xml");

            //write the xml root element
            serializer.Serialize(writer, docs);

            writer.Close();
        }

    }
}
