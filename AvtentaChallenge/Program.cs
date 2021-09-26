using DemoWS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace AvtentaChallenge
{
    class MainClass
    {
        // Source URL for calling client
        public static string SourceURL = "http://195.88.82.191/DemoWS/Services/SourceWs.svc";
        // Destination URL for submiting results
        public static string DestinationURL = "http://195.88.82.191/DemoWS/Services/DestinationWs.svc";
        public static void Main(string[] args)
        {
            // Initialize xml serializer with object type Akcija
            var serializer = new XmlSerializer(typeof(List<Akcija>));

            Console.WriteLine($"Input source URL (default URL: {SourceURL}), leave blank if you want to use default URL:");
            var clintSourceURL = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Getting data from source....");
            TextReader reader = new StringReader(GetDataFromSource(clintSourceURL));
            List<Akcija> result = (List<Akcija>)serializer.Deserialize(reader);
            Console.WriteLine("Data from source read successfully");

            Console.Clear();            
            Console.WriteLine($"Input destination URL (default URL: {DestinationURL}), leave blank if you want to use default URL:");
            var clintDestinationURL = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Submitting data to destination...");
            SumbitDataToDestination(clintDestinationURL, result);
            Console.WriteLine("Data successfully submitted");

            Console.ReadLine();
        }
        private static string GetDataFromSource(string clintSourceURL)
        {
            try
            {
                SourceWsClient client = new SourceWsClient("BasicHttpBinding_ISourceWs", string.IsNullOrEmpty(clintSourceURL) ? SourceURL : clintSourceURL);
                client.Open();
                var response = client.GetActions();
                client.Close();
                return response;
            }
            catch(Exception ex)
            {
                ExcetionHandling(ex.Message, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
        private static void SumbitDataToDestination(string clintDestinationURL, List<Akcija> result)
        {
            try
            {
                DestinationWsClient destination = new DestinationWsClient("BasicHttpBinding_IDestinationWs", string.IsNullOrEmpty(clintDestinationURL) ? DestinationURL : clintDestinationURL);
                destination.Open();
                var response = destination.SubmitActions(result.ToArray(), "nabergoj.miha@gmail.com");
                if (response != "ok")
                    throw new Exception($"Response from submit: {response}");
                destination.Close();
            }
            catch (Exception ex)
            {
                ExcetionHandling(ex.Message, MethodBase.GetCurrentMethod().Name);
            }
        }
        private static void ExcetionHandling(string ExcetionMessage, string MethodName)
        {
            Console.Clear();
            Console.WriteLine($"Error in method GetDataFromSource \n Message: {ExcetionMessage}");
            Console.WriteLine("Press any key to continue....");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
