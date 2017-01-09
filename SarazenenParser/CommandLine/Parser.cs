using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace CommandLine
{

    /// <summary>
    /// Contains the main parsing logic
    /// </summary>
    public static class Parser
    {

        public static ParsedDocument ParseDocument(string filePath)
        {
            // Where the parsed data is saved. This is also the return value.
            ParsedDocument parsedDocument = new ParsedDocument();
            Application app;
            Document document;
            // Open the given document as read-only
            try
            {
                app = new Application();
                try
                {
                    document = app.Documents.Open(filePath, ReadOnly: true);
                    try
                    {
                        if (document == null)
                        {
                            throw new Exception("Document " + filePath + " could not be loaded!");
                        }

                        int currentParagraphCount = 1;
                        int totalParagraphCount = document.Paragraphs.Count;
                        parsedDocument.DebugInfo.ParagraphCount = totalParagraphCount;

                        // The parsing is split into subfunctions to increase code readability
                        parsedDocument.DebugInfo.FilePath = filePath;
                        ParseGeneralInfo(ref document, ref parsedDocument, ref currentParagraphCount, totalParagraphCount);
                        while (currentParagraphCount < totalParagraphCount)
                        {
                            ParseSingleSource(ref document, ref parsedDocument, ref currentParagraphCount, totalParagraphCount);
                            if (string.IsNullOrEmpty(parsedDocument.Sources.Last().IDString))
                            {
                                parsedDocument.Sources.Remove(parsedDocument.Sources.Last());
                                break;
                            }
                        }
                        parsedDocument.DebugInfo.LastParagraphReached = currentParagraphCount;

                        // TODO: Close app?

                    }
                    catch (Exception e)
                    {
                        parsedDocument.DebugInfo.ParsingExceptions.Add(e.Message);
                    }
                    finally
                    {
                        if (document != null)
                        {
                            document.Close();
                        }
                    }
                }
                finally
                {
                    app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                    app.Quit();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            

            GC.Collect();

            return parsedDocument;
        }


        /// <summary>
        /// Parse the general info (everything not regarding the data for a singular source) of a document
        /// </summary>
        /// <param name="document">The document to parse.</param>
        /// <param name="parsedDocument">The object in which the parsed data is accumulated.</param>
        /// <param name="currentParagraphCount">The current parsing position. The document is parsed by paragraphs.</param>
        /// <param name="totalParagraphCount">The total number of paragraphs in the document.</param>
        /// <returns></returns>
        private static void ParseGeneralInfo(ref Document document, ref ParsedDocument parsedDocument,
                                             ref int currentParagraphCount, int totalParagraphCount)
        {
            // Keep track of the headline we expect to encounter next
            // Accesses elements of ParsingInfo.GeneralInfoHeadlines
            // The bool is set to true if the current encountered headline matches our expectations based on the document guidelines.
            int currentHeadlineCount = -1;
            bool currentHeadlineMatchesExpectations = false;



            // Iterate over all paragraphs of the document.
            // This loop is exited when all general document info has been parsed, causing the function to exit.
            // Afterwards the individual sources can be parsed.
            for (int i = currentParagraphCount; i <= totalParagraphCount; i++)
            {

                Paragraph currentParagraph = document.Paragraphs[i];

                if (currentParagraph == null)
                {
                    throw new Exception("Error while parsing document ");
                }

                //string paragraphStyle = (string) currentParagraph.get_Style().NameLocal;
                string paragraphText = document.Content.Paragraphs[i].Range.Text;
                
                // Used to distinguish between headlines and text bodies
                int bold = currentParagraph.Range.Bold;

                // Current paragraph is a headline
                if (bold == -1)
                {
                    currentHeadlineCount++;
                    if (currentHeadlineCount >= ParsingInfo.GeneralInfoHeadlines.Count())
                    {
                        return;
                    }
                    currentHeadlineMatchesExpectations = false;

                    if (!paragraphText.StartsWith(ParsingInfo.GeneralInfoHeadlines[currentHeadlineCount]))
                    {
                        parsedDocument.DebugInfo.ParsingExceptions.Add("Unexpected start of headline encountered! Found headline: '" +
                                            paragraphText +
                                            "', expected start: '" +
                                            ParsingInfo.GeneralInfoHeadlines[currentHeadlineCount] +
                                            "'.");
                        
                    }

                    // The first headline actually contains content so we need to handle it here
                    else if (currentHeadlineCount == 0)
                    {
                        parsedDocument.IDString = paragraphText.Substring(paragraphText.LastIndexOf('[')).Trim();
                    }


                    else
                    {
                        currentHeadlineMatchesExpectations = true;
                    }
                }

                // Current paragraph is a text body
                else if (bold == 0)
                {
                    if (!currentHeadlineMatchesExpectations)
                    {
                        continue;
                    }

                    // Add the content of the current paragraph to the correct property of the parsedDocument
                    switch (currentHeadlineCount)
                    {
                        case 1: // "Werktitel:"
                            parsedDocument.Title = AppendParagraph(parsedDocument.Title, paragraphText);
                            break;

                        case 2: // "Werktitel – alternative Schreibweisen:"
                            parsedDocument.AlternativeTitles = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 3: // "Verfasser:"
                            parsedDocument.AuthorNames = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 4: // "Lebensdaten des Verfassers:"
                            parsedDocument.AuthorLifespan = AppendParagraph(parsedDocument.AuthorLifespan, paragraphText);
                            break;

                        case 5: // "Abfassungszeitraum:"
                            parsedDocument.TimePeriod = AppendParagraph(parsedDocument.TimePeriod, paragraphText);
                            break;

                        case 6: // "Abfassungsort:"
                            parsedDocument.Location = AppendParagraph(parsedDocument.Location, paragraphText);
                            break;

                        case 7: // "Region:"
                            parsedDocument.Regions = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 8: // "Editionshinweise:"
                            parsedDocument.EditionInfo = AppendParagraph(parsedDocument.EditionInfo, paragraphText);
                            break;

                        case 9: // "Allgemeines:"
                            parsedDocument.GeneralInfo = AppendParagraph(parsedDocument.GeneralInfo, paragraphText);
                            break;

                        default:
                            throw new Exception("Error: Trying to access unknown headline case. If you read this the writer of this software probably made an off-by-one error.");
                            break;
                    }
                }

                // Current paragraph type could not be recognized
                else
                {
                    parsedDocument.DebugInfo.ParsingExceptions.Add("Unexpected paragraph type. Content: '" + paragraphText + "'.");
                }

                currentParagraphCount++;
            }
        }

        /// <summary>
        /// Parses a single source of a document.
        /// </summary>
        /// <param name="document">The document to parse.</param>
        /// <param name="parsedDocument">The object in which the parsed data is accumulated.</param>
        /// <param name="currentParagraphCount">The current parsing position. The document is parsed by paragraphs.</param>
        /// <param name="totalParagraphCount">The total number of paragraphs in the document.</param>
        /// <returns></returns>
        private static void ParseSingleSource(ref Document document, ref ParsedDocument parsedDocument,
                                             ref int currentParagraphCount, int totalParagraphCount)
        {
            // Keep track of the headline we expect to encounter next
            // Accesses elements of ParsingInfo.GeneralInfoHeadlines
            // The bool is set to true if the current encountered headline matches our expectations based on the document guidelines.
            int currentHeadlineCount = -1;
            bool currentHeadlineMatchesExpectations = false;

            Source source = new Source();
            parsedDocument.Sources.Add(source);

            // Iterate over all paragraphs of the document.
            // This loop is exited when all general document info has been parsed, causing the function to exit.
            // Afterwards the individual sources can be parsed.
            for (int i = currentParagraphCount; i <= totalParagraphCount; i++)
            {

                Paragraph currentParagraph = document.Paragraphs[i];

                if (currentParagraph == null)
                {
                    throw new Exception("Error while parsing document ");
                }

                //string paragraphStyle = (string)currentParagraph.get_Style().NameLocal;
                string paragraphText = document.Content.Paragraphs[i].Range.Text;

                if(String.IsNullOrEmpty(paragraphText.Trim()))
                {
                    currentParagraphCount++;
                    continue;
                }

                // Used to distinguish between headlines and text bodies
                int bold = currentParagraph.Range.Bold;
                WdUnderline underline = currentParagraph.Range.Underline;

                if(!String.IsNullOrEmpty(source.IDString))
                {
                    if(underline != WdUnderline.wdUnderlineNone && bold == -1)
                    {
                        return;
                    }
                }

                // Current paragraph is a headline
                if (bold == -1)
                {
                    currentHeadlineCount++;
                    //if (currentHeadlineCount >= ParsingInfo.SourceHeadlines.Count())
                    //{
                    //    if (currentParagraph.Range.Underline != WdUnderline.wdUnderlineNone)
                    //    {
                    //        currentParagraphCount++;
                    //    }
                    //    return;
                    //}
                    currentHeadlineMatchesExpectations = false;

                    if (!paragraphText.Trim().StartsWith(ParsingInfo.SourceHeadlines[currentHeadlineCount]))
                    {
                        parsedDocument.DebugInfo.ParsingExceptions.Add("Unexpected start of headline encountered! Found headline: '" +
                                            paragraphText +
                                            "', expected start: '" +
                                            ParsingInfo.SourceHeadlines[currentHeadlineCount] +
                                            "'. Paragraph " + currentParagraphCount);

                        // Try to recover by peeking inside the next paragraph
                        if (document.Paragraphs[currentParagraphCount + 1].Range.Text.StartsWith(ParsingInfo.SourceHeadlines[currentHeadlineCount]))
                        {
                            currentParagraphCount++;
                            currentHeadlineCount--;
                            parsedDocument.DebugInfo.ParsingExceptions.RemoveAt(parsedDocument.DebugInfo.ParsingExceptions.Count-1);
                            continue;
                        }
                    }

                    // The first headline actually contains content so we need to handle it here
                    else if (currentHeadlineCount == 0)
                    {
                        source.IDString = paragraphText.Substring(paragraphText.LastIndexOf('[')).Trim();
                    }


                    else
                    {
                        currentHeadlineMatchesExpectations = true;
                    }
                }

                // Current paragraph is a text body
                else if (bold == 0)
                {
                    if (!currentHeadlineMatchesExpectations)
                    {
                        continue;
                    }

                    // Add the content of the current paragraph to the correct property of the parsedDocument
                    switch (currentHeadlineCount)
                    {
                        case 1: // "Zitation:"
                            source.Citation = AppendParagraph(source.Citation, paragraphText);
                            break;

                        case 2: // "zeitliche (Quellen-)Angabe:"
                            source.SourceTime = AppendParagraph(source.SourceTime, paragraphText);
                            break;

                        case 3: // "Inhaltsangabe:"
                            source.Summary = AppendParagraph(source.Summary, paragraphText);
                            break;

                        case 4: // "Volltext:"
                            source.TextOriginal = AppendParagraph(source.TextOriginal, paragraphText);
                            break;

                        case 5: // "Übersetzung:"
                            source.TextTranslated = AppendParagraph(source.TextTranslated, paragraphText);
                            break;

                        case 6: // "Hinweise zur Übersetzung (Zitation):"
                            source.TranslationInfo = AppendParagraph(source.TranslationInfo, paragraphText);
                            break;

                        case 7: // "zeitliche (wissenschaftliche) Einordnung:"
                            source.EstimatedActualTime = AppendParagraph(source.EstimatedActualTime, paragraphText);
                            break;

                        case 8: // "geographisches Stichwort:"
                            source.GeographicKeywords = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 9: // "Bericht über ein/mehrere Individuum/en oder Kollektive:"
                            source.ParticipantKeywords = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 10: // "Interaktion (j/n):"
                            source.Interaction = AppendParagraph(source.Interaction, paragraphText);
                            break;

                        case 11: // "Auffälligkeiten:"
                            source.DistinctiveFeatures = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 12: // "Suchbegriffe der Stelle (mit Semikolon trennen):"
                            source.SearchKeywords = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 13: // "Anmerkungen:"
                            source.Notes = AppendParagraph(source.Notes, paragraphText);
                            break;

                        default:
                            throw new Exception("Error: Trying to access unknown headline case. If you read this the writer of this software possibly made an off-by-one error.");
                            break;
                    }
                }

                // Current paragraph type could not be recognized
                else
                {
                    parsedDocument.DebugInfo.ParsingExceptions.Add("Unexpected paragraph type. Content: '" + paragraphText + "'.");
                }

                currentParagraphCount++;
            }
        }

        private static List<string> GetStringListFromSeperatedString(string source, char seperator)
        {
            List<string> result = new List<string>();

            int start = 0;

            // Add all elements as individual strings
            while(source.Substring(start).Contains(seperator))
            {
                int end = source.Substring(start).IndexOf(seperator);
                result.Add(source.Substring(start, end).Trim());
                start += end + 1;
            }

            // The last alternative title is not followed by the seperator so we need to handle it here
            result.Add(source.Substring(start).Trim());


            return result;
        }

        /// <summary>
        /// Append a string to a string with a newline character inbetween
        /// </summary>
        /// <param name="original">The string to be extended</param>
        /// <param name="toAppend">The string to append</param>
        /// <returns></returns>
        private static string AppendParagraph(string original, string toAppend)
        {
            if(string.IsNullOrWhiteSpace(toAppend))
                return original.Trim();

            if (String.IsNullOrEmpty(original))
                return toAppend.Trim();

            return (original + "\n" + toAppend).Trim();
        }

    }

}
