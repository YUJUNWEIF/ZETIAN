//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//public class ParticlesReplacement : EditorWindow
//{
//    Shader shader_mo_1 = Shader.Find("Mobile/Diffuse");
//    Shader shader_mo_2 = Shader.Find("Mobile/Particles/Additive");
//    Shader shader_mo_3 = Shader.Find("Mobile/Particles/Alpha Blended");

//    Shader shader_pc_1 = Shader.Find("Diffuse");
//    Shader shader_pc_2 = Shader.Find("Particles/Additive");
//    Shader shader_pc_3 = Shader.Find("Particles/Alpha Blended");

//    //[@MenuItem("CUSTOM/Moble Material Setting")]
//    //private static void Init()
//    //{
//    //    ParticlesReplacement window = (ParticlesReplacement)GetWindow(typeof(ParticlesReplacement), true, "MobleMaterialSetting");
//    //    window.Show();
//    //}

//    // 显示窗体里面的内容
//    private void OnGUI()
//    {
//        GUILayout.BeginHorizontal();
//        //GUILayout.Label("Moble Material Setting");
//        //GUILayout.EndHorizontal();

//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("Diffuse -> Mobile/Diffuse"))
//        //    LoopSetMaterials(shader_pc_1, shader_mo_1);

//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("Particles/Additive -> Mobile/Particles/Additive"))
//        //    LoopSetMaterials(shader_pc_2, shader_mo_2);

//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("Particles/Alpha Blended -> Mobile/Particles/Alpha Blended"))
//        //    LoopSetMaterials(shader_pc_3, shader_mo_3);

//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("UILayout/Set Top/Center/Bottom Z value "))
//        //    LoopSetFrameTopCenterZ();

//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("UILayout/ModifyGuiList!"))
//        //    ModifyGuiList();
//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("UILayout/ModifyGuiListItem!"))
//        //    ModifyGuiListItem();

//        //GUILayout.Label(" ");
//        //if (GUILayout.Button("PlayerPrefs.DeleteAll()"))
//        //{
//        //    PlayerPrefs.DeleteKey("game_version");
//        //    PlayerPrefs.Save();
//        //}

//        GUILayout.Label(" ");
//        if (GUILayout.Button("Remove Missed Script"))
//        {
//            PlayerPrefs.DeleteKey("game_version");
//            PlayerPrefs.Save();
//        }
//    }

//    private void LoopSetMaterials(Shader old_shader, Shader new_shader)
//    {
//        Object[] materials = GetSelectedMaterials();
//        Selection.objects = new Object[0];

//        foreach (Material m in materials)
//        {
//            if (m.shader == old_shader)
//            {
//                m.shader = new_shader;
//            }
//        }
//    }

//    private Object[] GetSelectedMaterials()
//    {
//        return Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
//    }
//    static bool IsSubclassOfRawGeneric(System.Type parent, System.Type child)
//    {
//        if (child != null && child != typeof(object))
//        {
//            var cur = child.IsGenericType ? child.GetGenericTypeDefinition() : child;
//            if (parent == cur)
//            {
//                return true;
//            }
//            if (IsSubclassOfRawGeneric(parent, child.BaseType)) return true;
//            var ips = child.GetInterfaces();
//            foreach (var ip in ips)
//            {
//                if (IsSubclassOfRawGeneric(parent, ip))
//                {
//                    return true;
//                }
//            }
//        }
//        return false;
//    }
//}
using UnityEngine;
using UnityEditor;
public class FindMissingScriptsRecursively : EditorWindow
{
    static int go_count = 0, components_count = 0, missing_count = 0;

    [MenuItem("Window/FindMissingScriptsRecursively")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected();
        }
    }
    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }

    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log(s + " has an empty script attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject);
        }
    }
}