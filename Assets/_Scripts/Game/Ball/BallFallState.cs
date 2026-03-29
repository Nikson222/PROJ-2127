using _Scripts._Infrastructure.StateMachine;

namespace _Scripts.Game.Ball
{
    public class BallFallState : IState
    {
        private readonly BallController _ball;

        public BallFallState(BallController ball)
        {
            _ball = ball;
        }

        public void Enter()
        {
            _ball.EnableGravity();
        }

        public void Exit() { }
    }
}