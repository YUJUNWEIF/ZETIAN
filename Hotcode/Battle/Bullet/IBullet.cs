using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Util;

namespace geniusbaby
{
    public enum BulletType
    {
        DirectHit = 1,//直接攻击目标
        FlyLine = 2,//追踪目标
        FlyParacurve = 3,//抛物线弹道
        LeftRightWave = 4,//左右波形
        FullScreen = 5,//全屏
        JumpChain = 6,//弹跳型

        DelayHit = 10000,//延迟伤害
    }
    public abstract class IBullet : IBaseObj
    {
        protected float speed = 500f;
        public int casterId { get; protected set; }
        public cfg.weapon gunCfg { get; protected set; }
        public BulletType bulletType { get; protected set; }
        public abstract bool FrameUpdate(float deltaTime);
    }
}