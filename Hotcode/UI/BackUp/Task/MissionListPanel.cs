using UnityEngine;

namespace geniusbaby.ui
{
    public class MissionListPanel : ListContainer<MissionItemPanel, MissionItemPanel, CombatMission>
    {
        public void SetColor(Color color)
        {
            for (int index = 0; index < count; ++index)
            {
                GetItem(index).SetColor(color);
            }
        }
    }
}