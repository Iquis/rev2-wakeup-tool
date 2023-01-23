using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;

namespace GGXrdWakeupDPUtil.Library
{
    public class UpdateManager
    {
        private readonly GitHubClient _gitHubClient = new GitHubClient();
        private string releaseFolder = "Release";
        public ReversalToolVersion CheckUpdates()
        {
            try
            {
                ReversalToolVersion latestVersion = _gitHubClient.GetLatestVersion();

                return latestVersion;
            }
            catch (Exception)
            {
                LogManager.Instance.WriteLine("Failed to check for updates");
                throw;
            }
        }

        public bool DownloadUpdate(ReversalToolVersion reversalToolVersion)
        {
            string zipFile = $"{releaseFolder}_{Guid.NewGuid()}.zip";


            using (WebClient client = new WebClient())
            {
                client.DownloadFile(reversalToolVersion.Url, zipFile);
            }

            DeleteFolder(this.releaseFolder);

            System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, Directory.GetCurrentDirectory());

            File.Delete(zipFile);


            return true;
        }

        private void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }

        public bool InstallUpdate()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            foreach (var fileName in Directory.GetFiles(currentDirectory))
            {
                if (!fileName.EndsWith(LogManager.Instance.FileName))
                {
                    FileInfo fi = new FileInfo(fileName);

                    string newPath = $"{fi.Directory}\\{Path.GetFileNameWithoutExtension(fileName)}_OLD{fi.Extension}";

                    File.Move(fileName, newPath);
                }
            }

            foreach (var fileName in Directory.GetFiles($"{currentDirectory}\\{releaseFolder}"))
            {
                FileInfo fi = new FileInfo(fileName);
                File.Move(fileName, $"{currentDirectory}\\{fi.Name}");
            }

            DeleteFolder(this.releaseFolder);


            return true;
        }

        public void RestartApplication()
        {
            Assembly executingAssembly = Assembly.GetEntryAssembly();
            string file = executingAssembly.Location;
            if (File.Exists(file))
            {
                Process.Start(file);
                System.Environment.Exit(1);
            }
        }

        public void SaveVersion(ReversalToolVersion version)
        {
            Assembly executingAssembly = Assembly.GetEntryAssembly();
            var configFileName = $"{executingAssembly.Location}.config";

            if (!string.IsNullOrEmpty(configFileName))
            {
                XDocument doc = XDocument.Load(configFileName);
                var configurationNode = doc.Root;
                var appSettingsNode = configurationNode.Descendants("appSettings").FirstOrDefault();

                var versionNode = appSettingsNode.Descendants("aa").FirstOrDefault(x => x.Attribute("key").Value == "Version");

                if (versionNode == null)
                {
                    versionNode = new XElement("add");
                    appSettingsNode.Add(versionNode);
                }

                versionNode.SetAttributeValue("key", "CurrentVersion");
                versionNode.SetAttributeValue("value", version.Version.ToString());

                doc.Save(configFileName);

            }
        }

        public void CleanOldFiles()
        {
            foreach (var fileName in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                FileInfo fi = new FileInfo(fileName);

                if (fi.Name.EndsWith($"_OLD{fi.Extension}"))
                {
                    fi.Delete();
                }
            }
            DeleteFolder(this.releaseFolder);
        }
    }

    public class ReversalToolVersion
    {
        public Version Version { get; set; }

        public string Url { get; set; }
    }
}
