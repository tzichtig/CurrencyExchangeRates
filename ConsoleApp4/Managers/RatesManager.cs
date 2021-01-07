using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Currency.service.Managers
{
    public static class RatesManager
    {
        //class and attributes in HTML from bloomberd site
        private const string class_to_get_name = "full";
        private const string class_to_get_value = "value";
        private const string class_to_get_time = "time";


        //HTML Pages urls
        private const string html = @"https://www.bloomberg.com/markets/currencies";
        private const string html1 = @"https://www.bloomberg.com/markets/currencies/americas";
        private const string html2 = @"https://www.bloomberg.com/markets/currencies/europe-africa-middle-east";

        //PATH to data base file
        private static readonly string _filePath = ConfigurationManager.AppSettings["FilePath"]; //@"C:\Users\1\source\repos\database.json";
        private static readonly string interval = ConfigurationManager.AppSettings["Interval"]; //@"C:\Users\1\source\repos\database.json";

        private static List<string> myFavorite = new List<string>() { " USD-ILS ", " GBP-EUR ", " EUR-JPY ", " EUR-USD " };

        public static void StartDownloadRates()
        {
            while (true)
            {
                Console.WriteLine($"Start get data from blumberg site, time: {DateTime.Now}");
                var rates = GetValuesFromSites();
                if (rates != null)
                {
                    WriteToFile(rates);
                }
                Console.WriteLine($"Finish get data from blumberg site, time: {DateTime.Now}");
                //sleep 
                Thread.Sleep(int.Parse(interval));
            }

        }


        private static List<ExchangeRates> GetValuesFromSites()
        {
            try
            {
                var rates = new List<ExchangeRates>();

                var tasks = new List<Task<List<ExchangeRates>>>();
                tasks.Add(Task.Factory.StartNew(() => LoadRates(html)));
                tasks.Add(Task.Factory.StartNew(() => LoadRates(html1)));
                tasks.Add(Task.Factory.StartNew(() => LoadRates(html2)));

                Task.WaitAll(tasks.ToArray());
                foreach (var task in tasks)
                {
                    rates.AddRange(task.Result);
                }

                //filter 
                var res = rates.Where(r => myFavorite.Contains(r.Name)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetch data over http, exeption: {ex.Message}, Stack: {ex.StackTrace}");
                return null;
            }
        }

        private static List<ExchangeRates> LoadRates(string html)
        {
            List<ExchangeRates> rates = new List<ExchangeRates>();
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//tbody//tr").ToList();

            int index = 0;
            foreach (var tableRow in nodes)
            {
                try
                {
                    var name = tableRow.SelectNodes("..//th//a//div[@data-type='" + class_to_get_name + "']").ToArray()[index].InnerHtml;
                    var value = tableRow.SelectNodes("..//td[@data-type='" + class_to_get_value + "']//span").ToArray()[index].InnerHtml;
                    var lastDateUpdate = tableRow.SelectNodes("..//td[@data-type='" + class_to_get_time + "']//span").ToArray()[index].InnerHtml;
                    var _exchange = new ExchangeRates() { Name = name, Value = Convert.ToDouble(value), LastUpdate = lastDateUpdate };
                    rates.Add(_exchange);
                    index++;
                }
                catch (Exception ex)
                {
                    index++;
                    continue;

                }
            }
            return rates;
        }

        private static void WriteToFile(List<ExchangeRates> rates)
        {
            try
            {
                string json = JsonConvert.SerializeObject(rates);

                //write string to file
                System.IO.File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error while write to file ,{ex.StackTrace}");
            }
        }
    }
}
