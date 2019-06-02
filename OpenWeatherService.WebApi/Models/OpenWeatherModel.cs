using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenWeatherService.Models
{
    public class OpenWeatherModel
    {
       // public string apiResponse { get; set; }
        public Dictionary<string, string> cities
        {
            get; set;
        }
    }
}