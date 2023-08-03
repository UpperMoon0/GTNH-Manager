using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Ionic.Zip;

namespace GTNH_Manager
{
    public partial class GTNHManager : Form
    {
        private string version = "0.2";
        private DateTime lastUpdateTime = DateTime.MinValue;
        private string lastRemainingTime = "";
        public GTNHManager()
        {
            InitializeComponent();
            versionListBox.Items.Add("2.3.5");
            versionListBox.Items.Add("2.3.4");
            versionListBox.Items.Add("2.3.3");
        }

        private async void installButton_Click(object sender, EventArgs e)
        {
            // Check if the MultiMC path is valid and if a version is selected
            if (IsMultiMCPathValid() && versionListBox.SelectedItem != null)
            {
                // Disable the widgets
                installButton.Enabled = false;
                multiMCPathTextBox.Enabled = false;
                versionListBox.Enabled = false;
                optifineCheckBox.Enabled = false;

                // Get the selected version
                string? selectedVersion = versionListBox.SelectedItem.ToString();

                // Get the URL for the selected version
                if (selectedVersion != null && ModpackVersion.getUrlDict.TryGetValue(selectedVersion, out string? downloadUrl))
                {
                    // Set the temporary folder path and file path
                    string tempFolderPath = Path.GetTempPath();
                    string filePath = Path.Combine(tempFolderPath, $"GT_New_Horizons_{versionListBox.SelectedItem.ToString()}_Client_Java_17-20.zip");

                    // Update the status label
                    UpdateStatusLabel("Downloading the modpack", Color.Blue);

                    // Download the file using HttpClient
                    await InstallModpackFile(downloadUrl, filePath);

                    // Update the progress and status labels
                    progressLabel.Text = "";
                    UpdateStatusLabel("Installation complete", Color.Green);
                }
                else
                {
                    UpdateStatusLabel("Download failed", Color.Red);
                }

                // Enable the widgets
                installButton.Enabled = true;
                multiMCPathTextBox.Enabled = true;
                versionListBox.Enabled = true;
                optifineCheckBox.Enabled = true;
            }
            else
            {
                // Update the status label with an appropriate message
                if (!IsMultiMCPathValid() && versionListBox.SelectedItem == null)
                    UpdateStatusLabel("Invalid MultiMC path and version", Color.Red);
                else if (!IsMultiMCPathValid())
                    UpdateStatusLabel("Invalid MultiMC path", Color.Red);
                else if (versionListBox.SelectedItem == null)
                    UpdateStatusLabel("Select a version", Color.Red);
            }
        }

        private bool IsMultiMCPathValid()
        {
            // Check if the directory exists at the specified path
            return Directory.Exists(multiMCPathTextBox.Text);
        }

        private void UpdateStatusLabel(string text, Color color)
        {
            statusLabel.ForeColor = color;
            statusLabel.Text = text;
        }

        private async Task InstallModpackFile(string? downloadUrl, string filePath)
        {
            if (downloadUrl == null)
            {
                throw new ArgumentNullException(nameof(downloadUrl));
            }

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    long? contentLength = response.Content.Headers.ContentLength;
                    int bufferSize = 8192;
                    byte[] buffer = new byte[bufferSize];
                    long totalBytesRead = 0;
                    int bytesRead;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using (Stream downloadStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            UpdateProgress(totalBytesRead, contentLength, stopwatch);
                        }

                        // Check if the download is complete
                        if (totalBytesRead == contentLength)
                        {
                            await ExtractModpackFile(fileStream);
                        }
                    }
                }
            }
        }

        private async Task ExtractModpackFile(FileStream fileStream)
        {
            // Update the statusLabel text and color
            UpdateStatusLabel("Extracting Downloaded File", Color.Blue);
            // Extract the contents of the downloaded file to the instance folder
            string instancePath = ExtractDownloadedFile(fileStream.Name);
            // Install optifine
            if (optifineCheckBox.Checked)
            {
                await InstallOptifine(instancePath);
            }
            // Change pack settings
            ChangePackSetting(instancePath);
        }

        private async Task InstallOptifine(string instancePath)
        {
            UpdateStatusLabel("Downloading Optifine", Color.Blue);
            string optifineDownloadUrl = "https://drive.google.com/u/0/uc?id=1nR5ob2lm9RTMiczTlKiq5jCvvIFrhM_7&export=download";
            string optifineFilePath = Path.Combine(instancePath, ".minecraft", "mods", "OptiFine.jar");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(optifineDownloadUrl);
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


        private void ChangePackSetting(string instancePath)
        {
            // Set the instance configuration file path and content
            string instanceConfigPath = Path.Combine(instancePath, "instance.cfg");
            string[] instanceConfigLines = new string[]
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
            File.WriteAllLines(instanceConfigPath, instanceConfigLines);

            // Set the MMC pack file path and content
            string mmcPackFilePath = Path.Combine(instancePath, "mmc-pack.json");
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

        private void UpdateProgress(long totalBytesRead, long? contentLength, Stopwatch stopwatch)
        {
            double? percentComplete = contentLength.HasValue ? (totalBytesRead * 1d) / (contentLength * 1d) * 100 : 0;
            double bytesPerSecond = totalBytesRead / stopwatch.Elapsed.TotalSeconds;
            string speed = bytesPerSecond / 1024 / 1024 > 1 ? $"{bytesPerSecond / 1024 / 1024:F2} MB/s" : $"{bytesPerSecond / 1024:F2} KB/s";
            progressBar.Value = Math.Min((int)percentComplete, 100);

            // Update the progressLabel.Text only once every second
            if ((DateTime.Now - lastUpdateTime).TotalSeconds >= 1)
            {
                string progressText = $"{(int)percentComplete}% ({Math.Min(totalBytesRead, contentLength.GetValueOrDefault()) / (double)(1024 * 1024):F2} / {contentLength / (double)(1024 * 1024):F2} MB), Download speed: {speed}, ";

                TimeSpan remainingTime = TimeSpan.FromSeconds(3600);
                if (contentLength.HasValue && bytesPerSecond > 0)
                {
                    long remainingBytes = contentLength.Value - totalBytesRead;
                    double remainingSeconds = remainingBytes / bytesPerSecond;
                    remainingTime = TimeSpan.FromSeconds(remainingSeconds);
                    lastRemainingTime = remainingTime.ToString(@"hh\:mm\:ss");
                }
                progressText += $" Time remaining: {lastRemainingTime}";

                progressLabel.Text = progressText;

                lastUpdateTime = DateTime.Now;
            }
        }

        private string ExtractDownloadedFile(string filePath)
        {
            string instanceFolderPath = Path.Combine(multiMCPathTextBox.Text, "instances");

            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(filePath))
            {
                long totalBytesToExtract = zip.Sum(entry => entry.UncompressedSize);
                long totalBytesExtracted = 0;
                Stopwatch stopwatch = Stopwatch.StartNew();

                zip.ExtractProgress += (sender, e) =>
                {
                    if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
                    {
                        totalBytesExtracted += e.BytesTransferred;
                        UpdateProgress(totalBytesExtracted, totalBytesToExtract, stopwatch);
                    }
                };

                zip.ExtractAll(instanceFolderPath);
            }

            // Ensure that all streams are properly disposed of before attempting to delete the file
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete(filePath);

            // Return the path of the folder that was extracted from the jar file
            return Path.Combine(instanceFolderPath, $"GT New Horizons {versionListBox.SelectedItem.ToString()}");
        }
    }
}