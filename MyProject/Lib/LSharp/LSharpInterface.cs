using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Pdb;

public struct Code
{
    public byte[] dll;
    public byte[] pdb;
}

public class LSharpInterface
{
    class Logger : CLRSharp.ICLRSharp_Logger//实现L#的LOG接口
    {
        public void Log(string str) { Debug.Log(str); }
        public void Log_Error(string str) { Debug.LogError(str); }
        public void Log_Warning(string str) { Debug.LogWarning(str); }
    }
    class DebugCLRType : CLRSharp.ICLRType
    {
        public CLRSharp.ICLRSharp_Environment env { get { return null; } }
        public string Name { get { return TypeForSystem.Name; } }
        public string FullName { get { return TypeForSystem.FullName; } }
        public string FullNameWithAssembly { get { return TypeForSystem.AssemblyQualifiedName; } }
        public System.Type TypeForSystem { get; private set; }
        public CLRSharp.IMethod GetMethod(string funcname, CLRSharp.MethodParamList types) { return null; }
        public CLRSharp.IMethod[] GetMethods(string funcname) { return null; }
        public CLRSharp.IMethod[] GetAllMethods() { return null; }
        public object InitObj() { return null; }
        public CLRSharp.IMethod GetMethodT(string funcname, CLRSharp.MethodParamList TTypes, CLRSharp.MethodParamList types) { return null; }
        public CLRSharp.IField GetField(string name) { return null; }
        public string[] GetFieldNames() { return null; }
        public bool IsInst(object obj) { return false; }
        public CLRSharp.ICLRType GetNestType(CLRSharp.ICLRSharp_Environment env, string fullname) { return null; }
        public CLRSharp.ICLRType[] SubTypes { get { return null; } }
        public bool IsEnum() { return false; }
        public DebugCLRType(Type type) { TypeForSystem = type; }
    }
    static CLRSharp.CLRSharp_Environment m_enviorment;
    static CLRSharp.ThreadContext m_context;
    static List<Assembly> m_assembly = new List<Assembly>();
    const int debugLevel = 0;
    public enum AssemblyType
    {
        LSharp = 1,
        Dll = 2,
        Self = 3,
    }
    public static AssemblyType useLSharp { get { return AssemblyType.Self; } }
    public static void StartUp(List<Code> codes)
    {
        for (int index = 0; index < codes.Count; ++index)
        {
            switch (useLSharp)
            {
                case AssemblyType.LSharp:
                    if (index == 0)
                    {
                        m_enviorment = new CLRSharp.CLRSharp_Environment(new Logger());
                        m_context = new CLRSharp.ThreadContext(m_enviorment, debugLevel);
                    }
                    var msDll = new MemoryStream(codes[index].dll);
                    var msPdb = new MemoryStream(codes[index].pdb);
                    m_enviorment.LoadModule(msDll, null, new Mono.Cecil.Mdb.MdbReaderProvider());
                    break;
                case AssemblyType.Dll:
                    //m_assembly.Add(Assembly.Load(codes[index].dll, codes[index].pdb));
                    m_assembly.Add(Assembly.Load(codes[index].dll));//, codes[index].pdb));
                    break;
            }
        }
    }
    public static CLRSharp.ICLRType GetType(string fullName)
    {
        switch (useLSharp)
        {
            case AssemblyType.LSharp: return m_enviorment.GetType(fullName);//用全名称，包括命名空间
            case AssemblyType.Dll:
                for (int index = 0; index < m_assembly.Count; ++index)
                {
                    var type = m_assembly[index].GetType(fullName);
                    if (type != null) { return new DebugCLRType(type); }
                }
                return null;
            default:
                var assembly = Assembly.GetExecutingAssembly();
                var t = assembly.GetType(fullName);
                if (t != null) { return new DebugCLRType(t); }
                return null;
        }
    }
    public static object InstanceInvoke(CLRSharp.ICLRType m_clrType, object m_scriptObj, string methodName, params object[] objs)
    {
        if (useLSharp == AssemblyType.LSharp)
        {
            if (objs.Length > 0)
            {
                var clrTypes = Array.ConvertAll(objs, it => m_enviorment.GetType(it.GetType()));
                var method = m_clrType.GetMethod(methodName, CLRSharp.MethodParamList.Make(clrTypes));
                return (method != null) ? method.Invoke(m_context, m_scriptObj, objs, true, true) : null;
            }
            else
            {
                var method = m_clrType.GetMethod(methodName, CLRSharp.MethodParamList.constEmpty());
                return (method != null) ? method.Invoke(m_context, m_scriptObj, null, true, true) : null;
            }
        }
        else
        {
            var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var method = m_clrType.TypeForSystem.GetMethod(methodName, flag);
            return method.Invoke(m_scriptObj, objs);
        }
    }
    public static object StaticInvoke(CLRSharp.ICLRType m_clrType, string methodName, params object[] objs)
    {
        if (useLSharp == AssemblyType.LSharp)
        {
            if (objs.Length > 0)
            {
                var clrTypes = Array.ConvertAll(objs, it => m_enviorment.GetType(it.GetType()));
                var method = m_clrType.GetMethod(methodName, CLRSharp.MethodParamList.Make(clrTypes));
                return (method != null) ? method.Invoke(m_context, null, objs, true, true) : null;
            }
            else
            {
                var method = m_clrType.GetMethod(methodName, CLRSharp.MethodParamList.constEmpty());
                return (method != null) ? method.Invoke(m_context, null, null, true, true) : null;
            }
        }
        else
        {
            if (methodName == ".ctor")
            {
                if (objs.Length > 0)
                {
                    return Activator.CreateInstance(m_clrType.TypeForSystem, objs);
                }
                else
                {
                    return Activator.CreateInstance(m_clrType.TypeForSystem);
                }
            }
            else
            {
                var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                var method = m_clrType.TypeForSystem.GetMethod(methodName, flag);
                return method.Invoke(null, objs);
            }
        }
    }
}