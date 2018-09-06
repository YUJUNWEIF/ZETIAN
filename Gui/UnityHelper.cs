using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public struct TagLayers
    {
        public const int Default = 0;
        public const int TransparentFx = 1;
        public const int IgnoreRaycast = 2;
        public const int __Reserve3 = 3;
        public const int Water = 4;
        public const int UI = 5;
        public const int __Reserve6 = 6;
        public const int __Reserve7 = 7;
        public const int Fish = 8;
        public const int Terrain = 9;
        public const int Pet = 10;
        public const int Bullet = 11;
        public const int PostEffect = 30;
        public const int Recycle = 31;
    }

    public static class UnityHelper
    {
        public static string TenThousand(int value)
        {
            if (value >= 10000) { return string.Format("{0:N1}w", value * 1f / 10000); }
            return value.ToString();
        }

        const int MaxNameLength = 14;

        public static bool CheckValidName(string input)
        {
            if (string.IsNullOrEmpty(input)) { return false; }

            RangeValue chcode = new RangeValue(System.Convert.ToInt32("4e00", 16), System.Convert.ToInt32("9fff", 16));//中文unicode范围（0x4e00～0x9fff）
            int length = 0;
            for (int index = 0; index < input.Length; ++index)
            {
                int code = System.Char.ConvertToUtf32(input, index);//获得字符串input中指定索引index处字符unicode编码

                if (IsValidEnglishLetter(code) || IsValidOtherLetter(code))
                {
                    ++length;
                }
                else if (IsValidChinese(code, chcode)) { length += 2; }
                else
                {
                    return false;
                }
            }
            return length <= MaxNameLength;
        }
        public static bool IsValidChinese(int unicode, RangeValue chcode)
        {
            return unicode >= chcode.current && unicode <= chcode.max;
        }
        public static bool IsValidEnglishLetter(int unicode)
        {
            return (unicode >= 'a' && unicode <= 'z') || (unicode >= 'A' && unicode <= 'Z');
        }
        public static bool IsValidOtherLetter(int unicode)
        {
            return (unicode >= '0' && unicode <= '9' ||
                unicode == '.' ||
                unicode == '-' || unicode == '_');
        }
        public static string EncodeColor(string src, Color clr)
        {
            return EncodeColor32(src, clr);
        }
        public static string EncodeColor32(string src, Color32 clr)
        {
            var builder = new StringBuilder();
            builder.Append("<color=\"#");
            builder.Append(ToHexChar((byte)(clr.r >> 4)));
            builder.Append(ToHexChar((byte)(clr.r & 15)));
            builder.Append(ToHexChar((byte)(clr.g >> 4)));
            builder.Append(ToHexChar((byte)(clr.g & 15)));
            builder.Append(ToHexChar((byte)(clr.b >> 4)));
            builder.Append(ToHexChar((byte)(clr.b & 15)));
            builder.Append("\">");
            builder.Append(src);
            builder.Append(@"</color>");
            return builder.ToString();
        }
        public static char ToHexChar(byte value)
        {
            return value <= 9 ? (char)(value + '0') : (char)(value - 10 + 'A');
        }
        public static Color32 Color32FromString(string src)
        {
            try
            {
                switch (src.Length)
                {
                    case 6:
                        return new Color32(
                            System.Convert.ToByte(src.Substring(0, 2), 16),
                            System.Convert.ToByte(src.Substring(2, 2), 16),
                            System.Convert.ToByte(src.Substring(4, 2), 16),
                            255);
                    case 8:
                        return new Color32(
                            System.Convert.ToByte(src.Substring(0, 2), 16),
                            System.Convert.ToByte(src.Substring(2, 2), 16),
                            System.Convert.ToByte(src.Substring(4, 2), 16),
                            System.Convert.ToByte(src.Substring(6, 2), 16));
                }
            }
            catch (System.Exception) { }
            return new Color32();
        }
        public static Color ColorFromString(string src)
        {
            return Color32FromString(src);
        }
        public static string Color32ToString(Color32 clr)
        {
            var builder = new StringBuilder();
            builder.Append(ToHexChar((byte)(clr.r >> 4)));
            builder.Append(ToHexChar((byte)(clr.r & 15)));
            builder.Append(ToHexChar((byte)(clr.g >> 4)));
            builder.Append(ToHexChar((byte)(clr.g & 15)));
            builder.Append(ToHexChar((byte)(clr.b >> 4)));
            builder.Append(ToHexChar((byte)(clr.b & 15)));
            builder.Append(ToHexChar((byte)(clr.a >> 4)));
            builder.Append(ToHexChar((byte)(clr.a & 15)));
            return builder.ToString();
        }

        public static bool IsNullOrEmpty(Component go)
        {
            return go == null || go.Equals(null);
            //return !go;
        }
        public static void SafeRelease(Object go)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                Object.DestroyImmediate(go);
            }
            else
            {
                Object.Destroy(go);
            }
#else
            Object.Destroy(go);
#endif
        }
        public static Transform FindChild(Transform trans, string name)
        {
            if (trans.name == name) { return trans; }

            for (int index = 0; index < trans.childCount; ++index)
            {
                var child = trans.GetChild(index);
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    child = FindChild(child, name);
                    if (child != null) { return child; }
                }
            }
            return null;
        }
        public static void SetLayerRecursively(Transform comp, int layer)
        {
            if (comp.gameObject.layer == layer) { return; }

            comp.gameObject.layer = layer;
            for (int index = 0; index < comp.childCount; ++index)
            {
                var child = comp.GetChild(index);
                if (child) { SetLayerRecursively(child, layer); }
            }
        }
        public static bool bEditor
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.OSXEditor;
            }
        }
        public static GameObject CloneGameObject(GameObject prefab, string name = null)
        {
            var obj = CloneGameObjectNoInit(prefab, name);
            CallInitialize(obj.transform);
            return obj;
        }
        public static GameObject CloneGameObjectNoInit(GameObject prefab, string name = null)
        {
            var obj = GameObject.Instantiate(prefab);
            if (!string.IsNullOrEmpty(name)) { obj.name = name; }
            if (!obj.activeSelf) { obj.SetActive(true); }
            return obj;
        }
        public static void DestroyGameObjectNoUnInit(GameObject obj)
        {
            //CallUnInitialize(obj.transform);
            var asUI = obj.transform as RectTransform;
            if (asUI != null)
            {
                asUI.SetParent(null);
            }
            else
            {
                obj.transform.parent = null;
            }
            SafeRelease(obj);
        }
        public static void Show(Component child, Component parent)
        {
            Show(child, parent, false, false);
        }
        public static void Show(Component child, Component parent, bool useParentLayer)
        {
            Show(child, parent, useParentLayer, false);
        }
        public static void Show(Component child, Component parent, bool useParentLayer, bool asLastSibling)
        {
            SetParent(child.transform, parent.transform);
            Show(child, asLastSibling);
            if (useParentLayer)
            {
                SetLayerRecursively(child.transform, parent.gameObject.layer);
            }
        }
        public static void ShowAsChild(Transform child, Transform parent)
        {
            //SetParent(child, parent);
            if (child.parent != parent) child.parent = parent;
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.Euler(0, 0, 0);
            child.localScale = Vector3.one;
            if (!child.gameObject.activeSelf) { child.gameObject.SetActive(true); }
        }
        static void SetParent(Transform child, Transform parent)
        {
            if (child.parent != parent && parent != null)
            {
                var asUI = child as RectTransform;
                if (asUI != null)
                {
                    asUI.SetParent(parent);
                    asUI.anchoredPosition = Vector2.zero;
                }
                else { child.parent = parent; }
            }
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.Euler(0, 0, 0);
            child.localScale = Vector3.one;
        }
        public static void ShowCopyFrom(Component dst, Component src)
        {
            dst.transform.position = src.transform.position;
            dst.transform.rotation = src.transform.rotation;
            dst.transform.localScale = src.transform.lossyScale;
            Show(dst);
        }
        public static void Show(Component child, bool asLastSibling = false)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
            }
            if (asLastSibling) { child.transform.SetAsLastSibling(); }
            CallOnShow(child);
        }
        public static void Hide(Component child)
        {
            CallOnHide(child);
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
            }
        }
        public static void ShowHide(Component child, bool show)
        {
            if (show) { Show(child); }
            else { Hide(child); }
        }
        public static EventTrigger.Entry GetEntry(Component sender, EventTriggerType type)
        {
            var ev = sender.GetComponent<EventTrigger>();
            if (ev == null) { ev = sender.gameObject.AddComponent<EventTrigger>(); }
            if (ev.triggers == null) { ev.triggers = new List<EventTrigger.Entry>(); }
            var entry = ev.triggers.Find(it => it.eventID == type);
            if (entry == null)
            {
                entry = new EventTrigger.Entry();
                entry.eventID = type;
                ev.triggers.Add(entry);
            }
            return entry;
        }
        public static void Register(Component sender, EventTriggerType type, UnityAction<BaseEventData> pro)
        {
            var entry = GetEntry(sender, type);
            entry.callback.RemoveListener(pro);
            entry.callback.AddListener(pro);
        }
        public static void UnRegister(Component sender, EventTriggerType type, UnityAction<BaseEventData> pro)
        {
            var entry = GetEntry(sender, type);
            entry.callback.RemoveListener(pro);
        }
        public static void CallInitialize(Component parent)
        {
            var script = parent.GetComponent<BehaviorWrapper>();
            if (script) { script.OnInitialize(); }
        }
        public static void CallUnInitialize(Component parent)
        {
            var script = parent.GetComponent<BehaviorWrapper>();
            if (script) { script.OnUnInitialize(); }
        }
        public static void CallOnShow(Component parent)
        {
            if (!parent.gameObject.activeSelf) { return; }
            var script = parent.GetComponent<BehaviorWrapper>();
            if (script) { script.OnShow(); }
        }
        public static void CallOnHide(Component parent)
        {
            if (!parent.gameObject.activeSelf) { return; }
            var script = parent.GetComponent<BehaviorWrapper>();
            if (script) { script.OnHide(); }
        }
        public static GameObject BatchStatic(Transform go, string name = null)
        {
            GameObject m_night = new GameObject(string.IsNullOrEmpty(name) ? go.name : name);
            m_night.transform.parent = go.parent;
            m_night.transform.localPosition = go.localPosition;
            m_night.transform.localRotation = go.localRotation;
            m_night.transform.localScale = go.localScale;
            var mm = new Dictionary<Material, List<CombineInstance>>();
            FindInGO(go.gameObject, g =>
            {
                if (g.activeSelf)
                {
                    var filter = g.GetComponent<MeshFilter>();
                    var render = g.GetComponent<Renderer>();
                    if (filter != null && render != null && render.sharedMaterial)
                    {
                        List<CombineInstance> combine = null;
                        if (!mm.TryGetValue(render.sharedMaterial, out combine))
                        {
                            mm.Add(render.sharedMaterial, combine = new List<CombineInstance>());
                        }
                        combine.Add(new CombineInstance() { mesh = filter.sharedMesh, transform = m_night.transform.worldToLocalMatrix * g.transform.localToWorldMatrix });
                    }
                }
            });
            foreach (var mc in mm)
            {
                var g = new GameObject();
                g.transform.parent = m_night.transform;
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.transform.localScale = Vector3.one;
                var filter = g.AddComponent<MeshFilter>();
                var render = g.AddComponent<MeshRenderer>();
                var mesh = new Mesh();
                mesh.CombineMeshes(mc.Value.ToArray());
                filter.sharedMesh = mesh;
                render.sharedMaterial = mc.Key;
                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                render.receiveShadows = false;
                render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            }
            FindInGO(m_night.gameObject, g => g.isStatic = true);
            return m_night;
        }
        static bool FindInGO(GameObject go, System.Action<GameObject> action)
        {
            action(go);
            if (go)
            {
                for (int index = 0; index < go.transform.childCount; ++index)
                {
                    var childT = go.transform.GetChild(index);
                    if(!FindInGO(childT.gameObject, action)) return false;
                }
            }
            return true;
        }
        public static bool IsComputer()
        {
            return (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.OSXPlayer);
        }
        static Vector3[] m_corners = new Vector3[4];
        public static Bounds GetTargetBounds(RectTransform target, Transform trans)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var toLocal = trans.worldToLocalMatrix;
            target.GetWorldCorners(m_corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }
            var contentBounds = new Bounds(vMin, Vector3.zero);
            contentBounds.Encapsulate(vMax);
            return contentBounds;
        }
    }
}