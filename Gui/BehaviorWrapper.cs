using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface ICleanable
{
    void Clean();
}

[RequireComponent(typeof(DisallowMultipleComponent))]
public abstract class BehaviorWrapper : MonoBehaviour
{
    Util.CoroutineHelper m_coroutineHelper = new Util.CoroutineHelper();

    public Util.CoroutineHelper coroutineHelper { get { return m_coroutineHelper; } }
    public virtual void OnInitialize() { }
    public virtual void OnUnInitialize() { coroutineHelper.StopAll(); }
    public virtual void OnShow() { }
    public virtual void OnHide() { }

    protected IEnumerator DelayProc(object obj, Action action)
    {
        yield return obj;
        if (action != null) { action(); }
    }
}
