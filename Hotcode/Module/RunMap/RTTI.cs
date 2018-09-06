using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public static class RTTI
    {        
        static Dictionary<string, Func<Util.ISerializable>> m_factory = new Dictionary<string, Func<Util.ISerializable>>();
        static RTTI()
        {
            //m_factory.Add(typeof(NpcFuncParamless).ToString(), () => new NpcFuncParamless());
        }
        public static void Save(Util.OutStream os, Util.ISerializable seri)
        {
            os.WriteString(seri.GetType().ToString());
            os.Write(seri);
        }
        public static Util.ISerializable Load(Util.InStream os)
        {
            Func<Util.ISerializable> factory;
            if (m_factory.TryGetValue(os.ReadString(), out factory))
            {
                var result = factory();
                result.Unmarsh(os);
                return result;
            }
            return null;
        }
        public static void SaveList<E>(Util.OutStream os, IList<E> seri) where E : Util.ISerializable
        {
            os.WriteInt32(seri.Count);
            for (int index = 0; index < seri.Count; ++index)
            {
                Save(os, seri[index]);
            }
        }
        public static List<E> LoadList<E>(Util.InStream os) where E : Util.ISerializable
        {
            var count = os.ReadInt32();
            var result = new List<E>(count);
            for (int index = 0; index < count; ++index)
            {
                result.Add((E)Load(os));
            }
            return result;
        }
    }
}
