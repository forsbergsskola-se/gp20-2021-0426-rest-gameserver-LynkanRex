using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace TinyBrowser
{
    class Program
    {

        private static TcpClient tcpClient;
        private static Stream stream;

        private static string host = "acme.com";
        private static string originPath = "/";
        
        static void Main(string[] args)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(host, 80);
            tcpClient.SendTimeout = 3000;
            tcpClient.ReceiveTimeout = 3000;
            
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            StreamReader reader = new StreamReader(tcpClient.GetStream());
            
            string path = originPath;
            
            CompileWriterRequest(writer, path);

            string response = reader.ReadToEnd();
            
            int first = response.IndexOf("<body>");
            int last = response.IndexOf("</body>");
            
            string bodySubstring = response.Substring(first, last - first);
            
            Dictionary<int, string> linksList = LinkExtractor.Extract(bodySubstring);

            foreach (var link in linksList)
            {
                Console.WriteLine(link);
            }

            bool activeSession = true;
            
            while (activeSession)
            {
                Console.WriteLine("Make a selection, or enter 'Exit' to close");
                
                var choice = Console.ReadLine();
                
                if (choice == "Exit")
                {
                    Console.WriteLine("Closing connection!");
                    writer.Close();
                    reader.Close();
                    tcpClient.Close();
                    activeSession = false;
                }
                else
                {
                    int number = 0;
                    var convertedChoiceIsNumber = Int32.TryParse(choice, out number);

                    if (convertedChoiceIsNumber)
                    {
                        if (number >= 0)
                        {
                            string result = "";
                            if(linksList.TryGetValue(number, out result))
                            {
                                path = "/"+result;
                            
                                Console.WriteLine($"Attempting to connect to {host}{path}");
                            
                                tcpClient.Close(); 
                                tcpClient = new TcpClient();
                                tcpClient.Connect(host, 80);
                            
                            
                                writer = new StreamWriter(tcpClient.GetStream());
                                reader = new StreamReader(tcpClient.GetStream());
                            
                                CompileWriterRequest(writer, path);
                            
                                string newResponse = reader.ReadToEnd();
                            
                                Console.Write(newResponse);
                                int newFirst = newResponse.IndexOf("<body>");
                                int newLast = newResponse.IndexOf("</body>");
                            
                                Console.WriteLine(newFirst);
                                Console.WriteLine(newLast);
                            
                                string newBodySubstring = newResponse.Substring(newFirst+1, newLast - newFirst);
            
                                Dictionary<int, string> newLinksList = LinkExtractor.Extract(newBodySubstring);

                                foreach (var link in newLinksList)
                                {
                                    Console.WriteLine(link);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect entry, please give a number or \"Exit\" and press Return");
                    }
                }
            }
        }

        private static void CompileWriterRequest(StreamWriter writer, string path)
        {
            Console.WriteLine($"GET {path} HTTP/1.1");
            writer.WriteLine($"GET {path} HTTP/1.1");
            writer.WriteLine("Host: www.acme.com");
            writer.WriteLine("Connection: close");
            writer.WriteLine("");
            writer.Flush();
        }
    }

    class LinkExtractor
    {
        public static Dictionary<int, string> dictionary = new Dictionary<int, string>();

        public static Dictionary<int, string> Extract(string html)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            int index = 1;            
            Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);

            if (regex.IsMatch(html))
            {
                foreach (Match match in regex.Matches(html))
                {
                    dictionary.Add(index, match.Groups[1].Value);
                    index++;
                }
            }
            return dictionary;
        }
    }
}