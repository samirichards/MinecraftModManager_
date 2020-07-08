using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MinecraftModManager.Classes
{
    public class Mod :IDisposable
    {
        public string modid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string mcversion { get; set; }
        public string url { get; set; }
        public string updateUrl { get; set; }
        public string updateJson { get; set; }
        public string[] authorList { get; set; }
        public string credits { get; set; }
        public string logoFile { get; set; }
        public string[] screenshots { get; set; }
        public string parent { get; set; }
        public bool useDependencyInformation { get; set; }
        public string[] requiredMods { get; set; }
        public string[] dependencies { get; set; }
        public string[] dependants { get; set; }
        public BitmapImage logo { get; set; }
        public bool updateAvailable { get; set; }
        public string ToolTipMessage { get; set; }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            GC.Collect(4);
            this.Dispose();
        }

        public async Task CheckForUpdate()
        {
            try
            {
                if (updateJson != null)
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage responseMessage = await client.GetAsync(updateJson);
                    responseMessage.EnsureSuccessStatusCode();
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    JObject response = JObject.Parse(responseBody);

                    if (Utilities.IsVersionLater(response["promos"][mcversion + "-latest"].ToString(), version))
                    {
                        updateAvailable = true;
                        //MessageBox.Show("An update is availaible for " + name + " on " + mcversion + Environment.NewLine + name + " version " + response["promos"][mcversion + "-latest"].ToString());
                        ToolTipMessage = "Installed version: " + version + Environment.NewLine + "New version: " + response["promos"][mcversion + "-latest"].ToString();
                    }
                    else
                    {
                        updateAvailable = false;
                    }
                }
                else
                {
                    updateAvailable = false;
                }
            }
            catch (Exception e)
            {
                updateAvailable = false;
                MessageBox.Show("Error getting " + name + " update status\n" + e.InnerException.Message);
            }
        }
    }
}
