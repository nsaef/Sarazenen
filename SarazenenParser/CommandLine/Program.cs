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
            // TODO: Handle single document and folder batch processing
            // TODO: Move code below to method that handles a single document
            // TODO: Filepath given by args

            List<ParsedDocument> parsedDocuments;

            if (File.GetAttributes(args[0]).HasFlag(FileAttributes.Directory))
            {
                parsedDocuments = ParseDocumentsInFolder(args[0], "*.docx");
            }
            else
            {
                parsedDocuments = new List<ParsedDocument>();
                parsedDocuments.Add(ParseDocument(args[0]));
            }
            
            //// List of parsed documents where some sort of parsing exception occured
            List<ParsedDocument> problematicDocs = parsedDocuments.Where(x => x.ParsingExceptions.Count > 0).ToList();

            // add the parsed documents to an object that acts as root element
            DocumentCollection collection = new DocumentCollection();
            collection.Documents = parsedDocuments;

            // serialize to XML
            XmlWriter xml = new XmlWriter();
            xml.writeXml(collection);

            // ParsedDocument document = Parser.ParseDocument(@"E:\Projekte\Sarazenenprojekt (Bonn)\Quellen\0001-Thietmar von Merseburg_Chronicon sive Gesta Saxonum_bearb..docx");
            // TODO: post-processing, e.g. manually replace custom markup (locations, names, etc) with TEI-Elements in the strings of the document object
            // TODO: write TEI-XML file
        }

        /// <summary>
        /// Parse single document. Wrapped here for IO stuff (write to console etc).
        /// </summary>
        /// <param name="pathToDocx">The path to the file to be parsed.</param>
        /// <returns></returns>
        static ParsedDocument ParseDocument(string pathToDocx)
        {
            Console.WriteLine("Parsing file: " + pathToDocx);
            return Parser.ParseDocument(pathToDocx);
        }

        /// <summary>
        /// Parse multiple documents.
        /// </summary>
        /// <param name="directoryPath">Path to the directory that contains the docs. Subdirectories are searched as well.</param>
        /// <param name="fileNameFilter">FilePattern to be searched, e.g. "*.docx"</param>
        /// <returns></returns>
        static List<ParsedDocument> ParseDocumentsInFolder(string directoryPath, string fileNameFilter)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(directoryPath);
            List<ParsedDocument> parsedDocuments = new List<ParsedDocument>();
            foreach (FileInfo file in sourceDirectory.GetFiles(fileNameFilter, SearchOption.AllDirectories).Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)))
            {
                parsedDocuments.Add(ParseDocument(file.FullName));
            }

            return parsedDocuments;
        }
    }
}
