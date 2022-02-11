using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litera
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = @"c:/test/test_pio_kow.txt";
            string text = File.ReadAllText(path);
            int counter = 0;
            foreach(var c in text)
            {
                if (c == 'a')
                {
                    counter++;
                }
            }
            Console.WriteLine(counter);
            Console.ReadKey();
        }
    }
}
