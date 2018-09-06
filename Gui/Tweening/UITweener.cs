using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UITweener : MonoBehaviour
{
	public enum Method
	{
		Linear,
		EaseIn,
		EaseOut,
		EaseInOut,
		BounceIn,
		BounceOut,
	}
	public enum Style
	{
		Once,
		Loop,
		PingPong,
	}
	public Method method = Method.Linear;
	public Style style = Style.Once;
    public bool useCurve = false;
	[HideInInspector]
    public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public bool ignoreTimeScale = true;
    public Func<float, bool> messageProcessed;
    [HideInInspector]
    public AnimationCurve replaceCurve;
    public Util.ParamActions onLoopComplete = new Util.ParamActions();
    public Util.ParamActions onStop = new Util.ParamActions();

    protected virtual void OnStep(float factor) { }
    protected virtual void OnBegin() { }
    protected virtual void OnStop() { onStop.Fire(); }

    [System.NonSerialized] 
    float m_startTime = 0f;
    [System.NonSerialized]
    float m_last = 0;
    [SerializeField]
    float m_amountPerDelta = 1f;
    [SerializeField]
    float m_duration = 1f;
    [SerializeField]
    public float delay = 0f;
    [NonSerialized]
    float m_tweenFactor;
    [SerializeField]
    bool m_playing = false;
    public float duration
    {
        get { return m_duration; }
        set
        {
            if (!Mathf.Approximately(m_duration, value))
            {
                m_duration = (value > 0f) ? value : 1f;
                m_amountPerDelta = 1 / m_duration;
            }
        }
    }
    public float tweenFactor { get { return m_tweenFactor; } }
    public bool playing
    {
        get { return m_playing; }
        set
        {
            if (playing != value)
            {
                m_playing = value;
                if (m_playing)
                {
                    Play();
                }
                else
                {
                    Stop();
                }
            }
        }
    }
    public void Play(bool forward = true)
    {
        if (!enabled) { enabled = true; }
        m_playing = true;
        m_last = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        m_startTime = m_last + delay;
        m_last = m_startTime;
        m_amountPerDelta = Mathf.Abs(m_amountPerDelta);
        if (!forward) m_amountPerDelta = -m_amountPerDelta;
        m_tweenFactor = (m_amountPerDelta < 0f) ? 1f : 0f;
        OnBegin();
        Advance();
    }
    public void ContinuePlay(bool forward = true)
    {
        if (!enabled) { enabled = true; }
        if (playing)
        {
            m_amountPerDelta = Mathf.Abs(m_amountPerDelta) * (forward ? 1f : -1f);
        }
        else
        {
            Play(forward);
        }
    }
    public void Stop()
    {
        if (enabled) { enabled = false; }
        m_playing = false;
        OnStop();
    }
	public void SampleAt(float percent, bool stop = true)
    {
        m_tweenFactor = Mathf.Clamp01(percent);
        Sample();
		if (stop) {
			Stop ();
		}
    }
    void Update()
    {
        if (!m_playing) { return; }
        Advance();
    }
    protected void Advance()
    {
        float current = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        if (current <= m_startTime) { return; }

        float delta = current - m_last;
        m_last = current;
        m_tweenFactor += m_amountPerDelta * delta; // Advance the sampling factor

        bool finish = false;
        switch (style)
        {
            case Style.Loop:
                if (m_tweenFactor > 1f)
                {
                    m_tweenFactor -= Mathf.Floor(m_tweenFactor);
                    finish = true;
                }
                break;
            case Style.PingPong:// Ping-pong style reverses the direction
                if (m_tweenFactor > 1f || m_tweenFactor < 0f)
                {
                    m_tweenFactor = Mathf.PingPong(m_tweenFactor, 1);
                    m_amountPerDelta = -m_amountPerDelta;
                    finish = true;
                }
                break;
            case Style.Once:
                if (m_tweenFactor > 1f || m_tweenFactor < 0f)
                {
                    m_tweenFactor = Mathf.Clamp01(m_tweenFactor);
                    finish = true;
                }
                break;
        }
        Sample();
        if (finish)
        {
            switch (style)
            {
                case Style.Loop:
                case Style.PingPong: onLoopComplete.Fire(); break;
                case Style.Once: Stop(); break;
            }
        }
    }
    void Sample()
    {
        float val = m_tweenFactor;
        switch (method)
        {
            case Method.Linear: break;
            case Method.EaseIn:
                val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
                break;
            case Method.EaseOut:
                val = Mathf.Sin(0.5f * Mathf.PI * val);
                break;
            case Method.EaseInOut:
                const float pi2 = Mathf.PI * 2f;
                val = val - Mathf.Sin(val * pi2) / pi2;
                break;
            case Method.BounceIn:
                val = BounceLogic(val);
                break;
            case Method.BounceOut:
                val = 1f - BounceLogic(1f - val);
                break;
        }
        m_tweenFactor = val;
        if (useCurve)
        {
            if (replaceCurve != null) { m_tweenFactor = replaceCurve.Evaluate(val); }
            else if (animationCurve != null) { m_tweenFactor = animationCurve.Evaluate(val); }
        }
        //m_tweenFactor = (useCurve &&animationCurve != null) ? animationCurve.Evaluate(val) : val;

        if (messageProcessed != null && messageProcessed(m_tweenFactor)) { return; }
        OnStep(m_tweenFactor);
    }

	/// <summary>
	/// Main Bounce logic to simplify the Sample function
	/// </summary>
	
	float BounceLogic (float val)
	{
		if (val < 0.363636f) // 0.363636 = (1/ 2.75)
		{
			val = 7.5685f * val * val;
		}
		else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
		{
			val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
		}
		else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
		{
			val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
		}
		else
		{
			val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
		}
		return val;
	}
}
