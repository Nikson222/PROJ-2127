using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game.States;
using Zenject;

namespace _Scripts.Game.Ball
{
    public class BallDeadState : IState
    {
        private GameStateMachine _gameStateMachine;

        [Inject]
        public void Construct(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            _gameStateMachine.Enter<GameOverState>();
        }

        public void Exit() { }
    }
}