using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Losowy
{
    internal class Program
    {
        static string[] names = { "Ania", "Kasia", "Basia", "Zosia" };
        static string[] surnames = { "Kowalska", "Nowak" };
        static void Main(string[] args)
        {
            var rand = new Random();
            List<Person> people = new List<Person>();
            for(int i = 1; i <=100; i++)
            {
                int nameNumber = rand.Next(names.Length);
                int surnameNumber = rand.Next(surnames.Length);
                int birthYear = rand.Next(1990, 2001);

                Person person = new Person(i, names[nameNumber], surnames[surnameNumber], birthYear);
                people.Add(person);
            }
            string date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
            string filepath = "C:/test/users-" + date + ".txt";
            ExportToCSV(people, filepath);
            Console.ReadKey();
        }
        public static void ExportToCSV(List<Person> people, string filepath)
        {
            using(StreamWriter file = new StreamWriter(filepath, true))
            {
                file.WriteLine("Lp,Imię,Nazwisko,Rok_Urodzenia");
                foreach (Person person in people)
                {
                    file.WriteLine($"{person.Id},{person.Name},{person.Surname},{person.BirthYear}");
                }
            }
        }
        
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int BirthYear { get; set; }

            public Person(int id, string name, string surname, int birthYear)
            {
                Id = id;
                Name = name;
                Surname = surname;
                BirthYear = birthYear;
            }
        }
    }
}
