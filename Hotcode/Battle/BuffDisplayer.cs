using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace geniusbaby
{
    public interface IBfGetter
    {
        bool ShouldProc();
        string Get(int skiId);
    }
    public class BuffDisplayer : IBuffListener
    {
        struct BFX
        {
            public int skiId;
            public FXControl fx;
        }
        List<BFX> m_bfxes = new List<BFX>();
        Transform m_trans;
        IBfGetter m_bf;
        public BuffDisplayer(Transform trans, IBfGetter bf)
        {
            m_trans = trans;
            m_bf = bf;
        }
        public void OnAdd(Buff buff)
        {
            if (!m_bf.ShouldProc()) { return; }

            string fxName = m_bf.Get(buff.param);
            if (!string.IsNullOrEmpty(fxName))
            {
                var fx = FXControl.Create(GamePath.asset.fx3D, fxName, false);
                fx.Play(m_trans);
                m_bfxes.Add(new BFX() { fx = fx, skiId = buff.param });
            }
        }
        public void OnRmv(Buff buff)
        {
            if (!m_bf.ShouldProc()) { return; }

            var exist = m_bfxes.FindIndex(it => it.skiId == buff.param);
            if (exist >= 0)
            {
                FXControl.Destroy(m_bfxes[exist].fx);
                m_bfxes.RemoveAt(exist);
                return;
            }
        }
        public void OnSync()
        {
            if (!m_bf.ShouldProc()) { return; }

            for (int index = 0; index < m_bfxes.Count; ++index)
            {
                FXControl.Destroy(m_bfxes[index].fx);
            }
            m_bfxes.Clear();
        }
    }
}
