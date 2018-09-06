using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace geniusbaby.tab
{
    public class CGReader : FileReader
    {
        public override byte[] Load(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName;
            if (//Application.platform != RuntimePlatform.WindowsEditor &&
                //Application.platform != RuntimePlatform.OSXEditor &&
                File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    using (var www = new WWW(Application.streamingAssetsPath + "/" + fileName))
                    {
                        while (!www.isDone) { }
                        return www.bytes;
                    }
                }
                else
                {
                    return File.ReadAllBytes(Application.streamingAssetsPath + "/" + fileName);
                }
            }
        }
        public static void ParseZip(string fileName, Action<string, byte[]> parser)
        {
            new CGReader().Parse(fileName, parser);
        }
        public static byte[] LoadFile(string fileName)
        {
            return new CGReader().Load(fileName);
        }
    }
}
