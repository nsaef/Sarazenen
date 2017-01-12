using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommandLine
{
    /// <summary>
    /// A parsed docx document that can be prepared to be written to TEI XML.
    /// </summary>
    public class ParsedDocument
    {
        public ParsedDocument()
        {
            AlternativeTitles = new List<string>();
            AuthorNames = new List<string>();
            Regions = new List<string>();
            Sources = new List<Source>();
            DebugInfo = new DebugInfo();
        }

        /// <summary>
        /// For debugging/analysis purposes. Returns a string that reports all content properties that are NullOrEmpty(tm).
        /// </summary>
        /// <returns></returns>
        public string ReportNullOrEmptyProperties()
        {
            string result = "";

            result += string.IsNullOrEmpty(IDString) ? "\tIDString\n" : "";
            result += string.IsNullOrEmpty(Title) ? "\tTitle\n" : "";
            result += AlternativeTitles.Count == 0 ? "\tAlternativeTitles\n" : "";
            result += AuthorNames.Count == 0 ? "\tAuthorNames\n" : "";
            result += string.IsNullOrEmpty(AuthorLifespan) ? "\tAuthorLifespan\n" : "";
            result += string.IsNullOrEmpty(TimePeriod) ? "\tTimePeriod\n" : "";
            result += Regions.Count == 0 ? "\tRegions\n" : "";
            result += string.IsNullOrEmpty(EditionInfo) ? "\tEditionInfo\n" : "";
            result += string.IsNullOrEmpty(GeneralInfo) ? "\tGeneralInfo\n" : "";

            for (int i = 0; i < Sources.Count; i++ )
            {
                string sourceResult = Sources[i].ReportNullOrEmptyProperties();

                if(sourceResult != "")
                {
                    sourceResult = "\tSources["+i+"]:\n" + sourceResult;
                    result += sourceResult;
                }
            }

            if(result != "")
            {
                result = "File " + DebugInfo.FilePath + "\n" + result;
            }

            return result;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public DebugInfo DebugInfo { get; set; }

        /// <summary>
        /// Werk: Zahlencode
        /// </summary>
        [XmlElement(ElementName = "WerkId")]
        public string IDString { get; set; }

        /// <summary>
        /// Werktitel
        /// </summary>
        [XmlElement(ElementName = "WerkTitel")]
        public string Title { get; set; }

        /// <summary>
        /// Werktitel - alternative Schreibweisen
        /// </summary>
        [XmlArray("Alternativtitel")]
        [XmlArrayItem(typeof(string), ElementName = "Titel")]
        public List<string> AlternativeTitles { get; set; }

        /// <summary>
        /// Verfasser
        /// </summary>
        [XmlArray("Autoren")]
        [XmlArrayItem(typeof(string), ElementName = "Autor")]
        public List<string> AuthorNames { get; set; }

        /// <summary>
        /// Lebensdaten des Verfassers
        /// </summary>
        [XmlElement(ElementName = "LebensdatenVerfasser")]
        public string AuthorLifespan { get; set; }

        /// <summary>
        /// Abfassungszeitraum
        /// </summary>
        [XmlElement(ElementName = "Abfassungszeitraum")]
        public string TimePeriod { get; set; }

        /// <summary>
        /// Abfassungsort
        /// </summary>
         [XmlElement(ElementName = "Abfassungsort")]
        public string Location { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        [XmlArray("Regionen")]
        [XmlArrayItem(typeof(string), ElementName = "Region")]
        public List<string> Regions { get; set; }

        /// <summary>
        /// Editionshinweise
        /// </summary>
         [XmlElement(ElementName = "Editionshinweise")]
        public string EditionInfo { get; set; }

        /// <summary>
        /// Allgemeines
        /// Not in the guidelines but in the actual documents! (e.g. [0001])
        /// </summary>
         [XmlElement(ElementName = "Werkinformation")]
        public string GeneralInfo { get; set; }

        /// <summary>
        /// Quellenstellen
        /// </summary>
        [XmlArray("Quellen")]
        [XmlArrayItem(typeof(Source), ElementName = "Quelle")]
        public List<Source> Sources { get; set; }
    }

    /// <summary>
    /// Part of a ParsedDocument. Contains the data of a single source from the document.
    /// </summary>
    public class Source
    {

        public string ReportNullOrEmptyProperties()
        {
            string result = "";

            result += String.IsNullOrEmpty(IDString) ? "\t\tIDString\n" : "";
            result += String.IsNullOrEmpty(Citation) ? "\t\tCitation\n" : "";
            result += String.IsNullOrEmpty(SourceTime) ? "\t\tSourceTime\n" : "";
            result += String.IsNullOrEmpty(Summary) ? "\t\tSummary\n" : "";
            result += String.IsNullOrEmpty(TextOriginal) ? "\t\tTextOriginal\n" : "";
            result += String.IsNullOrEmpty(TextTranslated) ? "\t\tTextTranslated\n" : "";
            result += String.IsNullOrEmpty(TranslationInfo) ? "\t\tTranslationInfo\n" : "";
            result += String.IsNullOrEmpty(EstimatedActualTime) ? "\t\tEstimatedActualTime\n" : "";
            result += GeographicKeywords == null ? "\t\tGeographicKeywords\n" : "";
            result += ParticipantKeywords == null ? "\t\tParticipantKeywords\n" : "";
            result += String.IsNullOrEmpty(Interaction) ? "\t\tInteraction\n" : "";
            result += DistinctiveFeatures == null ? "\t\tDistinctiveFeatures\n" : "";
            result += SearchKeywords == null ? "\t\tSearchKeywords\n" : "";
            result += String.IsNullOrEmpty(Notes) ? "\t\tNotes\n" : "";

            return result;
        }


        /// <summary>
        /// Quellenstelle: [Zahlencode]
        /// </summary>
         [XmlElement(ElementName = "QuellenId")]
        public string IDString { get; set; }

        /// <summary>
        /// Zitation (Buch/Kapitel/Seite):
        /// </summary>
         [XmlElement(ElementName = "Zitation")]
        public string Citation { get; set; }

        /// <summary>
        /// zeitliche (Quellen-)Angabe:
        /// </summary>
         [XmlElement(ElementName = "ZeitangabeQuelle")]
        public string SourceTime { get; set; }

        /// <summary>
        /// Inhaltsangabe:
        /// </summary>
        [XmlElement(ElementName = "Inhaltsangabe")]
        public string Summary { get; set; }

        /// <summary>
        /// Volltext:
        /// (nicht übersetzt)
        /// </summary>
         [XmlElement(ElementName = "VolltextOriginalsprache")]
        public string TextOriginal { get; set; }

        /// <summary>
        /// Übersetzung:
        /// (deutsch)
        /// </summary>
         [XmlElement(ElementName = "VolltextUebersetzung")]
        public string TextTranslated { get; set; }

        /// <summary>
        /// Hinweise zur Übersetzung (Zitation):
        /// </summary>
         [XmlElement(ElementName = "ZitationUebersetzung")]
        public string TranslationInfo { get; set; }

        /// <summary>
        /// zeitliche (wissenschaftliche) Einordnung:
        /// </summary>
         [XmlElement(ElementName = "ZeitangabeWissenschaft")]
        public string EstimatedActualTime { get; set; }

        /// <summary>
        /// geographisches Stichwort:
        /// </summary>
        [XmlArray("GeographischesStichwort")]
        [XmlArrayItem(typeof(string), ElementName = "Ort")]
        public List<string> GeographicKeywords { get; set; }

        /// <summary>
        /// Bericht über ein/mehrere Individuum/en oder Kollektive: 
        /// </summary>
        [XmlArray("Beteiligte")]
        [XmlArrayItem(typeof(string), ElementName = "Beteiligter")]
        public List<string> ParticipantKeywords { get; set; }

        /// <summary>
        /// Interaktion (j/n):
        /// </summary>
        [XmlElement(ElementName = "Interaktion")]
        public string Interaction { get; set; }

        /// <summary>
        /// Auffälligkeiten:
        /// </summary>
        [XmlArrayItem(typeof(string), ElementName = "Schlagwort")]
        public List<string> DistinctiveFeatures { get; set; }

        /// <summary>
        /// Suchbegriffe der Stelle (mit Semikolon trennen): 
        /// </summary>
        [XmlArray("Suchbegriffe")]
        [XmlArrayItem(typeof(string), ElementName = "Suchwort")]
        public List<string> SearchKeywords { get; set; }

        /// <summary>
        /// Anmerkungen:
        /// </summary>
        [XmlElement(ElementName = "Anmerkungen")]
        public string Notes { get; set; }
    }

    /// <summary>
    /// Some encapsuled info that is not part of the actual data to be parsed but can be helpful for gaining insight into the parsing process.
    /// </summary>
    public class DebugInfo
    {
        public DebugInfo()
        {
            ParsingExceptions = new List<string>();
        }

        /// <summary>
        /// The absolute path to the file to be parsed.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Contains any problems that were encountered while trying to parse the document
        /// </summary>
        public List<string> ParsingExceptions { get; set; }

        public int LastParagraphReached = 0;
        public int ParagraphCount = 0;
    }
}
