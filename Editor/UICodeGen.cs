using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

class UICodeGen
{
    static string begin_tagString = @"//generate code begin";
    static string end_tagString = @"//generate code end";
    static string Frame_TypeString = @"Frame";
    static string Panel_TypeString = @"Panel";
    static string delare_template = "public {0} {1};";
    static string define_template = "{0} = transform.Find(\"{1}\").GetComponent<{2}>();";

    static string template =
@"using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using geniusbaby.pps;

namespace geniusbaby.LSharpScript
{
    public class ${name}${lsharptype} : ILSharpScript
    {
${begin_tag}
        ${declare}
        void __LoadComponet(Transform transform)
        {
            ${define}
        }
        void __DoInit()
        {
            ${variable}.OnInitialize(${ltype});
        }
        void __DoUninit()
        {
            ${variable}.OnUnInitialize();
        }
        void __DoShow()
        {
            ${variable}.OnShow();
        }
        void __DoHide()
        {
            ${variable}.OnHide();
        }
${end_tag}
        public override void OnInitialize(BehaviorWrapper api)
        {
            base.OnInitialize(api);
            __LoadComponet(api.transform);
        }
    }
}";

    class CodeWriter
    {
        public CodeWriter(String fileName)
        {
            m_fileNameString = fileName;
            m_bNeedMerge = false;
            if (checkExist())
            {
                try
                {
                    var lines = File.ReadAllLines(fileName);
                    for (int index = 0; index < lines.Length; ++index)
                    {
                        String line = lines[index];
                        if (line == null) { break; }
                        if (!m_bNeedMerge)
                        {
                            if (line.Trim() == begin_tagString) { m_bNeedMerge = true; }
                        }
                        m_oldCodesStrings.Add(line);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        public void write(String code)
        {
            m_newCodesStrings.Add(code);
        }
        public void flush()
        {
            if (m_bNeedMerge)
            {
                if (!checkDifference()) { return; }
                merge();
            }
            try
            {
                File.WriteAllLines(m_fileNameString, m_newCodesStrings.ToArray());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        private bool checkDifference()
        {
            //ListIterator<?> nb, ne, ob, oe;
            int nb = -1, ne = -1, ob = -1, oe = -1;
            if (string.IsNullOrEmpty(begin_tagString) || string.IsNullOrEmpty(end_tagString))
            {
                nb = 0; ne = m_newCodesStrings.Count;
                ob = 0; oe = m_oldCodesStrings.Count;
            }
            else
            {
                for (int index = 0; index < m_newCodesStrings.Count; index++)
                {
                    if (begin_tagString == m_newCodesStrings[index]) { nb = index; }
                    if (end_tagString == m_newCodesStrings[index]) { ne = index; break; }
                }

                for (int index = 0; index < m_oldCodesStrings.Count; index++)
                {
                    if (begin_tagString == m_oldCodesStrings[index]) { ob = index; }
                    if (end_tagString == m_oldCodesStrings[index]) { oe = index; break; }
                }
            }
            for (; nb != ne && ob != oe; ++nb, ++ob)
            {
                if (m_newCodesStrings[nb] != m_oldCodesStrings[ob])
                {
                    return true;
                }
            }
            return nb != ne || ob != oe;
        }
        private bool checkExist()
        {
            return File.Exists(m_fileNameString);
        }
        private void merge()
        {
            if (!string.IsNullOrEmpty(begin_tagString) && !string.IsNullOrEmpty(end_tagString))
            {
                int nb = m_newCodesStrings.Count, ne = 0;
                for (int itn = 0; itn < m_newCodesStrings.Count; ++itn)
                {
                    if (m_newCodesStrings[itn] == begin_tagString) { nb = itn; }
                    if (m_newCodesStrings[itn] == end_tagString) { ne = itn; break; }
                }
                if (ne <= nb) { return; }

                int ob = m_oldCodesStrings.Count, oe = 0;
                for (int ito = 0; ito < m_oldCodesStrings.Count; ++ito)
                {
                    if (m_oldCodesStrings[ito] == begin_tagString) { ob = ito; }
                    if (m_oldCodesStrings[ito] == end_tagString) { oe = ito; break; }
                }
                if (oe <= ob) { return; }

                List<String> sb = m_oldCodesStrings.GetRange(0, ob);
                List<String> sm = m_newCodesStrings.GetRange(nb, ne + 1 - nb);
                List<String> se = m_oldCodesStrings.GetRange(oe + 1, m_oldCodesStrings.Count - (oe + 1));
                List<String> tmp = new List<String>();
                tmp.AddRange(sb);
                tmp.AddRange(sm);
                tmp.AddRange(se);
                m_newCodesStrings = tmp;

                //			List<String> tmp = m_oldCodesStrings.subList(0, ob);
                //			tmp.addAll(m_newCodesStrings.subList(nb, ne + 1));
                //			tmp.addAll(m_oldCodesStrings.subList(oe + 1, m_oldCodesStrings.size()));
                //			m_oldCodesStrings = null;
                //			m_newCodesStrings = tmp;
            }
        }
        private String m_fileNameString;
        bool m_bNeedMerge;
        private List<String> m_newCodesStrings = new List<String>();
        private List<String> m_oldCodesStrings = new List<String>();
    }

    static string rootPath;

    [MenuItem("Custom/UIGen/Frame")]
    static public void FrameGen()
    {
        rootPath = EditorUtility.SaveFolderPanel("save uicode", Application.dataPath, string.Empty);
        var goes = Selection.gameObjects;
        for (int index = 0; index < goes.Length; ++index)
        {
            LGen(goes[index].transform, goes[index].name, Frame_TypeString, true);
        }
    }
    [MenuItem("Custom/UIGen/Panel")]
    static public void PanelGen()
    {
        rootPath = EditorUtility.SaveFolderPanel("save uicode", Application.dataPath, string.Empty);
        var goes = Selection.gameObjects;
        for (int index = 0; index < goes.Length; ++index)
        {
            LGen(goes[index].transform, goes[index].name, Panel_TypeString, false);
        }
    }
    class Variable
    {
        public string name;
        public string type;
        public string path;
        public string ltype;
    }
    static void LGen(Transform root, string name, string lsharptype, bool isFrame)
    {
        name = LSharpApiImpl.Regular(name);
        var id = name.IndexOf('_');
        if (id >= 0) { name = name.Substring(0, id); }
        name = RegularType(name);

        List<Variable> declare = new List<Variable>();
        List<Variable> variables = new List<Variable>();

        for (int index = 0; index < root.childCount; ++index)
        {
            var child = root.GetChild(index);
            Gen(child, declare, variables, null);
        }

        StringReader os = new StringReader(template);
        var cw = new CodeWriter(rootPath + (isFrame ? "/Frame/" : "/Panel/") + name + ".cs");
        while (true)
        {
            var line = os.ReadLine();
            if (line == null) { break; }

            if (line.Contains(@"${declare}"))
            {
                for (int index = 0; index < declare.Count; ++index)
                {
                    var decl = declare[index];
                    cw.write(line.Replace(@"${declare}", string.Format(delare_template, decl.type, decl.name)));
                }
            }
            else if (line.Contains(@"${define}"))
            {
                for (int index = 0; index < declare.Count; ++index)
                {
                    var decl = declare[index];
                    cw.write(line.Replace(@"${define}", string.Format(define_template, decl.name, decl.path, decl.type)));
                }
            }
            else if (line.Contains(@"${variable}"))
            {
                for (int index = 0; index < variables.Count; ++index)
                {
                    cw.write(line.Replace(@"${variable}", variables[index].name).Replace(@"${ltype}", variables[index].ltype));
                }
            }
            else
            {
                line = line.Replace(@"${name}", name);
                line = line.Replace(@"${lsharptype}", name.EndsWith(lsharptype) ? string.Empty : lsharptype);
                line = line.Replace(@"${begin_tag}", begin_tagString);
                line = line.Replace(@"${end_tag}", end_tagString);
                cw.write(line);
            }
        }
        cw.flush();
    }
    static void Gen(Transform transform, List<Variable> variables, List<Variable> behaviors, string parent)
    {
        var name = transform.name;//.Trim('_');
        var current = string.IsNullOrEmpty(parent) ? name : parent + "/" + name;

        //if (transform.name.StartsWith("_"))//ignore me
        //{
        //    for (int index = 0; index < transform.childCount; ++index)
        //    {
        //        Gen(transform.GetChild(index), declare, define, variables, current);
        //    }
        //}
        //else if (transform.name.StartsWith("__"))//ignore all
        //{
        //    return;
        //}
        if (!transform.name.StartsWith("@"))
        {
            for (int index = 0; index < transform.childCount; ++index)
            {
                Gen(transform.GetChild(index), variables, behaviors, current);
            }
        }
        else
        {
            var com = transform.GetComponent<BehaviorWrapper>();
            if (com)
            {
                if (com.GetType() == typeof(LSharpListContainer))
                {
                    var variable = new Variable()
                    {
                        name = LSharpApiImpl.Regular(current),
                        type = com.GetType().Name,
                        path = current,
                    };
                    variables.Add(variable);
                    behaviors.Add(variable);

                    var its = transform.GetComponentsInChildren<LSharpItemPanel>(true);
                    if (its.Length > 0)
                    {
                        var child = its[0];
                        LGen(child.transform, transform.name + "ItemPanel", Panel_TypeString, false);
                    }
                    for (int index = 0; index < transform.childCount; ++index)
                    {
                        var child = transform.GetChild(index);
                        if (!Array.Exists(its, it => it.transform == child))
                        {
                            Gen(child, variables, behaviors, current);
                        }
                    }
                    return;
                }
                else if (com.GetType() == typeof(LSharpAPI))
                {
                    var regularName = LSharpApiImpl.Regular(current);

                    var regularType = com.transform.name;
                    var id = regularType.IndexOf('_');
                    regularType = RegularType((id >= 0) ? regularType.Substring(0, id) : regularType);

                    var variable = new Variable()
                    {
                        name = regularName,
                        type = com.GetType().Name,
                        ltype = "\"geniusbaby.LSharpScript." + regularType + "\"",
                        path = current,
                    };
                    variables.Add(variable);
                    behaviors.Add(variable);

                    LGen(com.transform, regularType, string.Empty, false);
                    return;
                }
                else
                {
                    var variable = new Variable()
                    {
                        name = LSharpApiImpl.Regular(current),
                        type = com.GetType().Name,
                        path = current,
                    };
                    variables.Add(variable);
                    behaviors.Add(variable);
                }
            }
            else
            {
                if (TestType<TextImage2D>(transform, current, variables)) { }
                else if (TestType<TextImage3D>(transform, current, variables)) { }
                else if (TestType<DragGridCell>(transform, current, variables)) { }
                else if (TestType<GuiBar>(transform, current, variables)) { }
                else if (TestType<UnityEngine.UI.Slider>(transform, current, variables)) { }
                else if (TestType<UnityEngine.UI.Selectable>(transform, current, variables)) { }
                else if (TestType<UnityEngine.UI.Graphic>(transform, current, variables)) { }
                else if (TestType<HTMLEngine.UGUI.UGUIDemo>(transform, current, variables)) { }
                else { TestType<Transform>(transform, current, variables); }
            }
            for (int index = 0; index < transform.childCount; ++index)
            {
                Gen(transform.GetChild(index), variables, behaviors, current);
            }
        }
    }
    static bool TestType<T>(Transform transform, string current, List<Variable> variables) where T : Component
    {
        var comp = transform.GetComponent<T>();
        if (comp)
        {
            variables.Add(new Variable()
            {
                name = LSharpApiImpl.Regular(current),
                type = comp.GetType().Name,
                path = current,
            });
            return true;
        }
        return false;
    }
    static string RegularType(string name)
    {
        name = LSharpApiImpl.Regular(name);
        if (name.Length > 0)
        {
            return char.ToUpper(name[0]) + name.Substring(1, name.Length - 1);
        }
        return name;
    }
}