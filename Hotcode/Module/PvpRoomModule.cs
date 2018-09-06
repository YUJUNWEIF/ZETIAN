using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class PvpRoomModule : Singleton<PvpRoomModule>, IModule
    {
        public int roomId { get; private set; }
        public int combatType { get; private set; }   
        public int needPlayers { get; private set; }
        public List<pps.ProtoDetailPlayer> players { get; private set; }
        public bool full { get { return players.Count >= needPlayers; } }
        
        public int tickDown { get; private set; }
        public int randSeed { get; private set; }

        public Util.ParamActions onSync = new Util.ParamActions();
        public Util.ParamActions onTickDownSync = new Util.ParamActions();
        public Util.Param1Actions<pps.GSrvPvpNotify> onJoin = new Util.Param1Actions<pps.GSrvPvpNotify>();
        
        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        
        public void Sync(int roomId, int combatType, int needPlayers, List<pps.ProtoDetailPlayer> players)
        {
            this.roomId = roomId;
            this.combatType = combatType;
            this.needPlayers = needPlayers;
            this.players = players;
            onSync.Fire();
        }
        public void Sync(int combat)
        {
            this.combatType = combat;
        }
        public void Update(List<pps.ProtoDetailPlayer> ups)
        {
            bool needSync = false;
            for (int index = 0; index < ups.Count; ++index)
            {
                var up = ups[index];
                int exist = players.FindIndex(it => it.uniqueId == up.uniqueId);
                if (exist >= 0)
                {
                    players[exist] = up;
                }
                else
                {
                    players.Add(up);
                    needSync = true;
                }
            }
            if (needSync) { onSync.Fire(); }
        }
        public void Rmv(string playerId)
        {
            int exist = players.FindIndex(it => it.uniqueId == playerId);
            if (exist >= 0)
            {
                players.RemoveAt(exist);
                onSync.Fire();
            }
        }
        public void Clear()
        {
            Cancel();
        }
        public void Cancel()
        {
            this.combatType = GlobalDefine.GTypeUnknown;
            this.roomId = 0;
            this.needPlayers = 0;
            if (this.players != null) { this.players.Clear(); }
            onSync.Fire();
        }
        public void SyncTickDown(int tickDown, int randSeed)
        {
            this.tickDown = tickDown;
            this.randSeed = randSeed;
            onTickDownSync.Fire();
        }
    }
}
