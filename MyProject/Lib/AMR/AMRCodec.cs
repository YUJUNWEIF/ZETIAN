using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AMRCodec
{
    public const int SAMPLE_RATE = 8000;
    public const int CHANNELS = 1;
    public const int BITS_PER_SAMPLE = 16;

#if UNITY_IPHONE
    const string LibraryPath = "__Internal";
#else
    const string LibraryPath = "amr_codec";
#endif

    [DllImport(LibraryPath, EntryPoint = "MemoryAlloc", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr MemoryAlloc(int size);

    [DllImport(LibraryPath, EntryPoint = "MemoryOffset", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr MemoryOffset(IntPtr src, int offset);

    [DllImport(LibraryPath, EntryPoint = "MemoryFree", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MemoryFree(IntPtr ptr);

    [DllImport(LibraryPath, EntryPoint = "MemoryCopy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void MemoryCopy(IntPtr src, int srcIndex, IntPtr dst, int dstIndex, int length);


    public delegate void AMRFrameCodec(int frameCounter, IntPtr buffer, int bufferSize);

    [DllImport(LibraryPath, EntryPoint = "EncoderInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr EncoderInit();

    [DllImport(LibraryPath, EntryPoint = "EncoderUnInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncoderUnInit(IntPtr pCoder);

    [DllImport(LibraryPath, EntryPoint = "EncoderStart", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncoderStart(IntPtr pCoder, AMRFrameCodec fp);

    [DllImport(LibraryPath, EntryPoint = "EncoderInput", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncoderInput(IntPtr pCoder, IntPtr buffer, int length);

    [DllImport(LibraryPath, EntryPoint = "EncoderStop", CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncoderStop(IntPtr pCoder);


    [DllImport(LibraryPath, EntryPoint = "DecoderInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DecoderInit();

    [DllImport(LibraryPath, EntryPoint = "DecoderUnInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DecoderUnInit(IntPtr pCoder);

    [DllImport(LibraryPath, EntryPoint = "DecoderStart", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DecoderStart(IntPtr pCoder, AMRFrameCodec fp);

    [DllImport(LibraryPath, EntryPoint = "DecoderInput", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DecoderInput(IntPtr pCoder, IntPtr buffer, int length);

    [DllImport(LibraryPath, EntryPoint = "DecoderStop", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DecoderStop(IntPtr pCoder);

    //[DllImport(LibraryPath, EntryPoint = "SaveWav2File", CallingConvention = CallingConvention.Cdecl)]
    //public static extern void SaveWav2File(string path, int sampleRate, int bitsPerSample, int channels, int dataLength, IntPtr buffer);

    [DllImport(LibraryPath, EntryPoint = "WavWriteHeader", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WavWriteHeader(IntPtr buffer, int sampleRate, int bitsPerSample, int channels, int dataLength);

    [DllImport(LibraryPath, EntryPoint = "WavRewriteLen", CallingConvention = CallingConvention.Cdecl)]
    public static extern int WavRewriteLen(int dataLength);
}
