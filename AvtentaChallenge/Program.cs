using DemoWS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace AvtentaChallenge
{
    class MainClass
    {
        public static string SourceURL = "http://195.88.82.191/DemoWS/Services/SourceWs.svc";
        public static string TargetURL = "http://195.88.82.191/DemoWS/Services/DestinationWs.svc";
        public static void Main(string[] args)
        {
            Akcija responseList = new Akcija();
            SourceWsClient client = new SourceWsClient("BasicHttpBinding_ISourceWs",SourceURL);
            client.Open();
            var response = client.GetActions();
            client.Close();
            var serializer = new XmlSerializer(typeof(List<Akcija>));
            TextReader reader = new StringReader(response);
            List<Akcija> result = (List<Akcija>)serializer.Deserialize(reader);
            DestinationWsClient destination = new DestinationWsClient("BasicHttpBinding_IDestinationWs", TargetURL);
            destination.Open();
            destination.SubmitActions(result.ToArray(), "nabergoj.miha@gmail.com");
            destination.Close();
        }
    }
}
