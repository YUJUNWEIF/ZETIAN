using UnityEngine;
using System.Collections.Generic;

public abstract class ILSharpScript
{
    public BehaviorWrapper api { get; private set; }
    public virtual void OnInitialize(BehaviorWrapper api) { this.api = api; }
    public virtual void OnUnInitialize() { }
    public virtual void OnShow() { }
    public virtual void OnHide() { }
}