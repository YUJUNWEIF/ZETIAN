using System;
using System.Collections.Generic;

namespace geniusbaby
{
	public class ControllerManager : Singleton<ControllerManager>
	{
        private List<IGameEvent> m_controllers = new List<IGameEvent>();
        public void StartGame()
        {
            m_controllers.Add(new archive.SQLiteTableManager());
            for (int index = 0; index < m_controllers.Count; ++index)
            {
                m_controllers[index].OnStartGame();
            }
        }
        public void StopGame()
        {
            for (int index = 0; index < m_controllers.Count; ++index)
            {
                m_controllers[index].OnStopGame();
            }
            m_controllers.Clear();
        }
        public void Regist(IGameEvent ev)
        {
            m_controllers.Add(ev);
        }
    }
}
