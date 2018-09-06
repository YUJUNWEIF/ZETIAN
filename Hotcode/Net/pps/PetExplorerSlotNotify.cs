using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
	public partial class PetExplorerSlotNotifyImpl : IProtoImpl<PetExplorerSlotNotify>
	{
//generate code begin
		public int PId() { return PID.PetExplorerSlotNotify; }
		public PetExplorerSlotNotifyImpl(object proto, object notifies) : base(proto, notifies) { }
//generate code end
		public override void Process()
        {
            
            //switch (proto.act)
            //{
            //    case NotifyAction.Sy:
            //        {
            //            var exploreres = new List<PetExplorer>();
            //            var explorerCfgs = geniusbaby.tab.heroExplorer.Inst().RecordArray;
            //            for (int index = 0; index < explorerCfgs.Count; ++index)
            //            {
            //                var slot = proto.slots.Find(it => it.slotId == explorerCfgs[index].id);
            //                if (slot != null)
            //                {
            //                    exploreres.Add(Convert(slot));
            //                }
            //                else
            //                {
            //                    exploreres.Add(new PetExplorer() { slotId = explorerCfgs[index].id, unlock = false, bookId = 0, endAt = 0, randSeed = 0 });
            //                }
            //            }
            //            PetModule.Inst().ExplorerSync(exploreres);
            //        }
            //        break;
            //    case NotifyAction.Up:
            //        {
            //            var exploreres = proto.slots.ConvertAll(it => Convert(it));
            //            PetModule.Inst().ExplorerUpdate(exploreres);
            //        }
            //        break;
            //}
        }
        //PetExplorer Convert(PetExplorerSlotNotify.Slot explorer)
        //{
        //    var slot = new PetExplorer()
        //    {
        //        slotId = explorer.slotId,
        //        bookId = explorer.bookId,
        //        petIds = explorer.petIds,
        //        randSeed = explorer.randSeed,
        //        endAt = explorer.endAt,
        //        unlock = explorer.unlock,
        //        randAwards = new List<PetExplorer.RandAward>(),
        //    };

        //    if (explorer.bookId > 0)
        //    {
        //        var itemCfg = tab.item.Inst().Find(explorer.bookId);
        //        var bookCfg = tab.heroExplorerMap.Inst().Find(int.Parse(itemCfg.param));

        //        var rand = new Util.FastRandom((uint)explorer.randSeed);
        //        var awardIndexes = rand.RandomBetween(0, bookCfg.randDrop.Length, bookCfg.randAwardCount);
        //        const int interval = 10 * 60;
        //        var timeIndexes = rand.RandomBetween(0, bookCfg.timeSec / interval, bookCfg.randAwardCount);
        //        for (int index = 0; index < bookCfg.randAwardCount; ++index)
        //        {
        //            slot.randAwards.Add(new PetExplorer.RandAward() { index = awardIndexes[index], timeMs = timeIndexes[index] * interval });
        //        }
        //        FEMath.SortFixUnityBugAndNotStable(slot.randAwards, (x, y) => x.timeMs.CompareTo(y.timeMs));
        //    }
        //    return slot;
        //}
    }
}
