using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace GTNH_Manager
{
    public partial class GTNHManager : Form
    {
        private string version = "0.2";
        private string lastRemainingTime = "";
        public GTNHManager()
        {
            InitializeComponent();
            Text = $"GTNH Manager {version}";
            foreach (var v in ModpackVersion.getUrlDict)
            {
                versionListBox.Items.Add(v.Key);
            }
        }

        private async void installButton_Click(object sender, EventArgs e)
        {
            if (IsMultiMCPathValid() && versionListBox.SelectedItem != null)
            {
                installButton.Enabled = false;
                multiMCPathTextBox.Enabled = false;
                versionListBox.Enabled = false;
                optifineCheckBox.Enabled = false;
                suCheckBox.Enabled = false;

                string? selectedVersion = versionListBox.SelectedItem.ToString();

                if (selectedVersion != null && ModpackVersion.getUrlDict.TryGetValue(selectedVersion, out string? mpURL))
                {
                    await Stage1(mpURL);
                    await Stage2();
                    Stage3();
                    Stage4();
                    await Stage5();
                    await Stage6();

                    progressLabel.Text = "";
                    UpdateStatusLabel("Installation complete", Color.Green);
                }
                else
                {
                    UpdateStatusLabel("Download failed", Color.Red);
                }

                installButton.Enabled = true;
                multiMCPathTextBox.Enabled = true;
                versionListBox.Enabled = true;
                optifineCheckBox.Enabled = true;
                suCheckBox.Enabled = true;
            }
            else
            {
                if (!IsMultiMCPathValid() && versionListBox.SelectedItem == null)
                    UpdateStatusLabel("Invalid MultiMC path and version", Color.Red);
                else if (!IsMultiMCPathValid())
                    UpdateStatusLabel("Invalid MultiMC path", Color.Red);
                else if (versionListBox.SelectedItem == null)
                    UpdateStatusLabel("Select a version", Color.Red);
            }
        }
        private async Task Stage1(string mpURL)
        {
            // Download the modpack zip file to the Temp path
            string tempMpPath = GetTempMpPath();
            UpdateStatusLabel("Downloading the modpack", Color.Blue);
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(mpURL, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    long? contentLength = response.Content.Headers.ContentLength;
                    int bufferSize = 8192;
                    byte[] buffer = new byte[bufferSize];
                    long totalBytesRead = 0;
                    int bytesRead;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (Stream downloadStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(tempMpPath, FileMode.Create, FileAccess.Write))
                    {
                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            UpdateProgress(totalBytesRead, contentLength, stopwatch);
                        }
                    }
                }
            }
        }
        private async Task Stage2()
        {
            // Extract the zip file to the correct folder
            UpdateStatusLabel("Extracting modpack zip file", Color.Blue);
            string tempMpPath = GetTempMpPath();
            string instancesPath = GetInstancesPath();
            await Task.Run(() =>
            {
                using (FileStream fileStream = File.OpenRead(tempMpPath))
                using (ZipInputStream zipInputStream = new ZipInputStream(fileStream))
                {
                    // Use the Length property of the FileInfo class to get the size of the zip file on disk
                    long totalBytesToExtract = new FileInfo(tempMpPath).Length;
                    long totalBytesExtracted = 0;
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    ZipEntry entry;
                    while ((entry = zipInputStream.GetNextEntry()) != null)
                    {
                        string? directoryName = Path.GetDirectoryName(entry.Name);
                        string fileName = Path.GetFileName(entry.Name);

                        if (!string.IsNullOrEmpty(directoryName))
                        {
                            Directory.CreateDirectory(Path.Combine(instancesPath, directoryName));
                        }

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string destinationPath = Path.Combine(instancesPath, entry.Name);
                            using (FileStream destinationStream = File.Create(destinationPath))
                            {
                                int bufferSize = 8192;
                                byte[] buffer = new byte[bufferSize];
                                int bytesRead;
                                while ((bytesRead = zipInputStream.Read(buffer, 0, bufferSize)) > 0)
                                {
                                    destinationStream.Write(buffer, 0, bytesRead);
                                    totalBytesExtracted += bytesRead;
                                    UpdateProgress(totalBytesExtracted, totalBytesToExtract, stopwatch);
                                }
                            }
                        }
                    }
                }
            });
        }
        private void Stage3()
        {
            // Delete extracted zip file
            UpdateStatusLabel("Deleting modpack zip file", Color.Blue);
            ClearProgress();
            string tempMpPath = GetTempMpPath();
            File.Delete(tempMpPath);
        }
        private void Stage4()
        {
            // Change the instance.cfg file content
            UpdateStatusLabel("Changing the modpack settings", Color.Blue);
            string mpFolderPath = GetModpackFolderPath();
            string instanceCfgPath = Path.Combine(mpFolderPath, "instance.cfg");
            string[] instanceCfgLines = new string[]
            {
        "InstanceType=OneSix",
        "JavaPath=javaw",
        "JoinServerOnLaunch=false",
        "JvmArgs=--illegal-access=warn -Djava.security.manager=allow -Dfile.encoding=UTF-8 --add-opens java.base/jdk.internal.loader=ALL-UNNAMED --add-opens java.base/java.net=ALL-UNNAMED --add-opens java.base/java.nio=ALL-UNNAMED --add-opens java.base/java.io=ALL-UNNAMED --add-opens java.base/java.lang=ALL-UNNAMED --add-opens java.base/java.lang.reflect=ALL-UNNAMED --add-opens java.base/java.text=ALL-UNNAMED --add-opens java.base/java.util=ALL-UNNAMED --add-opens java.base/jdk.internal.reflect=ALL-UNNAMED --add-opens java.base/sun.nio.ch=ALL-UNNAMED --add-opens jdk.naming.dns/com.sun.jndi.dns=ALL-UNNAMED,java.naming --add-opens java.desktop/sun.awt.image=ALL-UNNAMED --add-modules jdk.dynalink --add-opens jdk.dynalink/jdk.dynalink.beans=ALL-UNNAMED --add-modules java.sql.rowset --add-opens java.sql.rowset/javax.sql.rowset.serial=ALL-UNNAMED",
        "MaxMemAlloc=8192",
        "MinMemAlloc=6144",
        "OverrideCommands=false",
        "OverrideConsole=false",
        "OverrideGameTime=false",
        "OverrideJavaArgs=true",
        "OverrideJavaLocation=true",
        "OverrideMemory=true",
        "OverrideNativeWorkarounds=false",
        "OverrideWindow=false",
        "PermGen=128",
        "iconKey=default",
        $"name={versionListBox.SelectedItem.ToString()}",
        "notes="
            };
            File.WriteAllLines(instanceCfgPath, instanceCfgLines);

            // Change the mmc-pack.json file content
            string mmcPackFilePath = Path.Combine(mpFolderPath, "mmc-pack.json");
            string mmcPackFileContent = @"{
    ""components"": [
        {
            ""cachedName"": ""LWJGL 3"",
            ""cachedVersion"": ""3.3.1"",
            ""cachedVolatile"": true,
            ""dependencyOnly"": true,
            ""uid"": ""org.lwjgl3"",
            ""version"": ""3.3.1""
        },
        {
            ""cachedName"": ""Minecraft"",
            ""cachedRequires"": [
                {
                    ""suggests"": ""2.9.4-nightly-20150209"",
                    ""uid"": ""org.lwjgl""
                }
            ],
            ""cachedVersion"": ""1.7.10"",
            ""important"": true,
            ""uid"": ""net.minecraft"",
            ""version"": ""1.7.10""
        },
        {
            ""cachedName"": ""Forge-lwjgl3ify-Patches"",
            ""cachedVersion"": ""1.4.0"",
            ""uid"": ""me.eigenraven.lwjgl3ify.forgepatches""
        },
        {
            ""cachedName"": ""Forge"",
            ""cachedRequires"": [
                {
                    ""equals"": ""1.7.10"",
                    ""uid"": ""net.minecraft""
                }
            ],
            ""cachedVersion"": ""10.13.4.1614"",
            ""uid"": ""net.minecraftforge"",
            ""version"": ""10.13.4.1614""
        }
    ],
    ""formatVersion"": 1
}";
            File.WriteAllText(mmcPackFilePath, mmcPackFileContent);
        }
        private async Task Stage5()
        {
            // Install optifine
            UpdateStatusLabel("Downloading Optifine", Color.Blue);
            if (optifineCheckBox.Checked)
            {
                string optifineFilePath = Path.Combine(GetModpackFolderPath(), ".minecraft", "mods", "OptiFine.jar");
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(ModpackVersion.OptifineDownloadUrl);
                    response.EnsureSuccessStatusCode();
                    long? contentLength = response.Content.Headers.ContentLength;
                    int bufferSize = 8192;
                    byte[] buffer = new byte[bufferSize];
                    long totalBytesRead = 0;
                    int bytesRead;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            opFileStream = new FileStream(optifineFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
                    {
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                        {
                            await opFileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            UpdateProgress(totalBytesRead, contentLength, stopwatch);
                        }
                    }
                }
            }
        }
        private async Task Stage6()
        {
            // Install server utilities
            UpdateStatusLabel("Downloading Server Utilities", Color.Blue);
            if (suCheckBox.Checked)
            {
                string serverUtilitiesFilePath = Path.Combine(GetModpackFolderPath(), ".minecraft", "mods", "ServerUtilities.jar");
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(ModpackVersion.SuDownloadUrl);
                    response.EnsureSuccessStatusCode();
                    long? contentLength = response.Content.Headers.ContentLength;
                    int bufferSize = 8192;
                    byte[] buffer = new byte[bufferSize];
                    long totalBytesRead = 0;
                    int bytesRead;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            suFileStream = new FileStream(serverUtilitiesFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
                    {
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                        {
                            await suFileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            UpdateProgress(totalBytesRead, contentLength, stopwatch);
                        }
                    }
                }
            }
        }
        private bool IsMultiMCPathValid() => Directory.Exists(multiMCPathTextBox.Text);
        private string GetTempMpPath() => Path.Combine(Path.GetTempPath(), $"GT_New_Horizons_{versionListBox.SelectedItem}_Client_Java_17-20.zip");
        private string GetInstancesPath() => Path.Combine(multiMCPathTextBox.Text, "instances");
        private string GetModpackFolderPath() => Path.Combine(GetInstancesPath(), $"GT New Horizons {versionListBox.SelectedItem}");
        private void UpdateStatusLabel(string text, Color color)
        {
            statusLabel.Invoke(new Action(() => { (statusLabel.Text, statusLabel.ForeColor) = (text, color); }));
        }
        private void ClearProgress()
        {
            progressLabel.Invoke(new Action(() => { progressLabel.Text = ""; }));
            progressBar.Invoke(new Action(() => { progressBar.Value = 0; }));
        }
        private void UpdateProgress(long totalBytesRead, long? contentLength, Stopwatch stopwatch)
        {
            if (contentLength.HasValue && totalBytesRead > contentLength) totalBytesRead = (long)contentLength;
            double? percentComplete = contentLength.HasValue ? (totalBytesRead * 1d) / (contentLength * 1d) * 100 : 0;
            double bytesPerSecond = totalBytesRead / stopwatch.Elapsed.TotalSeconds;
            string speed = bytesPerSecond / 1024 / 1024 > 1 ? $"{bytesPerSecond / 1024 / 1024:F2} MB/s" : $"{bytesPerSecond / 1024:F2} KB/s";

            progressBar.Invoke(new Action(() => { progressBar.Value = Math.Min((int)percentComplete, 100); }));

            string progressText = $"{(int)percentComplete}% ({Math.Min(totalBytesRead, contentLength.GetValueOrDefault()) / (double)(1024 * 1024):F2} / {contentLength / (double)(1024 * 1024):F2} MB), Speed: {speed}, ";

            TimeSpan remainingTime = TimeSpan.FromSeconds(3600);
            if (contentLength.HasValue && bytesPerSecond > 0)
            {
                long remainingBytes = contentLength.Value - totalBytesRead;
                double remainingSeconds = remainingBytes / bytesPerSecond;
                remainingTime = TimeSpan.FromSeconds(remainingSeconds);
                lastRemainingTime = remainingTime.ToString(@"hh\:mm\:ss");
            }
            progressText += $" Time remaining: {lastRemainingTime}";

            progressLabel.Invoke(new Action(() => { progressLabel.Text = progressText; }));
        }
    }
}