using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CommandLine {
    public class TeiHeader {
        public TeiHeader() {
            fileDesc = new FileDesc();
        }
        public TeiHeader(FileDesc f) {
            fileDesc = f;
        }

        public FileDesc fileDesc { get; set; }
    }

    public class FileDesc {
        public FileDesc() {
            titleStmt = new List<string>();
            publicationStmt = new List<string>();
        }

        public FileDesc(String title, String pub, SourceDesc source) {
            titleStmt = new List<string> { title };
            publicationStmt = new List<string> { pub };
            sourceDesc = source;
        }

        [XmlArray("titleStmt")]
        [XmlArrayItem(typeof(string), ElementName = "title")]
        public List<String> titleStmt;

        [XmlArray("publicationStmt")]
        [XmlArrayItem(typeof(string), ElementName = "p")]
        public List<String> publicationStmt;

        public SourceDesc sourceDesc { get; set; }
    }
}

public class SourceDesc {
    public SourceDesc() {
        biblStruct = new BiblStruct();
    }
    public SourceDesc(BiblStruct bibl) {
        biblStruct = bibl;
    }
    public SourceDesc(String text) {
        desc = text;
    }

    [XmlElement(ElementName = "p")]
    public String desc { get; set; }


    public BiblStruct biblStruct { get; set; }
}

public class BiblStruct {
    public BiblStruct() {
        monogr = new Monogr();
    }
    public BiblStruct(Monogr m) {
        monogr = m;
    }

    Monogr monogr { get; set; }
}

public class Monogr {
    public Monogr() {
        imprint = new Imprint();
    }
    public Monogr(String t, String e, Imprint i) {
        title = t;
        editor = e;
        imprint = i;
    }

    public String title { get; set; }
    public String editor { get; set; }
    public Imprint imprint { get; set; }
}

public class Imprint {
    public Imprint() {}
    public Imprint(String place, String iDate, String scope) {
        pubPlace = place;
        date = iDate;
        biblScope = scope;
    }

    public String pubPlace { get; set; }
    public String date { get; set; }
    public String biblScope { get; set; }
}
