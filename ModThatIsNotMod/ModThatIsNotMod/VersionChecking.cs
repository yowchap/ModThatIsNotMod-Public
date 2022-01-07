using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace ModThatIsNotMod
{
    public static class VersionChecking
    {
        /// <summary>
        /// Checks if there's a newer version of the mod on thunderstore.io and tells the user if there is.
        /// </summary>
        public static void CheckModVersion(MelonMod mod, string packageUrl)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.Timeout = new TimeSpan(0, 0, 5);

                HttpResponseMessage response = httpClient.GetAsync("https://boneworks.thunderstore.io/api/v1/package/").Result;
                HttpContent responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;

                ModData[] modData = JsonConvert.DeserializeObject<ModData[]>(responseString);
                bool foundMod = false;
                foreach (ModData data in modData)
                {
                    if (data.package_url == packageUrl)
                    {
                        foundMod = true;

                        Version latestVersion = new Version(data.versions[0].version_number);
                        foreach (ModData.ModVersion version in data.versions)
                        {
                            Version tempVersion = new Version(version.version_number);
                            if (tempVersion.CompareTo(latestVersion) > 0)
                                latestVersion = tempVersion;
                        }

                        Version usedVersion = new Version(mod.Info.Version);

                        // Warn the user if the mod is outdated. I doubt most people will pay attention but I can at least try ¯\_(ツ)_/¯
                        if (usedVersion.CompareTo(latestVersion) < 0)
                        {
                            ModConsole.Msg(ConsoleColor.Yellow, $"You are using an outdated version of {mod.Info.Name}", LoggingMode.MINIMAL);
                            ModConsole.Msg(ConsoleColor.Yellow, $"Your version: {usedVersion}, Latest version: {latestVersion}", LoggingMode.NORMAL);
                            ModConsole.Msg(ConsoleColor.Yellow, $"Download link: {packageUrl}", LoggingMode.MINIMAL);
                        }
                        else
                        {
                            ModConsole.Msg(ConsoleColor.Green, $"You are using the latest version of {mod.Info.Name}!");
                        }

                        break;
                    }
                }

                if (!foundMod)
                    ModConsole.Msg(ConsoleColor.Yellow, $"Couldn't check version for {mod.Info.Name}");

                responseContent.Dispose();
                response.Dispose();
                httpClient.Dispose();
                handler.Dispose();
            }
            catch
            {
                ModConsole.Msg(ConsoleColor.Yellow, $"Version checking for {mod.Info.Name} timed out.");
            }
        }

        private struct ModData
        {
#pragma warning disable CS0649
            public string name;
            public string full_name;
            public string owner;
            public string package_url;
            public string date_created;
            public string date_updated;
            public string uuid4;
            public int rating_score;
            public bool is_pinned;
            public bool is_deprecated;
            public bool has_nsfw_content;
            public string[] categories;
            public ModVersion[] versions;

            public struct ModVersion
            {
                public string name;
                public string full_name;
                public string description;
                public string icon;
                public string version_number;
                public string[] dependencies;
                public string download_url;
                public int downloads;
                public string date_created;
                public string website_url;
                public bool is_active;
                public string uuid4;
            }
#pragma warning restore 0649
        }
    }
}
