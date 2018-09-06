using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public abstract class IClipVisit
{
    public int sampleRate { get; protected set; }
    public int bitPerSample { get; protected set; }
    public int offset { get; protected set; }
    public int samples { get; protected set; }
    public int channels { get; protected set; }
    public abstract int GetData(float[] buffer, int offsetSamples);
    public int ResampleC1I16(Bf bf, int resampleRate, int resampleOffset)
    {
        if (resampleOffset < 0) { return -1; }

        if (sampleRate <= resampleRate)
        {
            var length = GetData(bf.fBuffer, resampleOffset);
            for (int index = 0; index < length; ++index)
            {
                if (channels == 1)
                {
                    bf.sBuffer[index] = (short)(bf.fBuffer[index] * short.MaxValue);
                }
                else
                {
                    int v = 0;
                    for (int c = 0; c < channels; ++c)
                    {
                        v += (int)(bf.fBuffer[index * channels + c] * short.MaxValue);
                    }
                    bf.sBuffer[index] = (short)(v / channels);
                }
            }
            return length;
        }
        else
        {
            int offset = (int)(resampleOffset * (long)sampleRate / resampleRate);
            long length = GetData(bf.fBuffer, offset);
            int relength = (int)(length * resampleRate / sampleRate);
            for (long index = 0; index < relength; ++index)
            {
                var start = (index * sampleRate / resampleRate) * channels;
                var end = ((index + 1) * sampleRate / resampleRate) * channels;
                int v = 0;
                for (long j = start; j < end; ++j)
                {
                    v += (int)(bf.fBuffer[j] * short.MaxValue);
                }
                bf.sBuffer[index] = (short)(v / (end - start));
            }
            return relength;
        }
    }
}
public class ClipBufferVisit : IClipVisit
{
    float[] m_buffer;
    public void SetData(float[] buffer, int sampleRate, int channels, int offset = 0, int samples = 0)
    {
        var capcity = (buffer.Length / channels);
        var max = capcity - offset;
        if (samples <= 0 || samples > max) { samples = max; }

        this.m_buffer = buffer;
        this.sampleRate = sampleRate;
        this.channels = channels;
        this.offset = offset;
        this.samples = samples;
        this.bitPerSample = sizeof(float) * 8;
    }
    public override int GetData(float[] buffer, int offsetSamples)
    {
        if (offsetSamples < 0) { return -1; }

        int maxSamples = (buffer.Length / channels);
        if (offsetSamples + maxSamples >= samples)
        {
            maxSamples = samples - offsetSamples;
        }
        if (maxSamples > 0)
        {
            Array.Copy(m_buffer, (offsetSamples + offset) * channels, buffer, 0, maxSamples * channels);
            return maxSamples;
        }
        return maxSamples;
    }
}

public class ClipVisit : IClipVisit
{
    AudioClip clip;
    public void SetData(AudioClip clip, int offset = 0, int samples = 0)
    {
        var capcity = clip.samples;
        var max = capcity - offset;
        if (samples <= 0 || samples > max) { samples = max; }

        this.clip = clip;
        this.sampleRate = clip.frequency;
        this.channels = clip.channels;
        this.offset = offset;
        this.samples = samples;
        this.bitPerSample = sizeof(float) * 8;
    }
    public override int GetData(float[] buffer, int offsetSamples)
    {
        if (offsetSamples < 0) { return -1; }

        int maxSamples = buffer.Length / channels;
        if (offsetSamples + maxSamples >= samples)
        {
            maxSamples = samples - offsetSamples;
        }
        if (maxSamples > 0)
        {
            if (clip.GetData(buffer, (int)(offsetSamples + offset))) { return maxSamples; }
        }
        return maxSamples;
    }
}

public interface IAMRVisit
{
    int GetData(IntPtr m_cpp, int offset, int maxLength);
}
public class AMRVisit : IAMRVisit
{
    byte[] data;
    public void SetData(byte[] data)
    {
        this.data = data;
    }
    public int GetData(IntPtr m_cpp, int offset, int maxLength)
    {
        int actualLen = maxLength;
        if (offset + actualLen >= data.Length)
        {
            actualLen = data.Length - offset;
        }
        if (actualLen > 0)
        {
            Marshal.Copy(data, offset, m_cpp, actualLen);
        }
        return actualLen;
    }
}