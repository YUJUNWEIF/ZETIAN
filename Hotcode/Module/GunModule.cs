using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public struct Gun
    {
        public int id;
        public bool locked;
    }
    public class GunModule : Singleton<GunModule>, IModule
    {
        public int equipId;
        public List<Gun> guns { get; private set; }
        public Util.ParamActions onEquipedSync = new Util.ParamActions();
        public Util.ParamActions onGunSync = new Util.ParamActions();
        public Util.Param2Actions<Gun, Gun> onGunUpdate = new Util.Param2Actions<Gun, Gun>();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public Gun Find(int id)
        {
            return guns.Find(it => it.id == id);
        }
        public void Sync(int equipId,List<Gun> guns)
        {
            this.equipId = equipId;
            this.guns = guns;
            onEquipedSync.Fire();
            onGunSync.Fire();
        }
        public void Update(int equipId, List<Gun> ups)
        {
            this.equipId = equipId;
            onEquipedSync.Fire();

            var needSync = false;
            for (int index = 0; index < ups.Count; ++index)
            {
                var exist = this.guns.FindIndex(it => it.id == ups[index].id);
                if (exist >= 0)
                {
                    var old = this.guns[exist];
                    onGunUpdate.Fire(this.guns[exist] = ups[index], old);
                }
                else
                {
                    this.guns.Add(ups[index]);
                    needSync = true;
                }
            }
            if (needSync) { onGunSync.Fire(); }
        }
    }
}