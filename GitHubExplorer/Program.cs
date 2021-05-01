using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubExplorer
{
    class Secrets
    {
        public string token { get; set; }
    }

    static class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        
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

        private static string[] SplitString(this string stringToSplit)
        {
            string[] splitString = stringToSplit.Split(",");
            
            return splitString;
        }

        static async Task Main(string[] args)
        {
            var secret = LoadAndValidateSecrets();
            HttpClient.BaseAddress = new Uri("https://api.github.com");

            HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token",secret.token);

            Console.WriteLine("Welcome to the GitHub explorer.");

            bool sessionActive = true;
            
            while (sessionActive)
            {
                try
                {
                    Console.WriteLine("Please enter either of the following options:\n" +
                                      "users\n" +
                                      "repos\n" +
                                      "\nOr Exit to close");

                    var userChoice = Console.ReadLine();
                    
                    if (userChoice.ToLower() == "users")
                    {
                        // TODO: Needs to also send request to username/repos & username/orgs to get details from there.
                        // TODO: Create classes that contains Properties that can be converted from the JSON in stead of doing string splits
                        Console.WriteLine("Please enter the name of the User that you'd like to look at");
                        var userName = Console.ReadLine();

                        Console.WriteLine($"Attempting to access {HttpClient.BaseAddress}users/{userName}");

                        HttpResponseMessage response = await HttpClient.GetAsync($"users/{userName}");

                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var splitStrings = responseBody.SplitString();

                        foreach (var entry in splitStrings)
                        {
                            Console.WriteLine(entry);
                        }
                    }

                    if (userChoice.ToLower() == "repos")
                    {
                        Console.WriteLine("Repos it is");
                    }

                    if (userChoice.ToLower() == "exit")
                    {
                        HttpClient.Dispose();
                        Console.WriteLine("Exiting...");
                        sessionActive = false;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException caught!");
                    Console.WriteLine("\nMessage: {0} ", e.Message);
                    throw;
                }
                finally
                {
                    Console.WriteLine("Exited successfully");
                }
            }
        }
    }
}
