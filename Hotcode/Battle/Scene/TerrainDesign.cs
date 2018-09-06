using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class TerrainDesign
    {
        public TerrainParam tp { get; private set; }
        public Collider ground { get; private set; }
        public Transform left { get; private set; }
        public Transform right { get; private set; }
        public List<Transform> bps = new List<Transform>();
        public void Init(TerrainParam tp)
        {
            this.tp = tp;
            ground = tp.transform.Find(@"static").GetComponentInChildren<Collider>();
            left = tp.transform.Find(@"dynamic/a");
            right = tp.transform.Find(@"dynamic/b");
            bps.Clear();
            BpsCollect(bps, tp.transform.Find(@"dynamic/bp/left"));
            BpsCollect(bps, tp.transform.Find(@"dynamic/bp/right"));
        }
        void BpsCollect(List<Transform> bps, Transform bp)
        {
            for (int index = 0; index < bp.childCount; ++index)
            {
                bps.Add(bp.GetChild(index));
            }
        }
        public Transform FindBp(Vector3 pos)
        {
            float max = float.PositiveInfinity;
            int bpSelect = -1;
            for (int index = 0; index < bps.Count; ++index)
            {
                var distance = pos - bps[index].position;
                if (distance.V2().sqrMagnitude < max) { bpSelect = index; }
            }
            return bps[bpSelect];
        }
        public Transform GetObj3DResetLocation()
        {
            return null;
        }
        public Transform GetObj3DBackLocation()
        {
            return null;
        }
        public Transform Find(string path)
        {
            return tp.transform.Find(path);
        }
    }
}