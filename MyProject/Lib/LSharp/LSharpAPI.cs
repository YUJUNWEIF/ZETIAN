using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Pdb;

public interface ILSharpApi
{
    object hotfixScript { get; }
    object InstanceInvoke(string methodName, params object[] objs);
    object StaticInvoke(string methodName, params object[] objs);
}

public class LSharpApiImpl
{
    public static string Regular(string name)
    {
        name = name.Replace('/', '_').Replace('\\', '_');

        var sb = new StringBuilder();
        for (int index = 0; index < name.Length; ++index)
        {
            if (ValidChar(name[index]))
            {
                sb.Append(name[index]);
            }
            else if (name[index] == '@') { }
            else if (name[index] == '/' || name[index] == '\\')
            {
                sb.Append('_');
            }
            else
            {
                throw new Exception("invalid name : " + name);
            }
        }
        return sb.ToString();
    }
    static bool ValidChar(char ch)
    {
        return ch == '_' || ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
    }
    CLRSharp.ICLRType m_clrType;
    public object hotfixScript { get; private set; }
    internal void OnInitialize(string name, BehaviorWrapper api)
    {
        if (hotfixScript != null)
        {
            Util.Logger.Instance.Error("duplicate init, check logic");
            return;
        }
        m_clrType = LSharpInterface.GetType(name);//用全名称，包括命名空间
        if (m_clrType != null) hotfixScript = StaticInvoke(".ctor");//执行构造函数
        InstanceInvoke("OnInitialize", api);
    }
    public virtual void OnUnInitialize()
    {
        InstanceInvoke("OnUnInitialize");
    }
    public virtual void OnShow()
    {
        InstanceInvoke("OnShow");
    }
    public virtual void OnHide()
    {
        InstanceInvoke("OnHide");
    }
    public object InstanceInvoke(string methodName, params object[] objs)
    {
        return m_clrType != null ? LSharpInterface.InstanceInvoke(m_clrType, hotfixScript, methodName, objs) : null;
    }
    public object StaticInvoke(string methodName, params object[] objs)
    {
        return m_clrType != null ? LSharpInterface.StaticInvoke(m_clrType, methodName, objs) : null;
    }
}
public class LSharpAPI : BehaviorWrapper
{
    LSharpApiImpl impl = new LSharpApiImpl();
    public object hotfixScript { get { return impl.hotfixScript; } }
    public override void OnInitialize()
    {
        OnInitialize("geniusbaby.LSharpScript." + LSharpApiImpl.Regular(name));
    }
    public void OnInitialize(string name)
    {
        base.OnInitialize();
        impl.OnInitialize(name, this);
    }
    public override void OnUnInitialize()
    {
        impl.OnUnInitialize();
        base.OnUnInitialize();
    }
    public override void OnShow()
    {
        base.OnShow();
        impl.OnShow();
    }
    public override void OnHide()
    {
        impl.OnHide();
        base.OnHide();
    }
    public object InstanceInvoke(string methodName, params object[] objs)
    {
        return impl.InstanceInvoke(methodName, objs);
    }
    public object StaticInvoke(string methodName, params object[] objs)
    {
        return impl.StaticInvoke(methodName, objs);
    }
}