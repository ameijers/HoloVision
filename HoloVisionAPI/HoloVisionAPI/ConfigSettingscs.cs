using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoloVisionAPI
{
    public class ConfigSettingscs
    {
        public static string VisionKey = "71b4a33b6f4647c693d1843f2066ab30";

        public static string VisionUrlParams = "visualFeatures=Tags";
        public static string VisionURL = string.Format("https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?{0}", VisionUrlParams);
    }
}
