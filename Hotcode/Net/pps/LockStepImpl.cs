using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
    public partial class LockStepImpl : IProtoImpl<LockStep>
    {
        public int PId() { return PID.LockStep; }
        public LockStepImpl(object proto, object notifies) : base(proto, notifies) { }
        public override void Process()
        {
            var cache = Net.ProtoManager.manager.Alloc<LockStep>();
            cache.Swap(proto);
            ClientLockStep.Inst().Receive(cache);
        }
    }
}
