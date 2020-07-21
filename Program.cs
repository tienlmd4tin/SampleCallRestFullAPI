using System;

namespace SampleCallRestFullAPI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Test call GET
            var callGet = new Restfull_Call_GET("https://192.168.1.129:443/refund/v1", "refund/refund");
            callGet.StartProcess("2020078127323", "FT2981924242412424", "Hoan tra GD...");

            //Test call POST
            var callPost = new Restfull_Call_POST("https://192.168.1.129:443/iapi/notifications/v1", "/history/registered");
            callPost.StartProcess("123", "0859837776");

            Console.ReadLine();
        }
    }
}