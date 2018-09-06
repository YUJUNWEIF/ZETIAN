using UnityEngine;
using System;
using System.Collections;

namespace geniusbaby
{
    public class IBaseObj : MonoBehaviour, IFinalize
    {
        public Util.CoroutineHelper coroutineHelper = new Util.CoroutineHelper();
        public EntityId entityId { get; protected set; }
        public int moduleId { get; protected set; }
        public virtual void UnInitialize()
        {
            coroutineHelper.StopAll();
        }
    }
}
