using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommandLine {
    /// <summary>
    /// Container element for all documents
    /// </summary>
    //[XmlRootAttribute("teiCorpus", Namespace = "http://www.tei-c.org/ns/1.0", IsNullable = false)]
    [XmlRootAttribute("Sarazenen", IsNullable = false)]
    public class DocumentCollection {
        public DocumentCollection() {
            Persons = new List<Person>();
            Places = new List<Place>();
            Documents = new List<ParsedDocument>();
        }

        [XmlArray("Personen")]
        [XmlArrayItem(typeof(Person), ElementName = "Person")]
        public List<Person> Persons { get; set; }

        [XmlArray("Orte")]
        [XmlArrayItem(typeof(Place), ElementName = "Ort")]
        public List<Place> Places { get; set; }

        [XmlElement(ElementName = "Dokumente")]
        public List<ParsedDocument> Documents { get; set; }       
    }


    /// <summary>
    /// Helper class for central person registry
    /// </summary>
    public class Person {

        public Person() {
            Id = Guid.NewGuid();
            AltNames = new List<string>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        [XmlElement(ElementName = "Rolle")]
        public string Role { get; set; }

        [XmlArray("Alternativnamen")]
        [XmlArrayItem(typeof(string), ElementName = "Name")]
        public List<string> AltNames { get; set; }
    }

    /// <summary>
    /// Helper class for central place registry
    /// </summary>
    public class Place {

        public Place() {
            Id = Guid.NewGuid();
            AltNames = new List<string>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        [XmlElement(ElementName = "Typ")]
        public string Type { get; set; }

        [XmlArray("Alternativnamen")]
        [XmlArrayItem(typeof(string), ElementName = "Name")]
        public List<string> AltNames { get; set; }
    }

}
