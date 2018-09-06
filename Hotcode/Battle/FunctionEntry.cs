using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace geniusbaby
{
    public class FunctionEntry
    {
        public static void Enter(int func)
        {
            switch (func)
            {
                case Function.PVE:
                    break;
                case Function.Pet:
                    break;
                case Function.PetEntry: MainState.Instance.ChangeState<SubPetState>(); break;
                //case Function.PetMerge: GuiManager.Inst().ShowFrame<ui.PetMergeFrame>(); break;
                //case Function.PetExplorer: GuiManager.Inst().ShowFrame<ui.PetExplorerFrame>(); break;
                case Function.Pacakge: break;
                //case Function.Quiz: GuiManager.Inst().ShowFrame<ui.QuizJoinFrame>(); break;
            }
        }
        public static bool IsFuncOpen(int func)
        {
            return true;
        }
    }
}
