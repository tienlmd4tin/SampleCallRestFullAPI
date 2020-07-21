using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System;
using System.Net;

namespace SampleCallRestFullAPI
{
    public class Restfull_Call_GET
    {
        private static Logger _loggerError = LogManager.GetCurrentClassLogger();
        public static string _confEndpoint;
        public static string _confResource;

        public Restfull_Call_GET(string confEndpoint, string confResource)
        {
            _confEndpoint = confEndpoint;
            _confResource = confResource;
        }

        public void StartProcess(string referenceNumber, string refundReferenceNumber, string remark)
        {
            var clientId = Guid.NewGuid().ToString();

            try
            {
                var restClient = new RestClient(_confEndpoint);
                var restRequest = new RestRequest(_confResource, Method.POST);
                restRequest.AddHeader("x-request-id", clientId);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("channel", "smartpay");

                restRequest.RequestFormat = ((DataFormat)0);
                restRequest.AddJsonBody((object)new
                {
                    ReferenceNumber = referenceNumber,
                    RefundReferenceNumber = refundReferenceNumber,
                    Remark = remark,
                    OperationType = string.Empty
                });

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                IRestResponse irestResponse = restClient.Execute(restRequest);

                var content = irestResponse.Content;
                var statusCode = irestResponse.StatusCode.ToString();

                if (!string.IsNullOrEmpty(content))
                {
                    var jobject = JObject.Parse(content);

                    var transactionId = jobject["transactionId"].ToString();
                    var status = jobject["status"].ToString();
                    var message = jobject["message"].ToString();
                    var error = jobject["error"].ToString();

                    Console.WriteLine($"transactionId: {transactionId} - status: {status} - error: {error} - message: {message}");
                }
            }
            catch (Exception ex)
            {
                _loggerError.Error("Refund Ewallet - MSA - refund/refund - session-id: " + clientId + " - error: " + ex.StackTrace);
            }
        }
    }
}