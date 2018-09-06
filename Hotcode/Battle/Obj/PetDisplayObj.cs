using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

namespace geniusbaby
{
    public class PetDisplayObj : IPetObj
    {
        public PetBase player { get; private set; }
        public void Initialize(PetBase player)
        {
            this.player = player;
            Change(player.mId);
        }
    }
}