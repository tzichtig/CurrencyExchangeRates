using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Currency.service.Managers;

namespace Test.Currency.service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========Start interval to get data from blumberg site ============");
            try
            {
               RatesManager.StartDownloadRates();

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error while get dat {ex.StackTrace}");
            }        
        }
    }
}
