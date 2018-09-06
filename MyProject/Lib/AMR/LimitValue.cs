using System;

public class LimitValue<T>
    where T : IComparable<T>
{
    T m_min;
    T m_max;
    T m_current;
    public LimitValue(T min, T max, T current)
    {
        m_min = min;
        m_max = max;
        m_current = current;
    }
    public T min { get { return m_min; } }
    public T max { get { return m_max; } }
    public T current
    {
        get { return m_current; }
        set
        {
            if (value.CompareTo(m_min) < 0) { m_current = m_min; }
            if (value.CompareTo(m_max) < 0) { m_current = m_max; }
            m_current = value;
        }
    }
}

public class FloatLimitValue : LimitValue<float>
{
    public FloatLimitValue(float min, float max, float current) : base(min, max, current) { }
    public void Lerp(float v)
    {
        current = v * min + (1 - v) * max;
    }
    public float Percent
    {
        get { return (current - min) * 1f / (max - min); }
    }
}

public class IntegerLimitValue : LimitValue<int>
{
    public IntegerLimitValue(int min, int max, int current) : base(min, max, current) { }
    public void Lerp(float v)
    {
        current = (int)(v * min + (1 - v) * max + 0.5f);
    }
    public float Percent
    {
        get { return (current - min) * 1f / (max - min); }
    }
}