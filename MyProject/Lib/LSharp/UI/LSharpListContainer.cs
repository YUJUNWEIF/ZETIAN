using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LSharpListContainer : GuiListBasic<LSharpItemPanel, LSharpItemPanel, object>
{
    public override void OnInitialize()
    {
        layoutType = LayoutType.None;
        factoryType = FactoryType.Pregen;
        base.OnInitialize();
    }
}