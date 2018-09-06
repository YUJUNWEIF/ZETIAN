using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LSharpPanel : LSharpAPI
{
    public void SetValue(object value)
    {
        InstanceInvoke(@"SetValue", value);
    }
    public object GetValue()
    {
        return InstanceInvoke(@"GetValue");
    }
}