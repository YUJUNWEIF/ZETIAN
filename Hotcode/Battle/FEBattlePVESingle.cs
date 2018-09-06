using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Util;

namespace geniusbaby
{
    public class FEBattlePVESingle : IFEBattle
    {
        public FEDefenderObj player { get { return (FEDefenderObj)players[0]  ; } }
        public FEBattlePVESingle() : base(false) { }
        public override void OnPlayerDie(CombatPlayer<IFEBattle> obj)
        {
            var script = GuiManager.Instance.ShowFrame(typeof(LSharpScript.ResultFailFrame).Name);
            var frame = LSharpScript.T.As<LSharpScript.ResultFailFrame>(script);
            frame.Display(() => ChangeState<FEFinishState>().Initialize(this));
        }
        public override Alphabet RandPeek()
        {
            return player.RandPeek();
        }
        public override IList<Alphabet> Pull(int count)
        {
            return player.Pull(count);
        }
        public void Reborn()
        {
            ChangeState<FENormalState>().Initialize(this);
            player.Reborn();
        }
        public override void StartFight()
        {
            base.StartFight();
            ChangeState<FENormalState>().Initialize(this);
        }
        public override void StopFight()
        {
            base.StopFight();
        }
    }
}