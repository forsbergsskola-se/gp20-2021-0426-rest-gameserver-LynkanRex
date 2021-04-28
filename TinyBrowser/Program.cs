using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;

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
            
            // Console.WriteLine("Attempting to connect to acme.com over port 80...");
            //
            // // HTTP 0.9
            // //Byte[] request = Encoding.ASCII.GetBytes("GET /\r\n");
            //
            // Byte[] request = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\n" + 
            //                                          "Host: www.acme.com\r\n");
            //
            // stream.Write(request);
            //
            // Console.WriteLine("Sent request");
            //
            // Byte[] bytes = new Byte[1024];
            // var lengthOfResponse = stream.Read(bytes);
            //
            //
            // Console.WriteLine("Receiving request...");
            //
            //
            //
            
            var choice = Console.ReadLine();
            if (choice == "Exit")
            {
                Console.WriteLine("Closing connection!");
                stream.Close();
                tcpClient.Close();
            }
        }
    }
}