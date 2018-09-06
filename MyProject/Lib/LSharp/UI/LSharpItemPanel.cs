using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LSharpItemPanel : IItemLogic<LSharpItemPanel>, IListItemBase<object>
{
    public object AttachValue
    {
        get { return def.GetValue(); }
        set { def.SetValue(value); }
    }
    public int index { get; set; }
    public IListBase ListComponent { get; set; }

    public static LSharpItemPanel LoadPrefab(string path, string uiName)
    {
        var prefab = BundleManager.Inst().LoadSync<GameObject>(path, uiName);
        var script = prefab.GetComponent<LSharpItemPanel>();
        if (!script) { script = prefab.AddComponent<LSharpItemPanel>(); }
        return script;
    }

    LSharpApiImpl impl = new LSharpApiImpl();
    public object hotfixScript { get { return impl.hotfixScript; } }
    public override void OnInitialize()
    {
        var regularName = LSharpApiImpl.Regular(name);
        var id = regularName.IndexOf('_');
        var regularType = (id >= 0) ? regularName.Substring(0, id) : regularName;
        OnInitialize("geniusbaby.LSharpScript." + regularType);
    }
    internal void OnInitialize(string name)
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
    public void SetValue(object value)
    {
        InstanceInvoke(@"SetValue", value);
    }
    public object GetValue()
    {
        return InstanceInvoke(@"GetValue");
    }
}