using System;
using System.IO;
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
            stream = tcpClient.GetStream();
            
            Console.WriteLine("Attempting to connect to acme.com over port 80...");

            // HTTP 0.9
            Byte[] request = Encoding.ASCII.GetBytes("GET /\r\n");
            
            // Byte[] request = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\n" +
            //                                          "Host: www.acme.com\r\n" +
            //                                          "Connection: Keep-Alive\r\n");
            
            stream.Write(request);
            
            Console.WriteLine("Sent request");
            
            Byte[] bytes = new Byte[1024];
            var lengthOfResponse = tcpClient.Client.Receive(bytes);

            Console.WriteLine("Receiving request...");
            
            var response = Encoding.ASCII.GetString(bytes, 0 , lengthOfResponse);
            
            Console.Write(response);
            
            var choice = Console.ReadLine();
            if (choice == "Exit")
            {
                Console.WriteLine("Closing connection!");    
                tcpClient.Close();
            }
        }
    }
}
