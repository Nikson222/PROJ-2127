using _Scripts._Infrastructure.StateMachine;
using _Scripts.Game.Ball;

namespace _Scripts.Game
{
    public class GameLoopState : IState
    {
        private readonly BallStateMachine _ballStateMachine;

        public GameLoopState(BallStateMachine ballStateMachine)
        {
            _ballStateMachine = ballStateMachine;
        }

        public void Enter()
        {
            _ballStateMachine.Enter<BallIdleState>();
        }

        public void Exit() { }
    }
}