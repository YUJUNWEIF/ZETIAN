using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct PetExplorer
    {
        public struct RandAward
        {
            public int timeMs;
            public int index;
        }
        public int slotId;
        public int bookId;
        public List<int> petIds;
        public int randSeed;
        public long endAt;
        public bool unlock;
        public List<RandAward> randAwards;
    }
    public class PetModule : Singleton<PetModule>, IModule
    {
        public Util.ParamActions onEquipedSync = new Util.ParamActions();
        public Util.ParamActions onPetSync = new Util.ParamActions();
        public Util.Param1Actions<PetBase> onPetUpdate = new Util.Param1Actions<PetBase>();
        public Util.ParamActions onExplorerSync = new Util.ParamActions();
        public Util.Param1Actions<PetExplorer> onExplorerUpdate = new Util.Param1Actions<PetExplorer>();
        public int equipId { get; private set; }
        public List<PetBase> pets { get; private set; }
        public List<PetExplorer> exploreres { get; private set; }
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public PetBase Find(int uniqueId)
        {
            return pets.Find(it => it.uniqueId == uniqueId);
        }
        public void PetSync(int equipId, List<PetBase> pets)
        {
            this.equipId = equipId;
            this.pets = pets;
            onEquipedSync.Fire();
            onPetSync.Fire();
        }
        public void PetUpdate(int equipId, List<PetBase> ups)
        {
            this.equipId = equipId;
            onEquipedSync.Fire();

            var needSync = false;
            for (int index = 0; index < ups.Count; ++index)
            {
                var exist = pets.FindIndex(it => it.uniqueId == ups[index].uniqueId);
                if (exist >= 0)
                {
                    onPetUpdate.Fire(pets[exist] = ups[index]);
                }
                else
                {
                    pets.Add(ups[index]);
                    needSync = true;
                }
            }
            if (needSync) { onPetSync.Fire(); }
        }
        public void PetRmv(List<int> uIds)
        {
            for (int index = 0; index < uIds.Count; ++index)
            {
                var exist = pets.FindIndex(it => it.uniqueId == uIds[index]);
                if (exist >= 0)
                {
                    pets.RemoveAt(exist);
                }
            }
            onPetSync.Fire();
        }
        public PetBase GetEquiped()
        {
            return pets.Find(it => it.uniqueId == equipId);
        }
        public void ExplorerSync(List<PetExplorer> exploreres)
        {
            this.exploreres = exploreres;
            onExplorerSync.Fire();
        }
        public void ExplorerUpdate(List<PetExplorer> ups)
        {
            var needSync = false;
            for (int index = 0; index < ups.Count; ++index)
            {
                var exist = exploreres.FindIndex(it => it.slotId == ups[index].slotId);
                if (exist >= 0)
                {
                    onExplorerUpdate.Fire(exploreres[exist] = ups[index]);
                }
                else
                {
                    exploreres.Add(ups[index]);
                    needSync = true;
                }
            }
            if (needSync) { onExplorerSync.Fire(); }
        }
        public bool IsPetExplorering(int petId)
        {
            for (int index = 0; index < exploreres.Count; ++index)
            {
                var explorer = exploreres[index];
                if (explorer.petIds != null && explorer.petIds.Contains(petId)) { return true; }
            }
            return false;
        }
        public int FindAllWithModuleId(int moduleId)
        {
            int count = 0;
            for (int index = 0; index < pets.Count; ++index)
            {
                if (pets[index].mId == moduleId) { ++count; }
            }
            return count;
        }
        //public static bool IsMaxLv(Pet pet)
        //{
        //    var petCfg = tab.pet.Inst().Find(pet.mId);
        //    return pet.lv >= petCfg.lvLimit;
        //}
    }
}