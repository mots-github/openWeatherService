using System;
using System.Net.Http;
using System.Net.Http.Headers;


namespace OpenWeatherService.Common
{
    public class MyStreamProvider : MultipartFormDataStreamProvider
    {
        public MyStreamProvider(string uploadPath)
            : base(uploadPath)
        {

        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = headers.ContentDisposition.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".csv";
            }
            return fileName.Replace("\"", string.Empty);
        }
    }
}