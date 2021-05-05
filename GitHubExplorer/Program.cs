using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubExplorer
{
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

        private static UserResponse ConvertUserRequestFromJSON(this string requestString)
        {
            return JsonSerializer.Deserialize<UserResponse>(requestString);
        }

        private static ReposResponse[] ConvertReposRequestFromJSON(this string requestString)
        {
            return JsonSerializer.Deserialize<ReposResponse[]>(requestString);
        }
        
        private static OrganizationResponse[] ConvertOrgRequestFromJSON(this string requestString)
        {
            return JsonSerializer.Deserialize<OrganizationResponse[]>(requestString);
        }
        
        private static OrganizationMembersResponse[] ConvertMembersRequestFromJSON(this string requestString)
        {
            return JsonSerializer.Deserialize<OrganizationMembersResponse[]>(requestString);
        }

        private static async Task DoProgramLoop()
        {
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
                        break;
                    }

                    HttpResponseMessage quickResponse = await HttpClient.GetAsync($"users/{userName}");

                    quickResponse.EnsureSuccessStatusCode();

                    string quickResponseBody = await quickResponse.Content.ReadAsStringAsync();

                    UserResponse quickUserResponse = quickResponseBody.ConvertUserRequestFromJSON();
                    
                    Console.WriteLine($"{quickUserResponse.name} from {quickUserResponse.location}, working for {quickUserResponse.company}");

                    Console.WriteLine(
                        $"Would you like to \n" +
                        $"1: view profile \n" +
                        $"2: view repositories\n" +
                        $"3: view organizations");
                    var choice = Console.ReadLine();

                    if (choice.ToLower() == "exit")
                    {
                        HttpClient.Dispose();
                        Console.WriteLine("Exiting...");
                        sessionActive = false;
                        break;
                    }

                    var choiceValue = Convert.ToInt32(choice);

                    if (choiceValue == 1)
                    {
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
                    else if (choiceValue == 2)
                    {
                        HttpResponseMessage response = await HttpClient.GetAsync($"users/{userName}/repos");

                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();

                        ReposResponse[] reposResponse = responseBody.ConvertReposRequestFromJSON();
                        
                        Console.WriteLine($"Repos for {userName}:");
                        foreach (var index in reposResponse)
                        {
                            
                            Console.WriteLine($"{index.name} ({index.description})");
                            Console.WriteLine($"{index.html_url}");
                            Console.WriteLine("");
                        }
                    }
                    else if (choiceValue == 3)
                    {
                        HttpResponseMessage response = await HttpClient.GetAsync($"users/{userName}/orgs");

                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();

                        OrganizationResponse[] organizationResponse = responseBody.ConvertOrgRequestFromJSON();

                        foreach (var index in organizationResponse)
                        {
                            Console.WriteLine($"{index.login} ({index.description})\n");
                            Console.WriteLine($"Members: ");

                            HttpResponseMessage membersResponse = await HttpClient.GetAsync($"orgs/{index.login}/members");

                            membersResponse.EnsureSuccessStatusCode();

                            string membersResponseBody = await membersResponse.Content.ReadAsStringAsync();

                            OrganizationMembersResponse[] orgMemberList =
                                membersResponseBody.ConvertMembersRequestFromJSON();
                            
                            foreach (var entry in orgMemberList)
                            {
                                Console.WriteLine(entry.login);
                                Console.WriteLine(entry.html_url);
                                Console.WriteLine("");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Improper input, please enter 1, 2 or exit");
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
        
        static async Task Main(string[] args)
        {
            var secret = LoadAndValidateSecrets();
            HttpClient.BaseAddress = new Uri("https://api.github.com");
        
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token",secret.token);
        
            Console.WriteLine("Welcome to the GitHub explorer.");
            
            await DoProgramLoop();
        }
    }
}
