using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace SampleCallRestFullAPI
{
    public class Restfull_Call_POST
    {
        private static Logger _loggerError = LogManager.GetCurrentClassLogger();
        public static string _confEndpoint;
        public static string _confResource;

        public Restfull_Call_POST(string confEndpoint, string confResource)
        {
            _confEndpoint = confEndpoint;
            _confResource = confResource;
        }

        public void StartProcess(string cifRq, string phoneNumberRq)
        {
            var clientId = Guid.NewGuid().ToString();

            try
            {
                var restClient = new RestClient(_confEndpoint);
                var restRequest = new RestRequest(_confResource, Method.GET);
                restRequest.AddHeader("x-request-id", clientId);
                restRequest.AddHeader("Content-Type", "application/json");

                restRequest.AddParameter("cif", cifRq);
                restRequest.AddParameter("phone-number", phoneNumberRq);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                IRestResponse irestResponse = restClient.Execute(restRequest);

                var content = irestResponse.Content;
                var statusCode = irestResponse.StatusCode.ToString();

                if (!string.IsNullOrEmpty(content))
                {
                    var jobject = JObject.Parse(content);

                    if (jobject != null && jobject["notiSearchList"] != null && jobject["notiSearchList"].ToString().Length > 0)
                    {
                        var arrNotiList = JArray.Parse(jobject["notiSearchList"].ToString());

                        foreach (var item in arrNotiList.Children())
                        {
                            var aggregateId = item.Children<JProperty>().FirstOrDefault(x => x.Name == "aggregateId").Value.ToString();
                            var cif = item.Children<JProperty>().FirstOrDefault(x => x.Name == "cif").Value.ToString();
                            var accountNumber = item.Children<JProperty>().FirstOrDefault(x => x.Name == "accountNumber").Value.ToString();
                            var mobileNumber = item.Children<JProperty>().FirstOrDefault(x => x.Name == "mobileNumber").Value.ToString();
                            var methodCode = item.Children<JProperty>().FirstOrDefault(x => x.Name == "methodCode").Value.ToString();

                            Console.WriteLine($"aggregateId: {aggregateId} - cif: {cif} - accountNumber: {accountNumber} - mobileNumber: {mobileNumber} - methodCode: {methodCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerError.Error("SearchRegisterAppByService - MSA - history/register - session-id: " + clientId + " - error: " + ex.StackTrace);
            }
        }
    }
}