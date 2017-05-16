using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    public static class Utils
    {
        public static void SaveAppStore(string path, Intel4004AppStore appStore)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(appStore));
        }

        public static Intel4004AppStore ReadAppStore(string path)
        {
            return JsonConvert.DeserializeObject<Intel4004AppStore>(File.ReadAllText(path));
        }

        public static void SaveApp(string path, Intel4004App app)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(app));
        }

        public static Intel4004App ReadApp(string path)
        {
            return JsonConvert.DeserializeObject<Intel4004App>(File.ReadAllText(path));
        }

        public static BitArray DataToBitArray(string data)
        {
            throw new NotImplementedException();
        }

        public static string BitArrayToData(BitArray data)
        {
            throw new NotImplementedException();
        }
    }
}
