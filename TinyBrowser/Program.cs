using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace TinyBrowser
{
    class Program
    {

        private static TcpClient tcpClient;
        private static Stream stream;
        
        static void Main(string[] args)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect("acme.com", 80);
            tcpClient.SendTimeout = 3000;
            tcpClient.ReceiveTimeout = 3000;
            
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            StreamReader reader = new StreamReader(tcpClient.GetStream());

            writer.WriteLine("GET / HTTP/1.1");
            writer.WriteLine("Host: www.acme.com");
            writer.WriteLine("");
            writer.Flush();

            string response = reader.ReadToEnd();
            
            //Console.Write(response);

            int first = response.IndexOf("<body>");
            int last = response.IndexOf("</body>");
            
            string bodySubstring = response.Substring(first, last - first);
            
            //Console.Write(bodySubstring);

            List<string> linksList = LinkExtractor.Extract(bodySubstring);
            var index = 0;

            foreach (var link in linksList)
            {
                index++;
                Console.WriteLine(index + " - " + link);
            }
            
            Console.WriteLine("Make a selection");
            var choice = Console.ReadLine();
            if (choice == "Exit")
            {
                Console.WriteLine("Closing connection!");
                writer.Close();
                reader.Close();
                tcpClient.Close();
            }
        }
    }

    class LinkExtractor
    {
        public static List<string> list = new List<string>();

        public static List<string> Extract(string html)
        {
            List<string> list = new List<string>();
            
            Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);

            if (regex.IsMatch(html))
            {
                foreach (Match match in regex.Matches(html))
                {
                    list.Add(match.Groups[1].Value);
                }
            }
            
            return list;
        }
    }
}