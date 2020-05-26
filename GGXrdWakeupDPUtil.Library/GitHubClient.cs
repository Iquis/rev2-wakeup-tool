using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GGXrdWakeupDPUtil.Library
{
    public class GitHubClient
    {
        private string dummyUserAgent = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public ReversalToolVersion GetLatestVersion()
        {
            try
            {
                string testUrl = "https://api.github.com/repos/Iquis/rev2-wakeup-tool/releases/latest";

                dynamic response;

                using (var webClient = new WebClient())
                {
                    webClient.Headers["User-Agent"] = dummyUserAgent;
                    string test = webClient.DownloadString(testUrl);
                    response = JsonConvert.DeserializeObject(test);
                }

                return new ReversalToolVersion()
                {
                    Version = response.tag_name,
                    Url = response.assets[0].browser_download_url
                };
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteLine("Failed to get LatestVersion");
                LogManager.Instance.WriteException(e);
                throw;
            }
        }
    }

    internal class GitHubRelease
    {
        public string Url { get; set; }
    }



}
