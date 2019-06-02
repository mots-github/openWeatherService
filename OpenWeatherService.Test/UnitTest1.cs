using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenWeatherService.Controllers;
using System.Web.Http;
using OpenWeatherService.Models;

namespace OpenWeatherService.Test
{
    [TestClass]
    public class UnitTest1
    {
        
        [TestMethod]
        public void TestMethod1()
        {
            OpenWeatherModel openWeatherObj = new OpenWeatherModel();
            WeatherController controller = new WeatherController();
            openWeatherObj.cities.Add("City of London", "2643741");
            var result = controller.GetWeatherInfo(openWeatherObj);

        }
    }
}
