using UnityEngine;
using System;
using System.IO;
using System.Text;

public struct IpAddr
{
    public string ip;
    public int port;
    public static IpAddr Parse(string str)
    {
        var splits = str.Split(':');
        return new IpAddr() { ip = splits[0], port = int.Parse(splits[1]) };
    }
}

public class GamePath
{
    public class Asset
    {
        public struct GameObj
        {
            public string anim;
            public string model;
            public string material;
            public string prefab;
            public string texture;
        }
        public struct UI
        {
            public string frame;
            public string panel;
        }
        public GameObj fish;
        public GameObj pet;
        public GameObj gun;
        public GameObj bullet;
        public GameObj alphabet;
        public UI ui;
        public string audio;
        public string fxUI;
        public string fx3D;
        public string terrain;
        public string font;
        public string background;
        public string[] atlas;
    }
    public class Music
    {
        public string musicVersion;
        public string musicLogin;
        public string musicMain;
        public string musicCombat;
        public string musicCombatVictor;
        public string musicCombatDefeat;

        public string soundClickBuilding;
        public string soundCloseBuilding;
        public string soundUpgradeBuilding;
        public string soundClickButton;
    }

    public class Manifest
    {
        public string cfg;
        public string art;
        public string lgap;
    }
    public class Net
    {
        public int keepAlive;
    }
    public class Url
    {
        public string logicURL;
        public string updateURL;
        public string noticeURL;
        public IpAddr udpAddr;
    }
    public class Debug
    {
        public bool debugMode;
        public bool script;
        public bool guideUseDebug;
        public int guideDebugStep;
    }
    public static Net net = new Net();
    public static Url url = new Url();
    public static Debug debug = new Debug();
    public static Asset asset = new Asset();
    public static Music music = new Music();
    public static Manifest manifest = new Manifest();

    public static string GetBundlePath(string assetPath)
    {
        bool debug =
            Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor;
        return (debug ? Application.persistentDataPath : Application.dataPath) + assetPath;
    }
    static GamePath()
    {
        byte[] bytes = null;
        try
        {
            var configPath = Application.streamingAssetsPath + "/GamePath.xml";
            if (Application.platform == RuntimePlatform.WindowsEditor &&
                Application.platform == RuntimePlatform.OSXEditor)
            {
                bytes = File.ReadAllBytes(configPath);
            }
            else
            {
                try
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            using (WWW www = new WWW(configPath))
                            {
                                while (!www.isDone) { }
                                bytes = www.bytes;
                            }
                            break;
                        default:
                            bytes = File.ReadAllBytes(configPath);
                            break;
                    }
                }
                catch (Exception ex) { Util.Logger.Instance.Error(ex.Message, ex); }
            }
            
            var parser = new Util.XMLParser();
            var doc = parser.Parse(Encoding.UTF8.GetString(bytes));
            {
                var current = doc.SelectSingleNode("application/platform");
                var platform = current.GetAttribute(@"current");
                var version = current.GetAttribute(@"version");

                var specify = doc.SelectSingleNode("application/platform_" + platform);
                url.logicURL = specify.GetAttribute(@"url").Replace("${version}", version);
                url.updateURL = specify.GetAttribute(@"update").Replace("${version}", version);
                url.noticeURL = specify.GetAttribute(@"notice").Replace("${version}", version);
                url.udpAddr = IpAddr.Parse(specify.GetAttribute(@"udp").Replace("${version}", version));
            }

            {
                var specify = doc.SelectSingleNode("application/net");
                net.keepAlive = int.Parse(specify.GetAttribute(@"keepAlive"));
            }

            {
                var specify = doc.SelectSingleNode("application/debug");
                debug.debugMode = bool.Parse(specify.GetAttribute(@"debugMode"));
                debug.script = bool.Parse(specify.GetAttribute(@"script"));
                debug.guideUseDebug = bool.Parse(specify.GetAttribute(@"guideUseDebug"));
                debug.guideDebugStep = int.Parse(specify.GetAttribute(@"guideDebugStep"));
            }

            {
                var ui = doc.SelectSingleNode("application/asset/ui");
                GamePath.asset.ui.frame = ui.GetAttribute(@"frame");
                GamePath.asset.ui.panel = ui.GetAttribute(@"panel");
            }

            {
                var current = doc.SelectSingleNode("application/manifest");
                GamePath.manifest.cfg = current.GetAttribute(@"cfg");
                GamePath.manifest.art = current.GetAttribute(@"art");
                GamePath.manifest.lgap = current.GetAttribute(@"lgap");
            }

            {
                GamePath.asset.fish = Convert(doc, "application/asset/fish");
                GamePath.asset.pet = Convert(doc, "application/asset/pet");
                GamePath.asset.gun = Convert(doc, "application/asset/gun");
                GamePath.asset.bullet = Convert(doc, "application/asset/bullet");
                GamePath.asset.alphabet = Convert(doc, "application/asset/alphabet");

                GamePath.asset.audio = doc.SelectSingleNode("application/asset/audio").GetAttribute(@"path");
                GamePath.asset.fxUI = doc.SelectSingleNode("application/asset/fxUI").GetAttribute(@"path");
                GamePath.asset.fx3D = doc.SelectSingleNode("application/asset/fx3D").GetAttribute(@"path");
                GamePath.asset.terrain = doc.SelectSingleNode("application/asset/terrain").GetAttribute(@"path");
                GamePath.asset.font = doc.SelectSingleNode("application/asset/font").GetAttribute(@"path");
                GamePath.asset.background = doc.SelectSingleNode("application/asset/background").GetAttribute(@"path");

                var atlastr = (doc.SelectSingleNode("application/asset/sprite")).GetAttribute(@"path");
                GamePath.asset.atlas = atlastr.Split(',');
            }

            {
                GamePath.music.musicVersion = doc.SelectSingleNode("application/audio/version").GetAttribute(@"music");
                GamePath.music.musicLogin = doc.SelectSingleNode("application/audio/login").GetAttribute(@"music");
                GamePath.music.musicMain = doc.SelectSingleNode("application/audio/main").GetAttribute(@"music");
                GamePath.music.musicCombat = doc.SelectSingleNode("application/audio/combat").GetAttribute(@"music");
                GamePath.music.musicCombatVictor = doc.SelectSingleNode("application/audio/combatVictor").GetAttribute(@"music");
                GamePath.music.musicCombatDefeat = doc.SelectSingleNode("application/audio/combatDefeat").GetAttribute(@"music");

                GamePath.music.soundClickBuilding = doc.SelectSingleNode("application/audio/clickBuilding").GetAttribute(@"sound");
                GamePath.music.soundCloseBuilding = doc.SelectSingleNode("application/audio/closeBuilding").GetAttribute(@"sound");
                GamePath.music.soundUpgradeBuilding = doc.SelectSingleNode("application/audio/upgradeBuilding").GetAttribute(@"sound");
                GamePath.music.soundClickButton = doc.SelectSingleNode("application/audio/clickButton").GetAttribute(@"sound");
            }
        }
        catch (Exception ex) { Util.Logger.Instance.Error(ex.Message, ex); }
    }

    static Asset.GameObj Convert(Util.XMLNode root, string node)
    {
        var no = root.SelectSingleNode(node);
        return new Asset.GameObj()
        {
            anim = no.GetAttribute("anim"),
            model = no.GetAttribute("model"),
            material = no.GetAttribute("material"),
            prefab = no.GetAttribute("prefab"),
            texture = no.GetAttribute("texture"),
        };
    }
}