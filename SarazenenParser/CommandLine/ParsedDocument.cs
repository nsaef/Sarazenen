using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine
{
    /// <summary>
    /// A parsed docx document that can be prepared to be written to TEI XML.
    /// </summary>
    public class ParsedDocument
    {
        public ParsedDocument()
        {
            ParsingExceptions = new List<string>();
            AlternativeTitles = new List<string>();
            AuthorNames = new List<string>();
            Regions = new List<string>();
            Sources = new List<Source>();
        }

        /// <summary>
        /// Contains any problems that were encountered while trying to parse the document
        /// </summary>
        public List<string> ParsingExceptions { get; set; }

        /// <summary>
        /// Werk: Zahlencode
        /// </summary>
        public string IDstring { get; set; }

        /// <summary>
        /// Werktitel
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Werktitel - alternative Schreibweisen
        /// </summary>
        public List<string> AlternativeTitles { get; set; }

        /// <summary>
        /// Verfasser
        /// </summary>
        public List<string> AuthorNames { get; set; }

        /// <summary>
        /// Lebensdaten des Verfassers
        /// </summary>
        public string AuthorLifespan { get; set; }

        /// <summary>
        /// Abfassungszeitraum
        /// </summary>
        public string TimePeriod { get; set; }

        /// <summary>
        /// Abfassungsort
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public List<string> Regions { get; set; }

        /// <summary>
        /// Editionshinweise
        /// </summary>
        public string EditionInfo { get; set; }

        /// <summary>
        /// Allgemeines
        /// Not in the guidelines but in the actual documents! (e.g. [0001])
        /// </summary>
        public string GeneralInfo { get; set; }

        /// <summary>
        /// Quellenstellen
        /// </summary>
        private List<Source> Sources { get; set; }
    }

    /// <summary>
    /// Part of a ParsedDocument. Contains the data of a single source from the document.
    /// </summary>
    public class Source
    {
        // TODO
    }
}
