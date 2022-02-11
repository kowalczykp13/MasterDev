using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zamiana
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Podaj ścieżkę do pliku");
            string path = Console.ReadLine();
            StreamWriter newFile;
            if (!File.Exists(path))
            {
                Console.WriteLine("Brak pliku");
            }
            else
            {
                DateTime today = DateTime.Now;
                String date = today.ToString("dd MM yyyy").Replace(" ", "");
                Console.WriteLine(date);
                var newPath = path.Replace(".txt", "")+ "_changed-"+date+".txt";
                
                newFile = File.CreateText(newPath);
                string text = File.ReadAllText(path);
                text = text.Replace("praca", "job");
                newFile.WriteLine(text);
                newFile.Close();
                Console.WriteLine("Utworzono nowy plik");
            }
            Console.ReadKey();
        }
    }
}
