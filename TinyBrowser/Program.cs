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

        private static StreamWriter writer;
        private static StreamReader reader;

        private static string host = "acme.com";
        private static string originPath = "/";
        
        static void Main(string[] args)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(host, 80);
            tcpClient.SendTimeout = 3000;
            tcpClient.ReceiveTimeout = 3000;
            
            writer = new StreamWriter(tcpClient.GetStream());
            reader = new StreamReader(tcpClient.GetStream());
            
            string path = originPath;
            
            CompileWriterRequest(writer, path);

            string response = reader.ReadToEnd();

            var websiteURI = new UriBuilder(null, host);
            var pageTitle = ExtractTagFromWebPage(response, "<title>", "</title>");
            var bodyTag = ExtractTagFromWebPage(response, "<body>", "</body>");
            
            Console.WriteLine($"Connected to {websiteURI}");
            Console.WriteLine($"Page: {pageTitle}");

            Dictionary<int, string> linksList = LinkExtractor.Extract(bodyTag);

            foreach (var link in linksList)
            {
                Console.WriteLine(link);
            }

            bool activeSession = true;
            
            while (activeSession)
            {
                Console.WriteLine("Make a selection, or enter 'Exit' to close");
                
                var choice = Console.ReadLine();
                
                if (choice.ToLower() == "exit")
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
                                
                                // TODO: Check if the first part after http:// or https:// matches the host, if not,
                                // connect to the new website host and go from there (example: http://blank.org)
                                
                                tcpClient.Connect(host, 80);

                                writer = new StreamWriter(tcpClient.GetStream());
                                reader = new StreamReader(tcpClient.GetStream());
                            
                                CompileWriterRequest(writer, path);
                            
                                response = reader.ReadToEnd();
                            
                                pageTitle = ExtractTagFromWebPage(response, "<title>", "</title>");
            
                                Console.WriteLine($"Page: {pageTitle}");
                                
                                bodyTag = ExtractTagFromWebPage(response, "<body>", "</body>");

                                Dictionary<int, string> newLinksList = LinkExtractor.Extract(bodyTag);

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

        private static string ExtractTagFromWebPage(string pageContentString, string startTag, string endTag)
        {
            int startIndex = pageContentString.IndexOf(startTag);
            int endIndex = pageContentString.IndexOf(endTag);

            startIndex += startTag.Length;
            
            var tag = pageContentString[startIndex..endIndex];

            return tag;
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
                    // TODO: Match for .js and other file extensions, and break out of this iteration if something like that is met,
                    // we're only interested in website-based HyperText Links
                    
                    // TODO: Match for if the link contains the host value, and remove that part in that case

                    dictionary.Add(index, match.Groups[1].Value);
                    index++;
                }
            }
            return dictionary;
        }
    }
}