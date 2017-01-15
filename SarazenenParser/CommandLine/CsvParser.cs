using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine {
    /// <summary>
    /// parser for register files (places, people)
    /// </summary>
    public class CsvParser {

        public CsvParser(string location) {
            path = location;
        }

        public string path { get; set; }
        public List<CsvRow> rows { get; set; }
        public string header { get; set; }


        public List<Person> createPersonRegister() {
            List<Person> register = new List<Person>();

            List<string> rows = this.readCsv();

            //determine order of elements
            var index = new Dictionary<int, string>();
            bool first = true;

            foreach (var row in rows) {
                String[] fields = row.Split('\t');
                Person person = new Person();

                if (first == true) {
                    //create the index
                    for (int i = 0; i < fields.Length; ++i) {
                        index[i] = fields[i];
                    }
                    first = false;
                    continue;
                }

                //read the field data and add it to the csv object.
                //iterate over fields
                for (int i = 0; i < fields.Length; ++i) {
                    if (index[i] == "vereinheitlichte Namen") {
                        person.Name = fields[i];
                    }
                    else if (index[i] == "(Zusatz-) Informationen") {
                        person.Role = fields[i];
                    }
                    else if (index[i] == "Alternativbezeichnungen" || index[i] == "" && i > 2) {
                        if (fields[i] != "") {
                            person.AltNames.Add(fields[i]);
                        }
                        
                    }
                }

                //push line into csv vector
                if (first == false) {
                    register.Add(person);
                }
                first = false;
            }

            return register;
        }

        public List<Place> createPlaceRegister() {
            List<Place> register = new List<Place>();

            List<string> rows = this.readCsv();

            //determine order of elements
            var index = new Dictionary<int, string>();
            bool first = true;

            foreach (var row in rows) {
                String[] fields = row.Split('\t');
                Place place = new Place();

                if (first == true) {
                    //create the index
                    for (int i = 0; i < fields.Length; ++i) {
                        index[i] = fields[i];
                    }
                    first = false;
                    continue;
                }

                //read the field data and add it to the csv object.
                //iterate over fields
                for (int i = 0; i < fields.Length; ++i) {
                    if (index[i] == "Orte/Regionen") {
                        place.Name = fields[i];
                    }
                    else if (index[i] == "Klassifizierung") {
                        place.Type = fields[i];
                    }
                    else if (index[i] == "Alternativbezeichnungen" || index[i] == "" && i > 2) {
                        if (fields[i] != "") {
                            place.AltNames.Add(fields[i]);
                        }

                    }
                }

                //push line into csv vector
                if (first == false) {
                    register.Add(place);
                }
                first = false;
            }

            return register;
        }


        /// <summary>
        /// Read a csv file and create a person or place list
        /// </summary>
        /// <returns>Registry</returns>
        public List<string> readCsv() {
            var reader = new StreamReader(File.OpenRead(path));
            List<string> rows = new List<string>();

            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                rows.Add(line);
            }

            reader.Close();

            return rows;
        }

    }
}
