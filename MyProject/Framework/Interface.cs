using System;
using System.Collections.Generic;

namespace geniusbaby
{
    public interface IGameEvent
    {
        void OnStartGame();
        void OnStopGame();
    }
}