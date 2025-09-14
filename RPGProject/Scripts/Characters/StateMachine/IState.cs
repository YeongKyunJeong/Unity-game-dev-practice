using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public interface IState
    {
        public void Enter();
        public void Enter(int dataKey);
        public void Exit();
        public void CallPhysicsUpdate();
        public void CallUpdate();

        public void OnAnimationEnterEvent();
        public void OnAnimationExitEvent();
        public void OnAnimationTransitEvent();

        public void SetDefaultState() { }
    }
}
