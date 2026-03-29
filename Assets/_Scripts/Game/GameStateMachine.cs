using _Scripts._Infrastructure.StateMachine;

namespace _Scripts.Game
{
    public class GameStateMachine : StateMachine
    {
        public bool IsInState<TState>() where TState : IState
        {
            return ActiveState is TState;
        }
    }
}