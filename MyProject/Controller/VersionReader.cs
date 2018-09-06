using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public struct GameVersion
    {
        public int major;
        public int build;
        public int proto;
        public int config;
        public string SaveToString()
        {
            return major.ToString() +
                '.' + build.ToString() +
                '.' + proto.ToString() +
                '.' + config.ToString();
        }
        public bool LoadFromString(string buf)
        {
            try
            {
                var vs = buf.Split('.');
                major = int.Parse(vs[0]);
                build = int.Parse(vs[1]);
                proto = int.Parse(vs[2]);
                config = int.Parse(vs[3]);
                return true;
            }
            catch (System.Exception) { }
            return false;
        }
    }
    public struct VersionParam
    {
        public string meta;
        public string path;
    }
    public class VersionReader
    {
        const string tag_version = @"game_version";
        public static GameVersion version { get; private set; }
        public static readonly Util.Param1Actions<VersionReader> done = new Util.Param1Actions<VersionReader>();

        static VersionReader() { Load(); }

        public static void Load()
        {
            version = new GameVersion()
            {
                //major = SanguoApp.Instance.major,
                //build = SanguoApp.Instance.build,
                //proto = ProtoCfgFromString(SanguoApp.Instance.protocfg),
                //resource = SanguoApp.Instance.resource,
            };
            if (UnityEngine.PlayerPrefs.HasKey(tag_version))
            {
                string buf = UnityEngine.WWW.UnEscapeURL(UnityEngine.PlayerPrefs.GetString(tag_version));
                version.LoadFromString(buf);
            }
        }
        public static void Save(GameVersion newVersion)
        {
            version = newVersion;
            string buf = version.SaveToString();
            UnityEngine.PlayerPrefs.SetString(tag_version, UnityEngine.WWW.EscapeURL(buf));
        }
        public static void AfterFinished()
        {
            done.Fire(null);
        }
        static string ProtoCfgToString(int v)
        {
            return new StringBuilder().Append(v >> 16).Append('.').Append(v & 0xff).ToString();
        }
        static int ProtoCfgFromString(string str)
        {
            int v = 0;
            try
            {
                var ss = str.Split('.');
                return (int.Parse(ss[0]) << 16) + int.Parse(ss[1]);
            }
            catch (System.Exception) { }
            return v;
        }
    }
}
