using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using geniusbaby;
using geniusbaby.ui;

public enum AnchorMask : uint
{
    HLeft = 1,
    HCenter = 2,
    HRight = 3,

    VUp = 1 << 16,
    VCenter = 2 << 16,
    VDown = 3 << 16,

    HMask = 0x0000FFFF,
    VMask = 0xFFFF0000,
}

public class GuiManager : Singleton<GuiManager>
{
    public struct FrameMeta
    {
        public string name;
        public bool shown;
        public int layer;
    }
    private List<FrameMeta> m_queued = new List<FrameMeta>();
    private Dictionary<string, BehaviorWrapper> m_cached = new Dictionary<string, BehaviorWrapper>();
    private List<GameObject> m_preload = new List<GameObject>();
    private Transform m_ui;
    private Transform m_prefab;
    private Transform m_cache;
    public Util.Param2Actions<BehaviorWrapper, bool> onShowHideFrame = new Util.Param2Actions<BehaviorWrapper, bool>();
    public GuiManager()
    {
        m_ui = Desktop.root.Find("UI");
        m_prefab = Desktop.root.Find("Prefab");
        m_cache = Desktop.root.Find("Cache");
    }
    public void Initialize() { }
    public void Uninitialize()
    {
        HideAll();
        PatchShouldRefactorLater();
    }
    GameObject GetFromPreload(string name) { return m_preload.Find(it => it.name == name); }
    //public IEnumerator Preload()
    //{
    //    var ta = Resources.Load<TextAsset>(GamePath.asset.param + "FrameCache");
    //    string[] frameNames = ta.text.Split(',');
    //    for (int index = 0; index < frameNames.Length; ++index)
    //    {
    //        var frameName = frameNames[index];
    //        if (!m_cached.ContainsKey(frameName) && IsPreload(frameName) == null)
    //        {
    //            var prefab = Resources.Load<GameObject>(GamePath.asset.ui + @"Layout/" + frameName);
    //            if (prefab.activeSelf) { prefab.SetActive(false); }
    //            var obj = GameObject.Instantiate(prefab) as GameObject;
    //            obj.name = frameName;
    //            ((RectTransform)obj.transform).SetParent(m_cache);
    //            m_preload.AddLast(obj);
    //            if ((index + 1) % 3 == 0) { yield return null; }
    //        }
    //    }
    //}
    public void PatchShouldRefactorLater()
    {
        foreach (var cf in m_cached)
        {
            ((RectTransform)cf.Value.transform).SetParent(m_cache);
        }
        //m_cached.Clear();
    }
    public IList<FrameMeta> SaveQueuedFrame()
    {
        var backup = new List<FrameMeta>();
        for (int index = 0; index < m_queued.Count; ++index) { backup.Add(m_queued[index]); }
        return backup;
    }
    public void RestoreQueuedFrame(IList<FrameMeta> backup)
    {
        HideAll();
        if (backup != null)
        {
            for (int it = 0; it < backup.Count; ++it)
            {
                var q = backup[it];
                if (q.shown)
                {
                    q.layer = _Relay();
                    m_queued.Add(q);
                    _ShowFrame(q);
                }
                else
                {
                    m_queued.Add(q);
                }
            }
        }
    }
    public BehaviorWrapper ShowFrame(string frameName)
    {
        var exist = m_queued.FindIndex(it => it.name == frameName);
        if (exist >= 0) { m_queued.RemoveAt(exist); }
        var q = new FrameMeta() { name = frameName, shown = true, layer = _Relay() };
        m_queued.Add(q);
        return _ShowFrame(q);
    }
    public BehaviorWrapper BringToTop(BehaviorWrapper script)
    {
        return ShowFrame(script.name);
    }
    public void HideFrame(BehaviorWrapper frame)
    {
        HideFrame(frame.name);
    }
    public void HideFrame(string name)
    {
        _RemoveFromQueue(name);
        _HideFrame(name);
    }
    public BehaviorWrapper PushFrame(string frameName)
    {
        var exist = m_queued.FindIndex(it => it.name == frameName);
        if (exist >= 0) { m_queued.RemoveAt(exist); }

        if (m_queued.Count > 0)
        {
            var q = m_queued[m_queued.Count - 1];
            q.shown = false;
            m_queued[m_queued.Count - 1] = q;
            _HideFrame(q.name);
        }

        var fm = new FrameMeta() { name = frameName, shown = true, layer = _Relay() };
        m_queued.Add(fm);
        return _ShowFrame(fm);
    }
    public BehaviorWrapper PopFrame()
    {
        if (m_queued.Count > 0)
        {
            var q = m_queued[m_queued.Count - 1];
            if (!q.shown) { Util.Logger.Instance.Error("guimanager logic error"); }
            m_queued.RemoveAt(m_queued.Count - 1);
            _HideFrame(q.name);
        }
        if (m_queued.Count > 0)
        {
            var q = m_queued[m_queued.Count - 1];
            q.shown = true;
            q.layer = _Relay();
            m_queued[m_queued.Count - 1] = q;
            return _ShowFrame(q);
        }
        return null;
    }
    public BehaviorWrapper ReplaceTop(string frameName)
    {
        if (m_queued.Count > 0)
        {
            var q = m_queued[m_queued.Count - 1];
            if (!q.shown) { Util.Logger.Instance.Error("guimanager logic error"); }
            m_queued.RemoveAt(m_queued.Count - 1);
            _HideFrame(q.name);
        }
        var fm = new FrameMeta() { name = frameName, shown = true, layer = _Relay() };
        m_queued.Add(fm);
        return _ShowFrame(fm);
    }
    public BehaviorWrapper ReplaceAll(string frameName)
    {
        HideAll();
        var fm = new FrameMeta() { name = frameName, shown = true, layer = _Relay() };
        m_queued.Add(fm);
        return _ShowFrame(fm);
    }
    public BehaviorWrapper GetCachedFrame(string name)
    {
        BehaviorWrapper frameScript = null;
        if (m_cached.TryGetValue(name, out frameScript)) { return frameScript; }
        return null;
    }
    public BehaviorWrapper GetTopAsFrame()
    {
        for (int index = m_queued.Count - 1; index >= 0; --index)
        {
            var q = m_queued[index];
            if (q.shown)
            {
                if (index != m_queued.Count - 1) { Util.Logger.Instance.Error("guimanager logic error"); }

                BehaviorWrapper frameScript = null;
                if (m_cached.TryGetValue(q.name, out frameScript)) { return frameScript; }
                break;
            }
        }
        return null;
    }
    public void HideAll()
    {
        while (m_queued.Count > 0)
        {
            var q = m_queued[m_queued.Count - 1];
            m_queued.RemoveAt(m_queued.Count - 1);
            _HideFrame(q.name);
        }
    }
    private void _RemoveFromQueue(string frameName)
    {
        var exist = m_queued.FindIndex(it => it.name == frameName);
        if (exist >= 0)
        {
            m_queued.RemoveAt(exist);
            _Relay();
        }
    }
    int _Relay()
    {
        int layer = 0;
        for (int index = 0; index < m_queued.Count; ++index)
        {
            var tmp = m_queued[index];
            if (tmp.shown)
            {
                var script = m_cached[tmp.name];
                (script as IGuiFrame).SetLayer(layer++);
            }
        }
        return layer;
    }
    private BehaviorWrapper _ShowFrame(FrameMeta fm)
    {
        BehaviorWrapper frameScript = null;
        if (!m_cached.TryGetValue(fm.name, out frameScript))
        {
            frameScript = LoadFrame<BehaviorWrapper>(fm.name);
            m_cached.Add(fm.name, frameScript);
        }
        (frameScript as IGuiFrame).Show(m_ui, fm.layer);
        onShowHideFrame.Fire(frameScript, true);
        return frameScript;
    }
    private void _HideFrame(string frameName)
    {
        BehaviorWrapper frameScript = null;
        if (m_cached.TryGetValue(frameName, out frameScript))
        {
            onShowHideFrame.Fire(frameScript, false);
            var frame = frameScript as IGuiFrame;
            frame.Hide();
            if (frame.AutoRelease)
            {
                m_cached.Remove(frameName);
                frameScript.OnUnInitialize();
                Util.UnityHelper.DestroyGameObjectNoUnInit(frameScript.gameObject);
            }
        }
    }
    T LoadFrame<T>(string frameName) where T : BehaviorWrapper
    {
        var script = Load<T>(GamePath.asset.ui.frame, frameName);
        var frame = script as IGuiFrame;
        if (!frame.TransparentMessage || frame.Fullscreen)
        {
            ((RectTransform)script.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Desktop.realWidth);
            ((RectTransform)script.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Desktop.realHeight);
        }
        if (!frame.TransparentMessage)
        {
            var msgEater = new GameObject("__msg_eater").AddComponent<Image>();
            msgEater.transform.parent = script.transform;
            msgEater.rectTransform.SetAsFirstSibling();
            msgEater.rectTransform.localScale = Vector3.one;
            msgEater.rectTransform.localRotation = Quaternion.identity;
            msgEater.transform.position = new Vector3(Desktop.root.position.x, Desktop.root.position.y);
            var loc = msgEater.transform.localPosition;
            msgEater.transform.localPosition = new Vector3(loc.x, loc.y, 0f);
            msgEater.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Desktop.realWidth);
            msgEater.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Desktop.realHeight);
            
            //if (msgEater == null) { msgEater = frameScript.gameObject.AddComponent<Image>(); }
            msgEater.enabled = !frame.TransparentMessage;
            msgEater.color = new Color(0, 0, 0, 0);
        }
        script.OnInitialize();
        
        return script;
    }
    public static void Display(RectTransform cachedRc, RectTransform target, AnchorMask anchor, Vector2 offset)
    {
        Vector3[] worldPos = new Vector3[4];
        Vector2[] localPos = new Vector2[4];
        target.GetWorldCorners(worldPos);
        Vector2 center = Vector2.zero;
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int index = 0; index < worldPos.Length; ++index)
        {
            Framework.C2DWorldPosRectangleLocal(worldPos[index], out localPos[index]);
            center += localPos[index];
            if (localPos[index].x < minX) { minX = localPos[index].x; }
            if (localPos[index].x > maxX) { maxX = localPos[index].x; }
            if (localPos[index].y < minY) { minY = localPos[index].y; }
            if (localPos[index].y > maxY) { maxY = localPos[index].y; }
        }

        float targetX = (minX + maxY) * 0.5f;
        float targetY = (maxY + minY) * 0.5f;
        float thisX = targetX;
        float thisY = targetY;
        float thisWidth = cachedRc.sizeDelta.x;
        float thisHeight = cachedRc.sizeDelta.y;

        switch (anchor & AnchorMask.HMask)
        {
            case AnchorMask.HLeft: thisX = minX - thisWidth * 0.5f; break;
            case AnchorMask.HRight: thisX = maxX + thisWidth * 0.5f; break;
            default: thisX = (minX + maxY) * 0.5f; break;
        }
        switch (anchor & AnchorMask.VMask)
        {
            case AnchorMask.VUp: thisY = maxY + thisHeight * 0.5f; break;
            case AnchorMask.VDown: thisY = minY - thisHeight * 0.5f; break;
            default: thisY = (maxY + minY) * 0.5f; break;
        }
        cachedRc.position = new Vector2(thisX, thisY) + offset;
    }
    public T Load<T>(string path, string uiName) where T : MonoBehaviour
    {
        var ui = GetFromPreload(uiName);
        if (!ui)
        {
            var prefab = Resources.Load<GameObject>(path + uiName);
            ui = Util.UnityHelper.CloneGameObjectNoInit(prefab, uiName);
        }
        else
        {
            //Util.UnityHelper.CallInitialize(ui.transform);
        }
        var uiScript = ui.GetComponent<T>();
        if (!uiScript)
        {
            uiScript = ui.AddComponent<T>();
            //uiScript.OnInitialize();
        }
        uiScript.name = uiName;
        return uiScript;
    }
    public T ShowFrame<T>() where T : BehaviorWrapper, IGuiFrame { return ShowFrame(typeof(T).Name) as T; }
    public void HideFrame<T>() where T : BehaviorWrapper, IGuiFrame { HideFrame(typeof(T).Name); }
    public T PushFrame<T>() where T : BehaviorWrapper, IGuiFrame { return PushFrame(typeof(T).Name) as T; }
    public T ReplaceTop<T>() where T : BehaviorWrapper, IGuiFrame { return ReplaceTop(typeof(T).Name) as T; }
    public T ReplaceAll<T>() where T : BehaviorWrapper, IGuiFrame { return ReplaceAll(typeof(T).Name) as T; }
    public T GetCachedFrame<T>() where T : BehaviorWrapper, IGuiFrame { return GetCachedFrame(typeof(T).Name) as T; }
    public T GetTopAsFrame<T>() where T : BehaviorWrapper, IGuiFrame { return GetTopAsFrame() as T; }

    public static T NewPanel<T>() where T : MonoBehaviour
    {
        var panelName = typeof(T).Name;
        var prefab = Resources.Load<GameObject>(GamePath.asset.ui.panel + panelName);
        var ui = Util.UnityHelper.CloneGameObjectNoInit(prefab, panelName);
        var script = ui.GetComponent<T>();
        if (script == null) { script = ui.AddComponent<T>(); }
        return script;
    }
    public T ShowPanel<T>(RectTransform parent, AnchorMask anchor, Vector2 offset) where T : BehaviorWrapper
    {
        var panelScript = Load<T>(GamePath.asset.ui.panel, typeof(T).Name);
        panelScript.OnInitialize();
        Util.UnityHelper.Show(panelScript.transform, parent);
        Display(panelScript.transform as RectTransform, parent, anchor, offset);
        return panelScript;
    }
}