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
        }

        /// <summary>
        /// Serialize all data as XML
        /// </summary>
        /// <param name="docs">Collection of all documents + person and place registry</param>
        public void writeXml(DocumentCollection docs) {
            //write a complete file with all data
            XmlSerializer serializer = new XmlSerializer(typeof(DocumentCollection));
            TextWriter writer = new StreamWriter(@"C:\Users\Amin\Documents\GitHub\sarazenen\Quellen\Auszug\XML\test.xml");

            serializer.Serialize(writer, docs);

            writer.Close();

            //write a file for the person registry
            XmlSerializer serializerPersons = new XmlSerializer(typeof(List<Person>));
            TextWriter writerPersons = new StreamWriter(@"C:\Users\Amin\Documents\GitHub\sarazenen\Quellen\Auszug\XML\Personen.xml");

            serializerPersons.Serialize(writerPersons, docs.Persons);

            writerPersons.Close();

            //write a file for the place registry
            XmlSerializer serializerPlaces = new XmlSerializer(typeof(List<Place>));
            TextWriter writerPlaces = new StreamWriter(@"C:\Users\Amin\Documents\GitHub\sarazenen\Quellen\Auszug\XML\Orte.xml");

            serializerPlaces.Serialize(writerPlaces, docs.Places);

            writerPlaces.Close();

            //write separate files for each document
            foreach (var doc in docs.Documents) {
                XmlSerializer serializerSeparateFiles = new XmlSerializer(typeof(ParsedDocument));
                    TextWriter writerSeparateFiles = new StreamWriter(@"C:\Users\Amin\Documents\GitHub\sarazenen\Quellen\Auszug\XML\Files\" + doc.Title + ".xml");
                    serializerSeparateFiles.Serialize(writerSeparateFiles, doc);

                    writerSeparateFiles.Close();
            }
        }

    }
}
