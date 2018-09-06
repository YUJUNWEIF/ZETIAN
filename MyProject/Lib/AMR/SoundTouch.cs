using System;
using System.Runtime.InteropServices;

namespace geniusbaby
{
    public class SoundTouch
    {
#if UNITY_IPHONE
        const string DLL = "__Internal";
#else
        const string DLL = "soundtouch";
#endif
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_getVersionId")]
        private static extern int GetVersionId();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr soundtouch_createInstance();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_destroyInstance(IntPtr h);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setRate(IntPtr h, float newRate);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setTempo(IntPtr h, float newTempo);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setRateChange(IntPtr h, float newRate);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setTempoChange(IntPtr h, float newTempo);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setPitch(IntPtr h, float newPitch);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setPitchOctaves(IntPtr h, float newPitch);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setPitchSemiTones(IntPtr h, float newPitch);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setChannels(IntPtr h, uint numChannels);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_setSampleRate(IntPtr h, uint srate);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_flush(IntPtr h);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_putSamples(IntPtr h, short[] samples, uint numSamples);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void soundtouch_clear(IntPtr h);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int soundtouch_setSetting(IntPtr h, int settingId, int value);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int soundtouch_getSetting(IntPtr h, int settingId);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint soundtouch_numUnprocessedSamples(IntPtr h);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint soundtouch_receiveSamples(IntPtr h, short[] outBuffer, uint maxSamples);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint soundtouch_numSamples(IntPtr h);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int soundtouch_isEmpty(IntPtr h);

        public enum SETTING
        {
            USE_AA_FILTER = 0,
            AA_FILTER_LENGTH = 1,
            USE_QUICKSEEK = 2,
            SEQUENCE_MS = 3,
            SEEKWINDOW_MS = 4,
            OVERLAP_MS = 5,


            /// Call "getSetting" with this ID to query processing sequence size in samples. 
            /// This value gives approximate value of how many input samples you'll need to 
            /// feed into SoundTouch after initial buffering to get out a new batch of
            /// output samples. 
            ///
            /// This value does not include initial buffering at beginning of a new processing 
            /// stream, use SETTING_INITIAL_LATENCY to get the initial buffering size.
            ///
            /// Notices: 
            /// - This is read-only parameter, i.e. setSetting ignores this parameter
            /// - This parameter value is not constant but change depending on 
            ///   tempo/pitch/rate/samplerate settings.
            NOMINAL_INPUT_SEQUENCE = 6,


            /// Call "getSetting" with this ID to query nominal average processing output 
            /// size in samples. This value tells approcimate value how many output samples 
            /// SoundTouch outputs once it does DSP processing run for a batch of input samples.
            ///
            /// Notices: 
            /// - This is read-only parameter, i.e. setSetting ignores this parameter
            /// - This parameter value is not constant but change depending on 
            ///   tempo/pitch/rate/samplerate settings.
            NOMINAL_OUTPUT_SEQUENCE = 7,


            /// Call "getSetting" with this ID to query initial processing latency, i.e.
            /// approx. how many samples you'll need to enter to SoundTouch pipeline before 
            /// you can expect to get first batch of ready output samples out. 
            ///
            /// After the first output batch, you can then expect to get approx. 
            /// SETTING_NOMINAL_OUTPUT_SEQUENCE ready samples out for every
            /// SETTING_NOMINAL_INPUT_SEQUENCE samples that you enter into SoundTouch.
            ///
            /// Example:
            ///     processing with parameter -tempo=5
            ///     => initial latency = 5509 samples
            ///        input sequence  = 4167 samples
            ///        output sequence = 3969 samples
            ///
            /// Accordingly, you can expect to feed in approx. 5509 samples at beginning of 
            /// the stream, and then you'll get out the first 3969 samples. After that, for 
            /// every approx. 4167 samples that you'll put in, you'll receive again approx. 
            /// 3969 samples out.
            ///
            /// This also means that average latency during stream processing is 
            /// INITIAL_LATENCY-OUTPUT_SEQUENCE/2, in the above example case 5509-3969/2 
            /// = 3524 samples
            /// 
            /// Notices: 
            /// - This is read-only parameter, i.e. setSetting ignores this parameter
            /// - This parameter value is not constant but change depending on 
            ///   tempo/pitch/rate/samplerate settings.
            INITIAL_LATENCY = 8
        }

        private IntPtr handle;
        public void Initialize() { handle = soundtouch_createInstance(); }
        public void UnInitialize() { soundtouch_destroyInstance(handle); }

        public uint NumSamples() { return soundtouch_numSamples(handle); }
        public uint NumUnprocessedSamples() { return soundtouch_numUnprocessedSamples(handle); }
        public int IsEmpty() { return soundtouch_isEmpty(handle); }
        public void Clear() { soundtouch_clear(handle); }
        public void Flush() { soundtouch_flush(handle); }
        public void SetChannels(uint numChannels) { soundtouch_setChannels(handle, numChannels); }
        public void SetSampleRate(uint srate) { soundtouch_setSampleRate(handle, srate); }

        public void SetTempo(float newTempo) { soundtouch_setTempo(handle, newTempo); }
        public void SetTempoChange(float newTempo) { soundtouch_setTempoChange(handle, newTempo); }
        public void SetRate(float newRate) { soundtouch_setTempo(handle, newRate); }
        public void SetRateChange(float newRate) { soundtouch_setRateChange(handle, newRate); }
        public void SetPitch(float newPitch) { soundtouch_setPitch(handle, newPitch); }
        public void SetPitchOctaves(float newPitch) { soundtouch_setPitchOctaves(handle, newPitch); }
        public void SetPitchSemiTones(float newPitch) { soundtouch_setPitchSemiTones(handle, newPitch); }
        public int SetSetting(SETTING settingId, int value) { return soundtouch_setSetting(handle, (int)settingId, value); }
        public int GetSetting(SETTING settingId) { return soundtouch_getSetting(handle, (int)settingId); }

        Action<short[], int> m_callback;
        int m_sampleRate;
        int m_channels;

        public void Encode(IClipVisit cv, Bf bf, SoundParam param, Action<short[], int> callback)
        {
            Start(cv.sampleRate, cv.channels, param, callback);

            int offsetSamples = 0;
            while (offsetSamples < cv.samples)
            {
                int samples = cv.GetData(bf.fBuffer, offsetSamples);
                if (samples <= 0)
                {
                    break;
                }
                for (int index = 0; index < samples * cv.channels; ++index)
                {
                    bf.sBuffer[index] = (short)(bf.fBuffer[index] * short.MaxValue);
                }
                offsetSamples += samples;
                //soundtouch_putSamples(handle, bf.sBuffer, (uint)samples);
                Input(bf.sBuffer, samples, bf);
                Receive(bf);
            }
            Stop(bf);
        }
        public void Start(int sampleRate, int channels, SoundParam param, Action<short[], int> callback)
        {
            m_sampleRate = sampleRate;
            m_channels = channels;
            m_callback = callback;
            SetTempoChange(param.tempoDelta.current);
            SetRateChange(param.rateDelta.current);
            SetPitchOctaves(param.pitchDelta.current);
            SetSampleRate((uint)sampleRate);
            SetChannels((uint)channels);
        }
        public void Input(short[] data, int samples, Bf bf)
        {
            soundtouch_putSamples(handle, data, (uint)samples);
            Receive(bf);
        }
        public void Stop(Bf bf)
        {
            Flush();
            Receive(bf);
            Clear();
        }
        void Receive(Bf bf)
        {
            while (true)
            {
                int samples = (int)soundtouch_receiveSamples(handle, bf.sBuffer, (uint)(bf.sBuffer.Length / m_channels));
                if (samples <= 0)
                {
                    break;
                }
                for (int index = 0; index < samples * m_channels; ++index)
                {
                    int value = (int)bf.sBuffer[index];
                    value = (value < short.MinValue) ? short.MinValue : (value > short.MaxValue) ? short.MaxValue : value;
                    //buffer[index + bufferOffset] = value * 1f / 32768;
                    bf.sBuffer[index] = (short)value;
                }
                //bufferOffset += (int)(samples * clip.channels);
                m_callback(bf.sBuffer, samples);
            }
        }
    }
}