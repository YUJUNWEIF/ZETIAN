using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace geniusbaby
{
    public enum PetBuildingFunc
    {
        Slot1 = 1,
        Slot2 = 2,
        Slot3 = 3,
        Merge = 4,
    }

    public class PetSceneManager : Singleton<PetSceneManager>, IGameEvent, ISceneManager
    {
        PetBuilding[] m_buildings;
        public void OnStartGame() { }
        public void OnStopGame() { }
        public void Enter()
        {
            SceneManager.Inst().EnterMap("pet", null, PreLoad);
        }
        public void Leave()
        {
            for (int index = 0; index < m_buildings.Length; ++index)
            {
                m_buildings[index].UnInitialize();
            }
            SceneManager.Instance.LeaveMap();
        }
        public IEnumerator PreLoad()
        {
            m_buildings = SceneManager.Inst().terrain.GetComponentsInChildren<PetBuilding>();
            for (int index = 0; index < m_buildings.Length; ++index)
            {
                m_buildings[index].Initialize(true);
            }
            yield return null;
        }
    }
}
