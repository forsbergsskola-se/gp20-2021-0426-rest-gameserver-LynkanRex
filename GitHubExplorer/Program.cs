using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
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

        private static UserResponse ConvertUserRequestFromJSON(this string stringToSplit)
        {
            return JsonSerializer.Deserialize<UserResponse>(stringToSplit);
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
                    // TODO: Needs to also send request to username/repos & username/orgs to get details from there.
                    Console.WriteLine("Please enter the name of a Github User that you'd like to look at" + 
                                          "\nYou can always enter 'Exit' to close");
                    var userName = Console.ReadLine();
                    
                    if (userName.ToLower() == "exit")
                    {
                        HttpClient.Dispose();
                        Console.WriteLine("Exiting...");
                        sessionActive = false;
                    }

                    Console.WriteLine($"Attempting to access {HttpClient.BaseAddress}users/{userName}");
        
                    HttpResponseMessage response = await HttpClient.GetAsync($"users/{userName}");
        
                    response.EnsureSuccessStatusCode();
        
                    string responseBody = await response.Content.ReadAsStringAsync();
        
                    UserResponse userResponse = responseBody.ConvertUserRequestFromJSON();
                        
                    foreach (PropertyDescriptor entry in TypeDescriptor.GetProperties(userResponse))
                    {
                        string name = entry.Name;
                        object value = entry.GetValue(userResponse);
                        Console.WriteLine("{0}: {1}", name, value);
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException caught!");
                    Console.WriteLine("\nMessage: {0} ", e.Message);
                    throw;
                }
            }
        }
    }
}
