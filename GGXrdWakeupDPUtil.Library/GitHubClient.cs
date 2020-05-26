using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

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

                GitHubRelease response;

                using (var webClient = new WebClient())
                {
                    webClient.Headers["User-Agent"] = dummyUserAgent;
                    string stringResult = webClient.DownloadString(testUrl);

                    var ser = new DataContractJsonSerializer(typeof(GitHubRelease));
                    response = ser.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(stringResult))) as GitHubRelease;
                }

                return new ReversalToolVersion()
                {
                    Version = response.Version,
                    Url = response.Url
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

    [DataContract]
    internal class GitHubRelease
    {
        [DataMember(Name = "tag_name")]
        public string Version { get; set; }
        public string Url => this.Assets.FirstOrDefault()?.DownloadUrl;

        [DataMember(Name = "assets")]
        public GitHubReleaseAsset[] Assets { get; set; }

    }

    [DataContract]
    internal class GitHubReleaseAsset
    {
        [DataMember(Name = "browser_download_url")]
        public string DownloadUrl { get; set; }
    }


}
