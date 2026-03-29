using _Scripts._Infrastructure.StateMachine;
using System;
using Zenject;

namespace _Scripts.Game.Ball
{
    public class BallIdleState : IState, IDisposable
    {
        private readonly InputController _input;
        private readonly BallStateMachine _stateMachine;

        public BallIdleState(InputController input, BallStateMachine stateMachine)
        {
            _input = input;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _input.Clicked += OnClicked;
        }

        public void Exit()
        {
            _input.Clicked -= OnClicked;
        }

        private void OnClicked()
        {
            _stateMachine.Enter<BallFallState>();
        }

        public void Dispose()
        {
            _input.Clicked -= OnClicked;
        }
    }
}