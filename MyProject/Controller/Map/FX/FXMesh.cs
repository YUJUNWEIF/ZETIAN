using UnityEngine;
using System;
using System.Collections.Generic;

public enum FXActiveState
{
    AS_INACTIVE,
    AS_ACTIVE,
    AS_DYING,
    AS_DEAD
}

public class FXMesh : MonoBehaviour
{
    public Animation anim;
    public void Play() { anim.Play(); }
    public void Stop() { anim.Stop(); }
    public bool IsActive() { return anim.isPlaying; }
}
