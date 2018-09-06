using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using UnityEditor;

public class EditorHelper : EditorWindow
{
    [@MenuItem("Custom/Del Setting")]
    private static void _EditorHelper()
    {
        EditorHelper window = (EditorHelper)GetWindow(typeof(EditorHelper), true, "Del Setting");
        window.Show();
    }
    // 显示窗体里面的内容
    void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Remove archive"))
        {
            string path = UnityEngine.Application.persistentDataPath + geniusbaby.archive.SQLiteTableManager.save;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            //PlayerPrefs.DeleteKey(typeof(geniusbaby.LoginModule).Name);
            //PlayerPrefs.DeleteKey(typeof(geniusbaby.PlayerModule).Name);
            //PlayerPrefs.Save();
        }
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            //FindInSelected(g =>
            //    {
            //        go_count++;
            //        Component[] components = g.GetComponents<Component>();
            //        for (int i = 0; i < components.Length; i++)
            //        {
            //            components_count++;
            //            if (components[i] == null)
            //            {
            //                missing_count++;
            //                string s = g.name;
            //                Transform t = g.transform;
            //                while (t.parent != null)
            //                {
            //                    s = t.parent.name + "/" + s;
            //                    t = t.parent;
            //                }
            //                Debug.Log(s + " has an empty script attached in position: " + i, g);
            //            }
            //        }
            //    });
                        }
        if (GUILayout.Button("Output GuiFrame Cache List"))
        {
            var go = Selection.gameObjects;
            //if (go.Length != 1) { return; }
            //var root = new GameObject();
            //root.transform.parent = go[0].transform.parent;
            //root.transform.localPosition = go[0].transform.localPosition;
            //root.transform.localRotation = go[0].transform.localRotation;
            //root.transform.localScale = go[0].transform.localScale;
            //var mm = new Dictionary<Material, List<CombineInstance>>();
            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("frameCaches");
            doc.AppendChild(root);
            var sb = new StringBuilder();
            for (int index = 0; index < go.Length; ++index)
            {
                if (index == 0)
                {
                }
                else
                {
                    sb.Append(',');
                }
                sb.Append(go[index].name);
            }
            root.InnerText = sb.ToString();

            var os = new MemoryStream();
            doc.Save(os);
            //File.WriteAllBytes(Application.dataPath + "/StreamingAssets/FrameCache.xml", os.ToArray());
            File.WriteAllText(Application.dataPath + "/StreamingAssets/FrameCache.txt", sb.ToString());

            var txt = File.ReadAllText(Application.dataPath + "/StreamingAssets/FrameCache.txt");
            var frames = txt.Split(',');

        }
        GUILayout.EndVertical();
    }
    void Check(Transform g, System.Action<Transform> act)
    {
        act(g);
        for (int index = 0; index < g.transform.childCount; ++index)
        {
            Check(g.transform.GetChild(index), act);
        }
    }
}