using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CommandLine
{
    /// <summary>
    /// The command line program that calls the parser and handles file input/output
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            DocumentCollection collection = new DocumentCollection();
            CsvParser personParser = new CsvParser(@"C:\Users\Amin\Documents\GitHub\sarazenen\Personenregister komplett.csv");
            collection.Persons = personParser.createPersonRegister();

            CsvParser placeParser = new CsvParser(@"C:\Users\Amin\Documents\GitHub\sarazenen\Ortsregister (1-50)_inklusive_Kloster.csv");
            collection.Places = placeParser.createPlaceRegister();

            List<ParsedDocument> parsedDocuments;

            // The first argument is the path. Depending on its properties all docx-files in a directory or a single file will be parsed.
            // Even in the case of a single document the result will be a List of parsed documents (in that case with a single entry).
            if (File.GetAttributes(args[0]).HasFlag(FileAttributes.Directory))
            {
                parsedDocuments = ParseDocumentsInFolder(args[0], "*.docx", collection);
            }
            else
            {
                parsedDocuments = new List<ParsedDocument>();
                parsedDocuments.Add(ParseDocument(args[0], collection));
            }

            // add the parsed documents to an object that acts as root 
            collection.Documents = parsedDocuments;

            // serialize to XML
            XmlWriter xml = new XmlWriter();
            xml.writeXml(collection);

            // ParsedDocument document = Parser.ParseDocument(@"E:\Projekte\Sarazenenprojekt (Bonn)\Quellen\0001-Thietmar von Merseburg_Chronicon sive Gesta Saxonum_bearb..docx");

            // The code below collects some analysis and debugging info. ********************************************** //
            List<string> filePaths = parsedDocuments.Select(x => x.DebugInfo.FilePath).ToList();

                
                
            List<ParsedDocument> problematicDocs =
                parsedDocuments.Where(x => x.DebugInfo.ParsingExceptions.Count > 0).ToList();

               
                
            List<string> nullOrEmptyProperties =
                parsedDocuments.Select(x => x.ReportNullOrEmptyProperties()).ToList();

            string completeNullOrEmptyProperties = nullOrEmptyProperties.Aggregate("\n",
                                                                                    (current, nullOrEmptyProperty) =>
                                                                                    current + nullOrEmptyProperty);


            List<ParsedDocument> documentsWhereTheEndMightHaveBeenTruncated =
                parsedDocuments.Where(x => (x.DebugInfo.LastParagraphReached - x.DebugInfo.ParagraphCount < 0)).
                    ToList();
            // ******************************************************************************************************** //

            // TODO: post-processing, e.g. manually replace custom markup (locations, names, etc) with TEI-Elements in the strings of the document object
            // TODO: write TEI-XML file
        
        }

        /// <summary>
        /// Parse single document. Wrapped here for IO stuff (write to console etc).
        /// </summary>
        /// <param name="pathToDocx">The path to the file to be parsed.</param>
        /// <returns></returns>
        static ParsedDocument ParseDocument(string pathToDocx, DocumentCollection parent)
        {
            Console.WriteLine("Parsing file: " + pathToDocx);
            return Parser.ParseDocument(pathToDocx, parent);
        }

        /// <summary>
        /// Parse multiple documents.
        /// </summary>
        /// <param name="directoryPath">Path to the directory that contains the docs. Subdirectories are searched as well.</param>
        /// <param name="fileNameFilter">FilePattern to be searched, e.g. "*.docx"</param>
        /// <returns></returns>
        static List<ParsedDocument> ParseDocumentsInFolder(string directoryPath, string fileNameFilter, DocumentCollection parent)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(directoryPath);
            List<ParsedDocument> parsedDocuments = new List<ParsedDocument>();
            foreach (FileInfo file in sourceDirectory.GetFiles(fileNameFilter, SearchOption.AllDirectories).Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)))
            {
                parsedDocuments.Add(ParseDocument(file.FullName, parent));
            }

            return parsedDocuments;
        }
    }
}
