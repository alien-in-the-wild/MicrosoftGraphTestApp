﻿using System.Text.Json.Nodes;
using MicrosoftGraphTestApp.GraphData;

namespace MicrosoftGraphTestApp
{
    class Program
    {
        /// <summary>
        /// Entry method
        /// </summary>
        /// <param name="_"></param>
        static async Task Main(string[] _)
        {
            GraphHelper graphHelper;
            Settings settings;
            
            try
            {
                // Load settings
                settings = Settings.LoadSettings();
                
                // Initialize Graph
                graphHelper = new GraphHelper(settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initializing Graph: " + e.Message);
                return;
            }
            
            int choice = -1;

            while (choice != 0)
            {
                Console.WriteLine("Please choose one of the following options:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Download groups");

                try
                {
                    choice = int.Parse(Console.ReadLine() ?? string.Empty);
                }
                catch (FormatException)
                {
                    // Set to invalid value
                    choice = -1;
                }

                switch(choice)
                {
                    case 0:
                        // Exit the program
                        Console.WriteLine("Goodbye...");
                        break;
                    case 1:
                        // Download groups
                        await DownloadGroupsAsync(settings, graphHelper);
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Fetches Groups from MS Graph API and save JSON to files
        /// </summary>
        /// <param name="settings">Settings instance</param>
        /// <param name="graphHelper">GraphHelper instance</param>
        private static async Task DownloadGroupsAsync(Settings settings, GraphHelper graphHelper)
        {
            try
            {
                // Fetching Groups from MS Graph API
                var groupsResponse = await graphHelper.GetGroupsAsync();
                if (groupsResponse == null || groupsResponse.Value == null || groupsResponse.Response == null)
                    throw new NullReferenceException("Error getting groups response.");
            
                // Path to save files
                string path = Path.Combine(settings.SaveGroupsPath, "MSGraph/Groups");
                Directory.CreateDirectory(path);
            
                // Parsing JSON to extract group objects
                JsonNode? jsonNode = JsonNode.Parse(groupsResponse.Response);
                if (jsonNode != null)
                {
                    Console.WriteLine($"Groups count: {groupsResponse.Value.Count}");
                    JsonArray jsonGroups = jsonNode["value"]!.AsArray();

                    for (int nGroup = 0; nGroup < groupsResponse.Value.Count; nGroup++)
                    {
                        Group group = groupsResponse.Value[nGroup];
                        string filePath = Path.Combine(path, $"{group.DisplayName}.json");
                        string groupJsonText = jsonGroups[nGroup]!.ToJsonString();
                    
                        // Saving group to the file
                        await File.WriteAllTextAsync(filePath, groupJsonText);
                        Console.WriteLine($"Group {nGroup+1}: {filePath}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error downloading groups: " + e.Message);
            }
        }
    }
}