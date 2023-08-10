namespace GTNH_Manager
{
    internal class ModpackVersion
    {
        private static Dictionary<String, String> urlDict = new Dictionary<String, String>(); 
        private const string optifineDownloadUrl = "https://drive.google.com/u/0/uc?id=1nR5ob2lm9RTMiczTlKiq5jCvvIFrhM_7&export=download";
        static ModpackVersion()
        {
            urlDict.Add("2.3.6", "https://dl.dropboxusercontent.com/scl/fi/qhtjdjoolozofjht2qrks/GT_New_Horizons_2.3.6_Client_Java_17-20.zip?dl=0&rlkey=z58bagc14k6ij7bxdhrue9sex");
            urlDict.Add("2.3.5", "https://www.dropbox.com/scl/fi/r5ohaxwfzvyrphx2bn6zj/GT_New_Horizons_2.3.5_Client_Java_17-20.zip?dl=1&rlkey=spfjszkg02nrej3eqds42r0cg");
            urlDict.Add("2.3.4", "https://dl.dropboxusercontent.com/s/a58kvuhbpiuvey8/GT_New_Horizons_2.3.4_Java_17-20.zip");
            urlDict.Add("2.3.3", "https://dl.dropboxusercontent.com/s/ow8zc655ee7w3hj/GT_New_Horizons_2.3.3_Java_17-20.zip");
        }
        public static Dictionary<String, String> getUrlDict
        {
            get { return urlDict; }
        }

        public static string OptifineDownloadUrl => optifineDownloadUrl;
    }
}
