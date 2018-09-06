using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public interface IPlayer2DObj
    {
        void Attach(FEDefenderObj obj);
        void DisplayAlphabetFly(EnglishDisplayParam edp, Transform scene2d, Vector2 fromPoint, float flyTime, AnimationCurve curve);
        void DisplayCoinFly(Transform scene2d, Vector2 from, float flyTime, AnimationCurve curve);
    }
}