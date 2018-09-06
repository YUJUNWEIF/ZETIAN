using System;
using System.Collections.Generic;

namespace geniusbaby.LSharpScript
{
    public class HotcodeFramework
    {
        private List<IGameEvent> m_controllers = new List<IGameEvent>();
        private Obj3dProc m_obj3dProc = new Obj3dProc();
        private LGAPManager m_wordMgr = new LGAPManager();

        private tab.CSVTableManager m_csvManager = new tab.CSVTableManager();
        private tab.__title m_tt = new tab.__title();
        private AIManager m_aimanager = new AIManager();
        private FEScene2d m_splineMgr = new FEScene2d();
        private MonsterArrayManager m_arrayMgr = new MonsterArrayManager();
        private DictManager m_dictManager = new DictManager();
        private ClientLockStep m_lockStep;
        private FEModule m_app = new FEModule();
        private HttpNetwork m_httpNet = new HttpNetwork();
        private TcpNetwork m_tcpNet = new TcpNetwork();

        public void StartGame()
        {
            //ModuleManager.Inst().Regist(new LoginModule());
            //ModuleManager.Inst().Regist(new PlayerModule());
            //ModuleManager.Inst().Regist(new PVEModule());
            ModuleManager.Inst().Regist(new AchieveModule());
            ModuleManager.Inst().Regist(new LoginModule());
            ModuleManager.Inst().Regist(new PlayerModule());
            ModuleManager.Inst().Regist(new GunModule());
            ModuleManager.Inst().Regist(new PackageModule());
            ModuleManager.Inst().Regist(new MapInfoModule());
            ModuleManager.Inst().Regist(new PVEModule());
            ModuleManager.Inst().Regist(new PvpRoomModule());
            ModuleManager.Inst().Regist(new ArchiveModule());
            ModuleManager.Inst().Regist(new PetModule());
            ModuleManager.Inst().Regist(new TitleModule());
            ModuleManager.Inst().Regist(new KnowledgeModule());
            ModuleManager.Inst().Regist(new TokenModule());
            ModuleManager.Inst().Regist(new ChatModule());
            ModuleManager.Inst().Regist(new FriendModule());
            ModuleManager.Inst().Regist(new MailModule());
            ModuleManager.Inst().Regist(new FuncTipModule());
            ModuleManager.Inst().Regist(new GuideModule());
            ModuleManager.Inst().Regist(new ShopModule());
            ModuleManager.Inst().Regist(new EnglishDictModule());
            ModuleManager.Inst().Regist(new MsgScrollBarModule());

            ControllerManager.Inst().Regist(new FightSceneManager());
            ControllerManager.Inst().Regist(new QuizSceneManager());
            ControllerManager.Inst().Regist(new ChatManager());
            ControllerManager.Inst().Regist(new PetSceneManager());
            ControllerManager.Inst().Regist(new FriendPetSceneManager());
            ControllerManager.Inst().Regist(new PrefabControl());
            ControllerManager.Inst().Regist(new NetworkWatcher());

            new Net.ProtoManager(new Net.ProtoCacher());
            Net.NetHelper.serializer = new ProtoSerializer();
            pps.ProtoReg.Register();
            pps.ProtoImplReg.Register();
            m_lockStep = new ClientLockStep();
            WaitSeconds.timeGetter = TimeGet;

            var reader = Framework.Inst().reader;
            Framework.Inst().reader.Parse("cfg/config.zip", (name, bytes) =>
            {
                switch (name)
                {
                    case GlobalDefine.ConfigContent.binXls: m_csvManager.LoadBinaryZipFile(bytes); break;
                    case GlobalDefine.ConfigContent.binMap: FEScene2d.Inst().LoadBinaryZipFile<FECacheSpline>(bytes); break;
                    case GlobalDefine.ConfigContent.globalParam: GlobalParam.LoadFromMemory(bytes); break;
                    case GlobalDefine.ConfigContent.binAI: m_aimanager.LoadBinaryZipFile(bytes); break;
                    case GlobalDefine.ConfigContent.binArray: MonsterArrayManager.Inst().LoadBinaryZipFile(bytes); break;
                }
            });
            LGAPManager.Inst().LoadLGAPInfo(Framework.Inst().reader);

            m_httpNet.StartGame();
        }
        public void StopGame()
        {
            for (int index = 0; index < m_controllers.Count; ++index)
            {
                m_controllers[index].OnStopGame();
            }
            m_controllers.Clear();

            m_dictManager.StopGame();
            m_httpNet.StopGame();
        }
        public void EnterGame()
        {
            StateManager.Instance.ChangeState<LoginState>();
        }
        public void OnClick(IBaseObj obj3d)
        {
            m_obj3dProc.OnClick(obj3d);
        }
        float TimeGet(TimeType type)
        {
            switch (type)
            {
                case TimeType.Time: return UnityEngine.Time.time;
                case TimeType.RealTime: return UnityEngine.Time.realtimeSinceStartup;
                case TimeType.SyncTime: return FightSceneManager.mod.timeMs;
                case TimeType.DisplayTime: return ClientLockStep.displayTimeMs;
            }
            return 0;
        }
    }
    public class T
    {
        public static Tt As<Tt>(BehaviorWrapper script) where Tt : ILSharpScript
        {
            return (script as LSharpAPI).hotfixScript as Tt;
        }
        public static Tt As<Tt>(LSharpItemPanel script) where Tt : ILSharpScript
        {
            return script.hotfixScript as Tt;
        }
        public static List<object> L<T>(IList<T> values)
        {
            var cv = new List<object>(values.Count);
            for (int index = 0; index < values.Count; ++index) { cv.Add(values[index]); }
            return cv;
        }
        public static void L<T>(IList<T> values, List<object> cv)
        {
            for (int index = 0; index < values.Count; ++index) { cv.Add(values[index]); }
        }
    }
}
