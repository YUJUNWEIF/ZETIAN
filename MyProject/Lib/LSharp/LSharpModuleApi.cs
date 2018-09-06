using System;
using System.Collections.Generic;

    public class LSharpModuleAPI
    {
        public static object InstanceInvoke(string moduleName, string methodName, params object[] objs)
        {
            CLRSharp.ICLRType m_clrType = LSharpInterface.GetType("HotFixCode." + moduleName);//用全名称，包括命名空间
            object m_scriptObj = LSharpInterface.StaticInvoke(m_clrType, "Inst");//获取单键实例
            return LSharpInterface.InstanceInvoke(m_clrType, m_scriptObj, methodName, objs);
        }
        public static object StaticInvoke(string moduleName, string methodName, params object[] objs)
        {
            CLRSharp.ICLRType m_clrType = LSharpInterface.GetType("HotFixCode." + moduleName);//用全名称，包括命名空间
            return LSharpInterface.StaticInvoke(m_clrType, methodName, objs);
        }
    }
