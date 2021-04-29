using System;
using System.IO;
using System.Text.Json;

namespace GitHubExplorer
{
    class Secrets
    {
        public string token { get; set; }
    }
    
    class Program
    {
        static Secrets LoadAndValidateSecrets()
        {
            Secrets secrets;

            if (!File.Exists("secrets.json"))
            {
                secrets = new Secrets();
                File.WriteAllText("secrets.json", JsonSerializer.Serialize(secrets));
            }
            else
            {
                secrets = JsonSerializer.Deserialize<Secrets>(File.ReadAllText("secrets.json"));
            }

            if (string.IsNullOrEmpty(secrets.token))
            {
                throw new Exception("ERROR: You need to define a Token in 'secrets.json' file to work");
            }
            
            return secrets;
        }

        static void Main(string[] args)
        {
            var secret = LoadAndValidateSecrets();
            
            Console.WriteLine(secret.token);
            
            return;
            Console.WriteLine("Hello! Please enter a username to view");
        }
    }
}
