using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNH_Manager
{
    internal class ModpackVersion
    {
        private static Dictionary<String, String> urlDict = new Dictionary<String, String>(); 
        static ModpackVersion()
        {
            urlDict.Add("2.3.5", "https://www.dropbox.com/scl/fi/r5ohaxwfzvyrphx2bn6zj/GT_New_Horizons_2.3.5_Client_Java_17-20.zip?dl=1&rlkey=spfjszkg02nrej3eqds42r0cg");
            urlDict.Add("2.3.4", "https://dl.dropboxusercontent.com/s/a58kvuhbpiuvey8/GT_New_Horizons_2.3.4_Java_17-20.zip");
            urlDict.Add("2.3.3", "https://dl.dropboxusercontent.com/s/ow8zc655ee7w3hj/GT_New_Horizons_2.3.3_Java_17-20.zip");
        }
        public static Dictionary<String, String> getUrlDict
        {
            get { return urlDict; }
        }
    }
}
