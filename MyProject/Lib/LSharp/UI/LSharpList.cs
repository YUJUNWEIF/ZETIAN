using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LSharpList : GuiListBasic<LSharpItemPanel, LSharpItemPanel, object>
{
    public LayoutType layout = LayoutType.Vert;
    public override void OnInitialize()
    {
        layoutType = layout;
        factoryType = FactoryType.New;
        base.OnInitialize();
    }
};