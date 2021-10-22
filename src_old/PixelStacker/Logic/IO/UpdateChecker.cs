﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelStacker.Logic
{
    public class UpdateSettings
    {
        public DateTime? LastChecked { get; set; } = null;
        public string SkipNotifyIfVersionIs { get; set; } = Constants.Version;
    }

    public class UpdateChecker
    {
        public async static Task CheckForUpdates(CancellationToken cancelToken)
        {
            var settings = Options.Get.UpdateSettings;

            if (settings.LastChecked == null || settings.LastChecked.Value < DateTime.UtcNow.AddHours(-2))
            {
                TaskManager.SafeReport(75, "Checking for updates");
                settings.LastChecked = DateTime.UtcNow;
                string latestVersion =
                    await DoRequest("https://api.spigotmc.org/legacy/update.php?resource=46812/")
                    ??
                    await DoGithubRequest()
                    ;

                Options.Save();

                if (latestVersion == null)
                {
                    TaskManager.SafeReport(100, "No updates available.");
                    return;
                }

                if (latestVersion == Constants.Version)
                {
                    TaskManager.SafeReport(100, "You are already using the latest version of PixelStacker");
                    return;
                }

                if (latestVersion == settings.SkipNotifyIfVersionIs)
                {
                    TaskManager.SafeReport(100, "Newest version available is still: "+latestVersion);
                    return;
                }

                TaskManager.SafeReport(100, "A new version is available!");
                var result = MessageBox.Show("A new update for PixelStacker is available. Would you like to download it? Say YES to go to the download page. Say NO to ignore this update.", "A new update is available.", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                if (result == DialogResult.No)
                {
                    settings.SkipNotifyIfVersionIs = latestVersion;
                }
                else if (result == DialogResult.Yes)
                {
                    settings.SkipNotifyIfVersionIs = latestVersion;
                    ProcessStartInfo sInfo = new ProcessStartInfo("https://www.spigotmc.org/resources/pixelstacker.46812/updates");
                    Process.Start(sInfo);
                }

                Options.Save();
            }
            return/* false*/;
        }


        private static async Task<string> DoRequest(string URL)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "PixelStacker");
                HttpResponseMessage response = client.GetAsync(URL).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                Console.WriteLine("Failed to check for updates.");
            }

            return null;
        }

        private static async Task<string> DoGithubRequest()
        {
            string json = await DoRequest("https://api.github.com/repos/pangamma/PixelStacker/releases/latest");
            var parsed = JsonConvert.DeserializeObject<GithubReleaseResponse>(json);
            string version = parsed.tag_name;
            return version;
        }
    }

    public class GithubReleaseResponse
    {
        public string tag_name { get; set; }
    }
}
