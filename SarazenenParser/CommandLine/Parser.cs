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

                        // The parsing is split into subfunctions to increase code readability
                        ParseGeneralInfo(ref document, ref parsedDocument, ref currentParagraphCount, totalParagraphCount);
                        // TODO: Add parsing of individual sources



                        // TODO: Close app?

                    }
                    catch (Exception e)
                    {
                        parsedDocument.ParsingExceptions.Add(e.Message);
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

                string paragraphStyle = (string) currentParagraph.get_Style().NameLocal;
                string paragraphText = document.Content.Paragraphs[i].Range.Text;



                // paragraphStyle is used to distinguish between headlines (categories) and text bodies (content)

                // Current paragraph is a headline
                if (ParsingInfo.HeadlineParagraphStyles.Any(x => x == paragraphStyle))
                {
                    currentHeadlineCount++;
                    if (currentHeadlineCount >= ParsingInfo.GeneralInfoHeadlines.Count())
                    {
                        return;
                    }
                    currentHeadlineMatchesExpectations = false;

                    // TODO: Recoverable error handling via currentHeadlineMatchesExpectations(?)
                    if (!paragraphText.StartsWith(ParsingInfo.GeneralInfoHeadlines[currentHeadlineCount]))
                    {
                        parsedDocument.ParsingExceptions.Add("Unexpected start of headline encountered! Found headline: '" +
                                            paragraphText +
                                            "', expected start: '" +
                                            ParsingInfo.GeneralInfoHeadlines[currentHeadlineCount] +
                                            "'.");
                        
                    }

                    // The first headline actually contains content so we need to handle it here
                    else if (currentHeadlineCount == 0)
                    {
                        parsedDocument.IDstring = paragraphText.Substring(paragraphText.LastIndexOf('[')).Trim();
                    }


                    else
                    {
                        currentHeadlineMatchesExpectations = true;
                    }
                }

                // Current paragraph is a text body
                else if (ParsingInfo.TextBodyParagraphStyles.Any(x => x == paragraphStyle))
                {
                    if (!currentHeadlineMatchesExpectations)
                    {
                        continue;
                    }

                    // Add the content of the current paragraph to the correct property of the parsedDocument
                    switch (currentHeadlineCount)
                    {
                        case 1: // "Werktitel:"
                            parsedDocument.Title = AppendConditionally(parsedDocument.Title, paragraphText);
                            break;

                        case 2: // "Werktitel – alternative Schreibweisen:"
                            parsedDocument.AlternativeTitles = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 3: // "Verfasser:"
                            parsedDocument.AuthorNames = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 4: // "Lebensdaten des Verfassers:"
                            parsedDocument.AuthorLifespan = AppendConditionally(parsedDocument.AuthorLifespan, paragraphText);
                            break;

                        case 5: // "Abfassungszeitraum:"
                            parsedDocument.TimePeriod = AppendConditionally(parsedDocument.TimePeriod, paragraphText);
                            break;

                        case 6: // "Abfassungsort:"
                            parsedDocument.Location = AppendConditionally(parsedDocument.Location, paragraphText);
                            break;

                        case 7: // "Region:"
                            parsedDocument.Regions = GetStringListFromSeperatedString(paragraphText, ';');
                            break;

                        case 8: // "Editionshinweise:"
                            parsedDocument.EditionInfo = AppendConditionally(parsedDocument.EditionInfo, paragraphText);
                            break;

                        case 9: // "Allgemeines:"
                            parsedDocument.GeneralInfo = AppendConditionally(parsedDocument.GeneralInfo, paragraphText);
                            break;

                        default:
                            throw new Exception("Error: Trying to access unknown headline case. If you read this the writer of this software probably made an off-by-one error.");
                            break;
                    }
                }

                // Current paragraph type could not be recognized
                else
                {
                    throw new Exception("Unexpected paragraph style: '" + paragraphStyle + "'.");
                }
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
        private static string AppendConditionally(string original, string toAppend)
        {
            if(string.IsNullOrWhiteSpace(toAppend))
                return original.Trim();

            if (String.IsNullOrEmpty(original))
                return toAppend.Trim();

            return (original + "\n" + toAppend).Trim();
        }

    }

}
