using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace NBP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double valuePLN = 0;
            var client = new HttpClient();
            var response = client.GetAsync("http://api.nbp.pl/api/exchangerates/rates/c/usd/");
            response.Wait();
            if(response.IsCompleted)
            {
                var result = response.Result;
                if(result.IsSuccessStatusCode)
                {
                    var messageTask = result.Content.ReadAsStringAsync();
                    messageTask.Wait();
                    var message = messageTask.Result;
                    JObject json = JObject.Parse(message);

                    double valueUSD = Convert.ToDouble(json["rates"][0]["bid"]);
                    Console.WriteLine("Aktualna wartość USD to: " + valueUSD + "zł");
                    

                    bool isGoodPrice = false;
                    do
                    {
                        try
                        {
                            Console.WriteLine("Podaj ilość PLN, które chcesz zamienić na USD: ");
                            var value = Console.ReadLine().Replace(".", ",");
                            valuePLN = Convert.ToDouble(value);
                            isGoodPrice = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Podałeś złą wartość");
                            isGoodPrice = false;
                        }

                    } while (!isGoodPrice);

                    double finalValue = Math.Round(valuePLN / valueUSD, 2);
                    Console.WriteLine($"{valuePLN} PLN jest równe {finalValue} USD");

                }
            }
            Console.ReadKey();

        }
    }
}
