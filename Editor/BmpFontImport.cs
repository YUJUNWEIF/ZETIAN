using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

class BmpFontImport
{
    public struct Info
    {
        public string face;
        public int size;
        public int bold;
        public int italic;
        public string charset;
        public bool unicode;
        public int stretchH;
        public bool smooth;
        public int aa;
        public int paddingLeft, paddingRight, paddingTop, paddingBottom;
        public int spaceX, spaceY;
    }
    public struct Common
    {
        public int lineHeight;
        public int baseLine;
        public int width;
        public int height;
        public int pages;
        public bool packed;
    }
    public struct Page
    {
        public int id;
        public string file;
    }
    public struct CharInfo
    {
        public int id;
        public int x, y;
        public int width, height;
        public int xoffset, yoffset;
        public int xadvance;
        public int page;
        public int chnl;
    }
    static string GetString(List<KeyValuePair> kvs, string key)
    {
        return ReadValue(kvs, key).Replace("\"", string.Empty);
    }
    static string ReadValue(List<KeyValuePair> kvs, string key)
    {
        return kvs.Find(it => it.key == key).value;
    }
    static int GetInt(List<KeyValuePair> kvs, string key)
    {
        return int.Parse(ReadValue(kvs, key));
    }
    static bool GetBool(List<KeyValuePair> kvs, string key)
    {
        return int.Parse(ReadValue(kvs, key)) == 1;
    }

    struct KeyValuePair
    {
        public string key;
        public string value;
    }

    [MenuItem("Custom/TexturePacker/Process to BmpFont")]
    static public void Load()
    {
        TextAsset ta = (TextAsset)Selection.activeObject;

        Info info = new Info();
        Common common = new Common();
        Page page = new Page();
        List<CharInfo> charInfos = new List<CharInfo>();
        var separator = new char[] { ' ' };

        var lines = ta.text.Split('\r', '\n');
        for (int index = 0; index < lines.Length; ++index )
        {
            string line = lines[index];
            if (string.IsNullOrEmpty(line)) continue;
            string[] split = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            var kvs = new List<KeyValuePair>();
            for (int splitIndex = 1; splitIndex < split.Length; ++splitIndex )
            {
                var s = split[splitIndex];
                int idx = s.IndexOf('=');
                if (idx > 0)
                {
                    kvs.Add(new KeyValuePair() {key = s.Substring(0, idx), value = s.Substring(idx + 1)});
                }
            }

            switch (split[0])
            {
                case "info": info = LoadInfo(kvs); break;
                case "kerning": break;
                case "common": common = LoadCommon(kvs); break;
                case "page": page = LoadPage(kvs); break;
                case "chars": break;
                case "char": charInfos.Add(LoadChar(kvs)); break;
            }
        }

        string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ta));
        string fontPath = rootPath + "/" + info.face + @".fontsettings";
        string matPath = rootPath + "/" + info.face + @".mat";
        Font font = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;
        if (font == null)
        {
            font = new Font();
            AssetDatabase.CreateAsset(font, fontPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            //font = AssetDatabase.LoadAssetAtPath(rootPath + "/" + name, typeof(Font)) as Font;
        }
        else AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        if (font.material == null)
        {
            Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
            if (mat == null)
            {
                Shader shader = Shader.Find("GUI/Text Shader");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, matPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                //mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
            }
            else AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            font.material = mat;
        }

        font.name = info.face;
        font.characterInfo = ConvertTo(info, common, charInfos).ToArray();
        font.material.mainTexture = AssetDatabase.LoadAssetAtPath(rootPath + "/" + page.file, typeof(Texture2D)) as Texture2D;
      
        AssetDatabase.SaveAssets();
    }

    static List<CharacterInfo> ConvertTo(Info info, Common common, List<CharInfo> charInfos)
    {
        return charInfos.ConvertAll(it =>
        {
            CharacterInfo charInfo = new CharacterInfo();

            charInfo.index = it.id;
            charInfo.width = it.xadvance;
            charInfo.flipped = false;

            //charInfo.uv.x = it.x * 1f / common.width;
            //charInfo.uv.y = it.y * 1f / common.height;
            //charInfo.uv.width = it.width * 1f / common.width;
            //charInfo.uv.height = it.height * 1f / common.height;
            //charInfo.vert.x = it.xoffset;
            //charInfo.vert.y = -it.yoffset;
            //charInfo.vert.width = it.width;
            //charInfo.vert.height = -it.height;

            var uv = new Rect();
            uv.x = it.x * 1f / common.width;
            uv.y = it.y * 1f / common.height;
            uv.width = it.width * 1f / common.width;
            uv.height = it.height * 1f / common.height;
            uv.y = 1f - uv.y - uv.height;
            charInfo.uv = uv;

            var vert = new Rect();
            vert.x = it.xoffset;
            vert.y = it.yoffset;
            vert.width = it.width;
            vert.height = it.height;
            vert.y = info.size * 0.5f - vert.y;
            vert.height = -vert.height;
            charInfo.vert = vert;

            return charInfo;
        });
    }
    static Info LoadInfo(List<KeyValuePair> kvs) 
    {
        Info info = new Info();
        info.face = GetString(kvs, @"face");
        info.size = GetInt(kvs, @"size");
        info.bold = GetInt(kvs, @"bold");
        info.italic = GetInt(kvs, @"italic");
        info.charset = GetString(kvs, @"charset");
        info.unicode = GetBool(kvs, @"unicode");
        info.stretchH = GetInt(kvs, @"stretchH");
        info.smooth = GetBool(kvs, @"smooth");
        info.aa = GetInt(kvs, @"aa");

        var paddings = GetString(kvs, @"padding").Split(',');
        info.paddingLeft = int.Parse(paddings[0]);
        info.paddingRight = int.Parse(paddings[1]);
        info.paddingTop = int.Parse(paddings[2]);
        info.paddingBottom = int.Parse(paddings[3]);

        var spacings = GetString(kvs, @"spacing").Split(',');
        info.spaceX = int.Parse(spacings[0]);
        info.spaceY = int.Parse(spacings[1]);

        return info; 
    }
    //static bool LoadKerning(List<KeyValuePair> kvs)
    //{
    //        int first = GetInt(line[1]);
    //        int second = GetInt(line[2]);
    //        int amount = GetInt(line[3]);
    //        //BMGlyph glyph = font.GetGlyph(second, true);
    //        //if (glyph != null) glyph.SetKerning(first, amount);
    //}
    static Common LoadCommon(List<KeyValuePair> kvs) 
    {
        Common common = new Common();
        common.lineHeight = GetInt(kvs, @"lineHeight");
        common.baseLine = GetInt(kvs, @"base");;
        common.width = GetInt(kvs, @"scaleW");
        common.height = GetInt(kvs, @"scaleH");
        common.pages = GetInt(kvs, @"pages");
        common.packed = GetBool(kvs, @"packed");
        return common;
    }
    static Page LoadPage(List<KeyValuePair> kvs)
    {
        Page page = new Page();
        page.id = GetInt(kvs, @"id");
        page.file = GetString(kvs, @"file");
        return page;
    }
    static void LoadChars(List<KeyValuePair> kvs) { }
    static CharInfo LoadChar(List<KeyValuePair> kvs)
    {
        CharInfo charInfo = new CharInfo();
        charInfo.id = GetInt(kvs, @"id");
        charInfo.x = GetInt(kvs, @"x");
        charInfo.y = GetInt(kvs, @"y");
        charInfo.width = GetInt(kvs, @"width");
        charInfo.height = GetInt(kvs, @"height");
        charInfo.xoffset = GetInt(kvs, @"xoffset");
        charInfo.yoffset = GetInt(kvs, @"yoffset");
        charInfo.xadvance = GetInt(kvs, @"xadvance");
        charInfo.page = GetInt(kvs, @"page");
        charInfo.chnl = GetInt(kvs, @"chnl");
        return charInfo;
    }
}