using DemoWS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace AvtentaChallenge
{
    class MainClass
    {
        // Source URL for calling client read from app.config
        public static string SourceURL = ConfigurationManager.AppSettings["SourceURL"];
        // Destination URL for submiting results read from app.config
        public static string DestinationURL = ConfigurationManager.AppSettings["DestinationURL"];
        public static void Main(string[] args)
        {
            // Initializes xml serializer variable with object type Akcija
            var serializerXML = new XmlSerializer(typeof(List<Akcija>));

            /*
             Initializes TextReader variable with data from source.
             Initializes List with object Akcija and populates data from deserializing TextReader 
            */
            Console.Clear();
            Console.WriteLine("Getting data from source....");
            TextReader readerWithData = new StringReader(GetDataFromSource());
            List<Akcija> resultList = (List<Akcija>)serializerXML.Deserialize(readerWithData);
            Console.WriteLine("Data from source read successfully");

            //Submits List of data to destination  
            Console.Clear();
            Console.WriteLine("Submitting data to destination...");
            SubmitDataToDestination("nabergoj.miha@gmail.com", resultList);
            Console.WriteLine("Data successfully submitted");

            Console.ReadLine();
        }
        private static string GetDataFromSource()
        {
            try
            {
                /*
                 Initializes source client with provided URL
                 Opens client and calls SOAP web service to get data
                 Closes clent and returns a response string
                */

                SourceWsClient client = new SourceWsClient("BasicHttpBinding_ISourceWs", SourceURL);
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
        private static void SubmitDataToDestination(string Email, List<Akcija> data)
        {
            try
            {
                /*
                 Initializes destination client with provided URL
                 Opens destination and submits transformed List to Array of data to destination
                */

                DestinationWsClient destination = new DestinationWsClient("BasicHttpBinding_IDestinationWs", DestinationURL);
                destination.Open();
                var response = destination.SubmitActions(data.ToArray(), Email);
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
            /*
             When exception occures:
                Excetion message is displayed
                Program closes
            */
            
            Console.Clear();
            Console.WriteLine($"Error in method GetDataFromSource \n Message: {ExcetionMessage}");
            Console.WriteLine("Press any key to continue....");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
