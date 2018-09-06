using System;
using System.Collections;
using System.Collections.Generic;

public interface IActionState
{
    void Enter(ActionStateManager manager);
    void Leave();
    void Update();
}

public class ActionStateManager
{
    bool m_stateSwitching;
    Stack<IActionState> m_queued = new Stack<IActionState>();
    Util.CoroutineHelper m_coroutineHelper = new Util.CoroutineHelper();
    public bool IsSwitching() { return m_stateSwitching; }
    public void CompleteSwitch() { m_stateSwitching = false; }
    protected IActionState GetCurrent()
    {
        if (m_queued.Count > 0) { return m_queued.Peek(); }
        return null;
    }
    public void ChangeState<T>() where T : IActionState, new()
    {
        m_coroutineHelper.StopAll();
        IActionState current = GetCurrent();
        if (current == null || current.GetType() != typeof(T))
        {
            m_stateSwitching = true;
            m_coroutineHelper.StartCoroutine(ChangeStateNextFrame(new T()));
        }
    }
    public void PushState<T>() where T : IActionState, new()
    {
        m_coroutineHelper.StopAll();

        IActionState current = GetCurrent();
        if (current == null || current.GetType() != typeof(T))
        {
            m_stateSwitching = true;
            m_coroutineHelper.StartCoroutine(PushStateNextFrame(new T()));
        }
    }
    public void PopState()
    {
        m_coroutineHelper.StopAll();
        m_stateSwitching = true;
        m_coroutineHelper.StartCoroutine(PopStateNextFrame());
    }
    public void Update()
    {
        IActionState current = GetCurrent();
        if (current != null) { current.Update(); }
    }
    public void ReplaceAll<T>() where T : IActionState, new()
    {
        m_coroutineHelper.StopAll();
        m_stateSwitching = true;
        m_coroutineHelper.StartCoroutine(ReplaceAllNextFrame(new T()));
    }
    IEnumerator ChangeStateNextFrame<T>(T next) where T : IActionState, new()
    {
        yield return null;
        IActionState current = GetCurrent();
        if (current != null) { current.Leave(); m_queued.Pop(); }
        current = next;
        if (current != null) { m_queued.Push(current); current.Enter(this); }
    }
    IEnumerator PushStateNextFrame<T>(T next) where T : IActionState, new()
    {
        yield return null;
        IActionState current = GetCurrent();
        if (current != null) { current.Leave(); }
        current = next;
        if (current != null) { m_queued.Push(current); current.Enter(this); }
    }
    IEnumerator PopStateNextFrame()
    {
        yield return null;
        IActionState current = GetCurrent();
        if (current != null) { current.Leave(); m_queued.Pop(); }
        current = GetCurrent();
        if (current != null) { current.Enter(this); }
    }
    IEnumerator ReplaceAllNextFrame<T>(T next) where T : IActionState, new()
    {
        yield return null;
        IActionState current = GetCurrent();
        if (current != null && next.GetType() == current.GetType())
        {
            m_queued.Clear();
            current = next;
            m_queued.Push(current);
        }
        else
        {
            if (current != null) { current.Leave(); m_queued.Clear(); }
            current = next;
            if (current != null) { m_queued.Push(current); current.Enter(this); }
        }
    }
    public bool IsState<T>() where T : IActionState
    {
        return GetCurrent().GetType() == typeof(T);
    }
}

public class StateManager : ActionStateManager //: Singleton<StateManager>
{
    public static StateManager Instance { get; private set; }
    public StateManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else { throw new Exception("Can't be create twice!"); }
    }
    //ActionStateManager m_imp = new ActionStateManager();
    //public void ChangeState<T>() where T : IActionState, new()
    //{
    //    m_imp.ChangeState<T>();
    //}
}