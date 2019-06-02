using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using OpenWeatherService.Models;
using OpenWeatherService.Common;
using System.Reflection;

namespace OpenWeatherService.Controllers
{
   
    public class WeatherController : ApiController
    {
        /// <summary>
        /// This method(POST) verifies and reads the uploaded file to populates the Model class
        /// </summary>
        /// <returns>string message</returns>
        [HttpPost]
        [Route("api/weather")]
        public async Task<string> PostAsync()
        {
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    string uploadPath = HttpContext.Current.Server.MapPath(Resource.UploadFolder);
                    MyStreamProvider streamProvider = new MyStreamProvider(uploadPath);
                    string message = string.Empty;
                    OpenWeatherModel openWeatherMdl = new OpenWeatherModel();
                    openWeatherMdl.cities = new Dictionary<string, string>();

                    await Request.Content.ReadAsMultipartAsync(streamProvider);
                    string uploadFilePath = streamProvider.GetLocalFileName(streamProvider.FileData[0].Headers);
                                                         

                    //Read the contents of CSV file.
                    string csvData = File.ReadAllText(uploadFilePath);

                    //Execute a loop over the file rows and populate cities in the object.
                    foreach (string row in csvData.Split('\n'))
                    {                        
                        if (!string.IsNullOrEmpty(row))
                        {                         
                            openWeatherMdl.cities.Add(row.Split(',')[0], row.Split(',')[1].Replace("\r", ""));                                                  
                        }
                    }

                    message = GetWeatherInfo(openWeatherMdl);
                    

                     return message;
                    }
                    else
                    {
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                        throw new HttpResponseException(response);
                    }
                }
            
            catch (Exception ex)
            {
                Logger.Publish(EventType.Exception, this.GetType().Name, MethodBase.GetCurrentMethod().Name.ToString(), ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.Message, ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }
        /// <summary>
        ///  This class gets the weather info from the Openweather API and saves it on the Output folder
        /// </summary>
        /// <param name="openWeatherObj"></param>
        /// <returns></returns>
       [HttpGet]
       [Route("api/weather/{openWeatherObj}")]
        public string GetWeatherInfo(OpenWeatherModel openWeatherObj)
        {

            try
            {
                if (openWeatherObj != null && openWeatherObj.cities != null)
                {
                    string downloadPath = HttpContext.Current.Server.MapPath(Resource.OutputFolder);
                    foreach (var row in openWeatherObj.cities)
                    {
                        /*Calling API http://openweathermap.org/api */
                        string apiKey = Resource.ApiKey;
                        string apiResponse = string.Empty;
                        string downloadFilePath = string.Concat(downloadPath, @"\", row.Key);

                        HttpWebRequest apiRequest =
                        WebRequest.Create(string.Format(Resource.ApiUrl, row.Value, apiKey)) as HttpWebRequest;

                        using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                        {
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            apiResponse = reader.ReadToEnd();
                        }
                        /*End*/


                        if (!string.IsNullOrEmpty(apiResponse))
                        {
                            Directory.CreateDirectory(downloadFilePath);
                            string fileName = string.Concat(downloadFilePath, @"\", DateTime.Now.ToString("MM-dd-yyyy"), ".json");
                            if (!File.Exists(fileName))
                            {
                                File.Create(fileName).Dispose(); ;
                                using (var tw = new StreamWriter(fileName, true))
                                {
                                    tw.WriteLine(apiResponse);
                                }
                            }
                            else if (File.Exists(fileName))
                            {
                                using (var tw = new StreamWriter(fileName, true))
                                {
                                    tw.WriteLine(apiResponse);
                                }

                            }
                        }
                    }
                    return "Files processed successfully, weather information is downloaded to 'Output' folder!";
                }
                else
                {
                    return "Unable to read city details from input file!";
                }
            }
            catch (Exception ex)
            {
                Logger.Publish(EventType.Exception, this.GetType().Name, MethodBase.GetCurrentMethod().Name.ToString(), ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.Message, ex.StackTrace);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
           
        }
    }  

}