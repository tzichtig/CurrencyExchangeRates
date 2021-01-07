using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.WebApi.ExchangeRates.Models
{
    public class ExchangeRate
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string LastUpdate { get; set; }
    }
}