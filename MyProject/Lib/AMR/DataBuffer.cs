using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct DataBuffer
{
    public int size;
    public IntPtr data;
    public DataBuffer(IntPtr data, int size) : this()
    {
        this.size = size;
        this.data = data;
    }
}

public class Bf
{
    public const int MaxLength = 4 * 1024;
    public float[] fBuffer = new float[MaxLength];
    public short[] sBuffer = new short[MaxLength];
    public byte[] bBuffer = new byte[MaxLength * sizeof(float)];
    public IntPtr cpp = Marshal.AllocHGlobal(MaxLength * sizeof(float));
}
