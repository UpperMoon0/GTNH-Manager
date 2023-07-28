using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using Ionic.Zip;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace GTNH_Manager
{
    public partial class GTNHManager : Form
    {
        string version = "0.1";
        public GTNHManager()
        {
            InitializeComponent();
            versionListBox.Items.Add("2.3.5 BETA");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // This method is called when the downloadButton is clicked
        private async void downloadButton_Click(object sender, EventArgs e)
        {
            // Check if the path specified in the multiMCPathTextBox is valid
            if (IsMultiMCPathValid() && versionListBox.SelectedItem != null)
            {
                // Check if an item is selected in the versionListBox and if the selected item is "2.3.5 BETA"
                if (versionListBox.SelectedItem != null && versionListBox.SelectedItem.ToString() == "2.3.5 BETA")
                {
                    // Set the URL of the file to download
                    string url = "https://www.dropbox.com/scl/fi/r5ohaxwfzvyrphx2bn6zj/GT_New_Horizons_2.3.5_Client_Java_17-20.zip?dl=1&rlkey=spfjszkg02nrej3eqds42r0cg";
                    // Set the path of the temporary folder
                    string tempPath = Path.GetTempPath();
                    // Set the path of the file to save in the temporary folder
                    string filePath = Path.Combine(tempPath, "GT_New_Horizons_2.3.5_Client_Java_17-20.zip");
                    // Update the statusLabel text and color
                    UpdateStatusLabel("Downloading the modpack", Color.Blue);

                    // Create a new HttpClient to download the file
                    using (HttpClient client = new HttpClient())
                    {
                        // Send a GET request to the specified URL
                        HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        // Check if the response was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Get the content length of the response
                            long? contentLength = response.Content.Headers.ContentLength;
                            // Get the content stream of the response
                            using (Stream stream = await response.Content.ReadAsStreamAsync())
                            // Create a new FileStream to save the content to a file
                            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                // Download the file and track its progress
                                await InstallFile(stream, fileStream, contentLength);
                            }
                        }
                    }
                    progressLabel.Text = "";
                    UpdateStatusLabel("Installation complete", Color.Green);
                }
            }
            if (!IsMultiMCPathValid() && versionListBox.SelectedItem == null)
                UpdateStatusLabel("Invalid MultiMC path and version", Color.Red);
            else if (!IsMultiMCPathValid())
                UpdateStatusLabel("Invalid MultiMC path", Color.Red);
            else if (versionListBox.SelectedItem == null)
                UpdateStatusLabel("Select a version", Color.Red);   
        }

        // This method checks if the path specified in the multiMCPathTextBox is valid
        private bool IsMultiMCPathValid()
        {
            // Check if the directory exists at the specified path
            if (Directory.Exists(multiMCPathTextBox.Text))
                return true;
            else
                return false;
        }

        // This method updates the text and color of the statusLabel
        private void UpdateStatusLabel(string text, Color color)
        {
            statusLabel.ForeColor = color;
            statusLabel.Text = text;
        }

        //Download and install the file
        private async Task InstallFile(Stream stream, FileStream fileStream, long? contentLength)
        {
            int bufferSize = 8192;
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while ((bytesRead = await stream.ReadAsync(buffer, 0, bufferSize)) != 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                UpdateProgress(totalBytesRead, contentLength, stopwatch);
            }

            // Ensure that the FileStream is properly disposed of before attempting to extract its contents
            fileStream.Dispose();

            // Check if the download is complete
            if (totalBytesRead == contentLength)
            {
                // Update the statusLabel text and color
                UpdateStatusLabel("Creating Instance Folder", Color.Blue);
                // Update the statusLabel text and color
                UpdateStatusLabel("Extracting Downloaded File", Color.Blue);
                // Extract the contents of the downloaded file to the instance folder
                string instancePath = ExtractDownloadedFile(fileStream.Name);
                // Install optifine
                if (optifineCheckBox.Checked)
                {
                    UpdateStatusLabel("Downloading Optifine", Color.Blue);
                    string optifineDownloadUrl = "https://drive.google.com/u/0/uc?id=1nR5ob2lm9RTMiczTlKiq5jCvvIFrhM_7&export=download";
                    string optifineFilePath = Path.Combine(instancePath, ".minecraft", "mods", "OptiFine.jar");
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(optifineDownloadUrl);
                        response.EnsureSuccessStatusCode();
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                opFileStream = new FileStream(optifineFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
                        {
                            totalBytesRead = 0;
                            contentLength = response.Content.Headers.ContentLength;
                            stopwatch.Restart();
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                            {
                                await opFileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;
                                UpdateProgress(totalBytesRead, contentLength, stopwatch);
                            }
                        }
                    }
                }
                // Change pack settings
                ChangePackSetting(instancePath);
            }
        }


        //Change the pack setting
        private void ChangePackSetting(string instancePath)
        {
            // Write to instance.cfg
            string instanceCfgPath = Path.Combine(instancePath, "instance.cfg");
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
                "name=" + versionListBox.SelectedItem.ToString(),
                "notes="
            };
            File.WriteAllLines(instanceCfgPath, instanceCfgLines);

            // Write to mmc-pack.json
            // Don't indent the string line
            string mmcPackJsonPath = Path.Combine(instancePath, "mmc-pack.json");
            string mmcPackJsonContent = @"{
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
            File.WriteAllText(mmcPackJsonPath, mmcPackJsonContent);
        }

        // This method updates the download progress bar and label with the download progress
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private string _lastRemainingTime = "";

        // This method updates the download progress bar and label with the download progress
        private void UpdateProgress(long totalBytesRead, long? contentLength, Stopwatch stopwatch)
        {
            int percentComplete = 0;
            if (contentLength.HasValue)
            {
                percentComplete = (int)((totalBytesRead * 1d) / (contentLength * 1d) * 100);
            }
            double bytesPerSecond = totalBytesRead / stopwatch.Elapsed.TotalSeconds;
            string speed;
            if (bytesPerSecond / 1024 / 1024 > 1)
            {
                speed = $"{bytesPerSecond / 1024 / 1024:F2} MB/s";
            }
            else
            {
                speed = $"{bytesPerSecond / 1024:F2} KB/s";
            }
            if (percentComplete > 100)
                percentComplete = 100;
            progressBar.Value = percentComplete;

            // Update the progressLabel.Text only once every second
            if ((DateTime.Now - _lastUpdateTime).TotalSeconds >= 1)
            {
                string progressText = $"{percentComplete}% ({Math.Min(totalBytesRead, contentLength.GetValueOrDefault()) / 1024 / 1024:F2} / {contentLength / 1024 / 1024:F2} MB), Download speed: {speed}, ";

                TimeSpan remainingTime = TimeSpan.FromSeconds(3600);
                if (contentLength.HasValue && bytesPerSecond > 0)
                {
                    long remainingBytes = contentLength.Value - totalBytesRead;
                    double remainingSeconds = remainingBytes / bytesPerSecond;
                    remainingTime = TimeSpan.FromSeconds(remainingSeconds);
                    _lastRemainingTime = remainingTime.ToString(@"hh\:mm\:ss");
                }
                progressText += $" Time remaining: {_lastRemainingTime}";

                progressLabel.Text = progressText;

                _lastUpdateTime = DateTime.Now;
            }
        }

        // This method extracts the contents of the downloaded file to the instance folder, deletes the downloaded file, and returns the path of the folder that was extracted from the jar file
        private string ExtractDownloadedFile(string filePath)
        {
            string instancePath = Path.Combine(multiMCPathTextBox.Text, "instances");

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

                zip.ExtractAll(instancePath);
            }

            // Ensure that all streams are properly disposed of before attempting to delete the file
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete(filePath);

            // Return the path of the folder that was extracted from the jar file
            return Path.Combine(instancePath, "GT New Horizons " + GetSelectedModpackVersionNumber());
        }

        // Get selected modpack version number
        private string? GetSelectedModpackVersionNumber()
        {
            string? version = versionListBox.SelectedItem.ToString();
            if (version != null)
            {
                int index = version.IndexOf(" ");
                if (index != -1)
                {
                    version = version.Substring(0, index);
                }
                return version;
            }
            return null;
        }
    }
}