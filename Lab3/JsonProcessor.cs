using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab3
{
    internal class JsonProcessor
    {
        public static void Serialize(string path, ObservableCollection <StudentsClub> results)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (FileStream fstream = new FileStream(path, FileMode.Create))
            {

                JsonSerializer.Serialize(fstream, results, options);
            }
        }

        public static ObservableCollection <StudentsClub> Deserialize(string path)
        {
            ObservableCollection <StudentsClub> results = new ObservableCollection<StudentsClub>();
            using (FileStream fstream = new FileStream(path, FileMode.Open))
            {
                var clubs = JsonSerializer.Deserialize<List<StudentsClub>>(fstream);

                foreach (StudentsClub club in clubs)
                {
                    results.Add(club);
                }

                return results;
            }
        }
    }
}
