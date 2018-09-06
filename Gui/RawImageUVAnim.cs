using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby.ui
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageUVAnim : MonoBehaviour
    {
        public RawImage background;
        public AnimationCurve curveX;
        public AnimationCurve curveY;
        void Awake()
        {
            if (background == null) { background = GetComponent<RawImage>(); }
        }
        void Update()
        {
            var rc = background.uvRect;
            if (curveX != null)
            {
                rc.x = curveX.Evaluate(Time.time);
            }
            if (curveY != null)
            {
                rc.y = curveY.Evaluate(Time.time);
            }
            background.uvRect = rc;
        }
    }
}

