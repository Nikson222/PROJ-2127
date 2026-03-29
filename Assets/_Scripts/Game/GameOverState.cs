using _Scripts.Game.Services;
using _Scripts._Infrastructure;
using _Scripts._Infrastructure.StateMachine;
using _Scripts._Infrastructure.UI;
using UnityEngine.SceneManagement;

namespace _Scripts.Game.States
{
    public class GameOverState : IState
    {
        private readonly PlayerProfile _playerProfile;
        private readonly ScoreCounter _scoreCounter;
        private readonly GameStateMachine _gameStateMachine;

        private UIPanelService _uiPanelService;

        public GameOverState(
            PlayerProfile playerProfile,
            ScoreCounter scoreCounter,
            GameStateMachine gameStateMachine,
            UIPanelService uiPanelService)
        {
            _playerProfile = playerProfile;
            _scoreCounter = scoreCounter;
            _gameStateMachine = gameStateMachine;
            _uiPanelService = uiPanelService;
        }

        public void Enter()
        {
            _playerProfile.SetMaxScore(_scoreCounter.Score);

            _uiPanelService.OpenPanel<GameOverPanel>();
        }

        public void Exit()
        {
        }
    }
}