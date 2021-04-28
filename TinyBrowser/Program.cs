using System;
using System.IO;
using System.Net.Sockets;

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
            
            Console.Write(response);

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
}