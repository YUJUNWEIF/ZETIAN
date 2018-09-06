using UnityEngine;
using System.Collections.Generic;

public interface IGuiFrame
{
    bool TransparentMessage { get; }
    bool Fullscreen { get; }
    bool AutoRelease { get; }
    bool IsShow { get; }

    void Show(Transform desktop, int layer);
    void Hide();
    void SetLayer(int layer);
}