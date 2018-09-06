using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Xml;

namespace geniusbaby
{
    public class SoundParam
    {
        //public float tempo = 1.0f;
        //public float tempoChange = 0;//(-50 ~ 100) --> tempo(0.5f ~ 2.0f)
        //public float rate = 1.0f;
        //public float rateChange = 0;//(-50 ~ 100) --> tempo(0.5f ~ 2.0f)
        //public float pitch = 1f;
        //public float pitchSemiTones = 0f;//-12f ~ 12f -->pitch(0.5f ~ 2.0f)

        public const int DEFAULT_SEQUENCE_MS = 40;
        public const int DEFAULT_SEEKWINDOW_MS = 15;
        public const int DEFAULT_OVERLAP_MS = 8;

        public string name;
        int m_antiAliasLength = 32;
        bool m_speech = true;
        public FloatLimitValue tempoDelta = new FloatLimitValue(-50f, 100f, 0f);
        public FloatLimitValue rateDelta = new FloatLimitValue(-50f, 100f, 0f);
        public FloatLimitValue pitchDelta = new FloatLimitValue(-12f, 12f, 0f);
        public bool speech
        {
            get { return m_speech; }
            set
            {
                m_speech = value;
                sequenceMS.current = m_speech ? DEFAULT_SEQUENCE_MS : 0;
                seekWindowMS.current = m_speech ? DEFAULT_SEEKWINDOW_MS : 0;
            }
        }
        public bool quickSeek = false;
        public bool antiAlias = false;//default false
        public int antiAliasLength//(1~16) * 8, default is 32
        {
            get { return m_antiAliasLength; }
            set
            {
                var multi = value / 8;
                if (multi < 1) { multi = 1; }
                else if (multi > 16) { multi = 16; }
                m_antiAliasLength = multi * 8;
            }
        }
        public IntegerLimitValue sequenceMS = new IntegerLimitValue(0, 100, 0);
        public IntegerLimitValue seekWindowMS = new IntegerLimitValue(0, 40, 0);
        public IntegerLimitValue overlapMs = new IntegerLimitValue(0, 20, 8);
    }

    public class SPManager : Util.IBinSerializer, Util.IJsonSerializer
    {
        public List<SoundParam> sparams = new List<SoundParam>();
        public SoundParam Find(Predicate<SoundParam> match)
        {
            for (int index = 0; index < sparams.Count; ++index)
            {
                var v = sparams[index];
                if (match(v)) { return v; }
            }
            return null;
        }
        public void UnMarsh(BinaryReader os)
        {
            sparams = DeBinary.Deserializer.Deserialize<List<SoundParam>>(os);
        }
        public void Marsh(BinaryWriter os)
        {
            DeBinary.Serializer.Serialize(sparams, os);
        }
        public void FromJson(string jsonString)
        {
            DeJson.Deserializer.Deserialize<List<SoundParam>>(jsonString);
        }
        public string SaveToJson()
        {
            return DeJson.Serializer.Serialize(sparams);
        }
        public static void LoadFromMemory(byte[] bytes)
        {
            try
            {
                var str = Encoding.UTF8.GetString(bytes);
                Util.XMLParser xmlParser = new Util.XMLParser();
                Util.XMLNode xn = xmlParser.Parse(str);
            }
            catch (Exception ex) { Util.Logger.Instance.Error(ex.Message, ex); }
        }
    }
}