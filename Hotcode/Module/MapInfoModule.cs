using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class MapInfo
    {
        public int id;
    }
    public class MapInfoModule : Singleton<MapInfoModule>, IModule
    {
        public List<MapInfo> mapInfos = new List<MapInfo>();
        public Util.ParamActions onSync = new Util.ParamActions();
        public Util.Param1Actions<MapInfo> onUpdate = new Util.Param1Actions<MapInfo>();
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        public void Sync(List<MapInfo> mapInfos)
        {
            this.mapInfos = mapInfos;
            onSync.Fire();
        }
        public void Update(MapInfo mapInfo)
        {
            int index = mapInfos.FindIndex(it => it.id == mapInfo.id);
            if (index >= 0)
            {
                onUpdate.Fire(mapInfos[index] = mapInfo);
            }
        }
        public MapInfo Find(int mapId)
        {
            return mapInfos.Find(it => it.id == mapId);
        }
    }
}