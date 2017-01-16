using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine
{
    /// <summary>
    /// Additional info for parsing documents that describes the document structure
    /// </summary>
    public static class ParsingInfo
    {
        /// <summary>
        /// The start of each general headline in order of their appearance in the document.
        /// Used for parsing.
        /// </summary>
        public static readonly string[] GeneralInfoHeadlines =
            {
                "Werk",
                "Werktitel",
                "Werktitel – alternative Schreibweisen",
                "Verfasser",
                "Lebensdaten des Verfassers",
                "Abfassungszeitraum",
                "Abfassungsort",
                "Region",
                "Editionshinweise",
                "Allgemeines"
            };

        /// <summary>
        /// The start of each headline for a single source ("Quellenstelle") in order of their appearance in the document.
        /// Used for parsing.
        /// </summary>
        public static readonly string[] SourceHeadlines =
            {
                "Quellenstelle",
                "Zitation",
                "zeitliche (Quellen-)Angabe",
                "Inhaltsangabe",
                "Volltext",
                "Übersetzung",
                "Hinweise zur Übersetzung (Zitation)",
                "zeitliche (wissenschaftliche) Einordnung",
                "geographisches Stichwort",
                "Bericht über ein/mehrere Individuum/en oder Kollektive",
                "Interaktion (j/n)",
                "Auffälligkeiten",
                "Suchbegriffe der Stelle (mit Semikolon trennen)",
                "Anmerkungen"
            };


        // The Word styles that are used for headlines
        public static readonly string[] HeadlineParagraphStyles =
            {
                "Vorlage_Aufnahme_Quellenstelle"
            };

        // The Word styles that are used for text bodies
        public static readonly string[] TextBodyParagraphStyles =
            {
                "Standard",
                "Standard (Web)"
            };
    }
}
