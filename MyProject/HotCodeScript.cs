using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public interface IFormulaLoader
    {
        object InstanceInvoke(string methodName, params object[] args);
        object StaticInvoke(string methodName, params object[] args);
    }
    public class HotcodeEntry : IFormulaLoader
    {
        CLRSharp.ICLRType m_type;
        object m_obj;
        static HotcodeEntry m_entry;
        HotcodeEntry()
        {
            m_type = LSharpInterface.GetType(@"geniusbaby.LSharpScript.HotcodeFramework");
            m_obj = LSharpInterface.StaticInvoke(m_type, @".ctor");
        }
        public object InstanceInvoke(string methodName, params object[] args)
        {
            var obj = LSharpInterface.InstanceInvoke(m_type, m_obj, methodName, args);
            var box = (obj as CLRSharp.VBox);
            if (box != null)
            {
                return box.BoxDefine();
            }
            return obj;
        }
        public object StaticInvoke(string methodName, params object[] args)
        {
            var obj = LSharpInterface.StaticInvoke(m_type, methodName, args);
            var box = (obj as CLRSharp.VBox);
            if (box != null)
            {
                return box.BoxDefine();
            }
            return obj;
        }
        public static void StartGame()
        {
            m_entry = new HotcodeEntry();
            m_entry.InstanceInvoke("StartGame");
        }
        public static void StopGame()
        {
            if (m_entry != null)
            {
                m_entry.InstanceInvoke("StopGame");
                m_entry = null;
            }
        }
        public static void EnterGame()
        {
            m_entry.InstanceInvoke("EnterGame");
        }
        public static void OnClick(IBaseObj building)
        {
            m_entry.InstanceInvoke("OnClick", building);
        }
    }
}