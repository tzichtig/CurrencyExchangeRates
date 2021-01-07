using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Test.WebApi.ExchangeRates.Models;

namespace Test.WebApi.ExchangeRates.Controllers
{
    public class ExchangeRatesController : ApiController
    {
        private  readonly string _filePath = ConfigurationManager.AppSettings["FilePath"];
        // GET api/values
        public HttpResponseMessage Get()
        {

            var res = new List<ExchangeRate>();

            try
            {
                using (StreamReader r = new StreamReader(_filePath))
                {
                    string json = r.ReadToEnd();
                    res = JsonConvert.DeserializeObject<List<ExchangeRate>>(json);
                }
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error while get data");
            }
        }
        
       
    }
}
