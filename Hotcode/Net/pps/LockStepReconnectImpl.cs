using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
    public partial class LockStepReconnectImpl : IProtoImpl<LockStepReconnect>
    {
        public int PId() { return PID.LockStepReconnect; }
        public LockStepReconnectImpl(object proto, object notifies) : base(proto, notifies) { }
        public override void Process()
        {
            ClientLockStep.Inst().Reconnecting(proto.steps);
        }
    }
}
