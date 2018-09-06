using System;
using System.Collections.Generic;
using geniusbaby;

namespace geniusbaby
{
    public class Friend
    {        
        public enum Type
        {
            Not = 0,
            Ok = 1,
            NeedSelfConfirm = 2,
        }
        public Type type;
        public string id;
        public string name;
        public string url;
        public long giveClock;
        public bool isOnline;
    }    
    public class FriendChat
    {
        public long utc;
        public bool meTalk;
        public string other;
        public string msg;
    }
    public struct FriendPetExplorer
    {
        public int slotId;
        public int bookId;
        public List<int> petIds;
        public int randSeed;
        public long endAt;
        public bool unlock;
        public bool canRobbery;

        public static FriendPetExplorer FromProto(geniusbaby.pps.VisitInfo it)
        {
            return new FriendPetExplorer()
            {
                slotId = it.slotId,
                unlock = it.unlock,
                bookId = it.slotId,
                endAt = it.endAt,
                petIds = it.petMIds,
                canRobbery = it.canRobbery,
            };
        }
    }
    public class FriendModule : Singleton<FriendModule>, IModule
    {
        public int capacity { get; private set; }
        public List<Friend> friends = new List<Friend>();
        public List<FriendChat> chats = new List<FriendChat>();

        public string friUId { get; private set; }
        public List<FriendPetExplorer> friendSlots { get; private set; }

        public Util.ParamActions onSync = new Util.ParamActions();
        public Util.Param1Actions<Friend> onUpdate = new Util.Param1Actions<Friend>();
        public Util.ParamActions onFriSlotSync = new Util.ParamActions();
        public Util.Param1Actions<FriendPetExplorer> onFriSlotUp = new Util.Param1Actions<FriendPetExplorer>();
        public Util.ParamActions onChatSync = new Util.ParamActions();

        public void OnLogin() { }
        public void OnLogout() { }
        public void OnMainEnter() { }
        public void OnMainExit() { }
        
        public void SychronizeFriends(List<Friend> friends)
        {
            this.friends = friends;
            onSync.Fire();
        }
        public void Update(List<Friend> ups)
        {
            bool needSync = false;
            for (int index = 0; index < ups.Count; ++index)
            {
                var fri = ups[index];
                int exist = friends.FindIndex(f => f.id == fri.id);
                if (exist >= 0)
                {
                    onUpdate.Fire(friends[exist] = fri);
                }
                else
                {
                    friends.Add(fri);
                    needSync = true;
                }
            }
            if (needSync) { onSync.Fire(); }
        }
        public void Rmv(string friendId)
        {
            int exist = friends.FindIndex(f => f.id == friendId);
            if (exist >= 0)
            {
                friends.RemoveAt(exist);
                onSync.Fire();
            }
        }
        public void VisitSync(string friUId, List<FriendPetExplorer> friendSlots)
        {
            this.friUId = friUId;
            this.friendSlots = friendSlots;
            onFriSlotSync.Fire();
        }
        public void VisitRobbery(string friUId, int slotId)
        {
            this.friUId = friUId;
            var exist = friendSlots.FindIndex(it => it.slotId == slotId);
            var slot = friendSlots[exist];
            slot.canRobbery = false;
            onFriSlotUp.Fire(friendSlots[exist] = slot);
        }
        public void SyncChat(List<FriendChat> sy)
        {
            chats = sy;
            onChatSync.Fire();
        }
        public void UpdateChat(List<FriendChat> up)
        {
            for (int index = 0; index < up.Count; ++index)
            {
                chats.Add(up[index]);
            }
            onChatSync.Fire();
        }
    }
}
