using _Scripts._Infrastructure.Extensions;
using _Scripts.Game;
using _Scripts.Game.Ball;
using _Scripts.Game.Colors;
using _Scripts.Game.Effects;
using _Scripts.Game.Platform;
using _Scripts.Game.States;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [Header("Scene References")]
        [SerializeField] private PlatformController _platformPrefab;
        [SerializeField] private PlatformManager _platformManager;
        [SerializeField] private Transform _platformParent;
        [SerializeField] private BallController _ballController;
        [SerializeField] private CameraSizeInitializer _cameraSizeInitializer;

        [Header("Settings")]
        [SerializeField] private PlatformSettings _platformSettings;
        [SerializeField] private GameColorConfig gameColorConfig;
        [SerializeField] private EffectIconConfig _effectIconConfig;
        [SerializeField] private EffectIconView _effectIconPrefab;
        [SerializeField] private FloatingParticle _floatingParticlePrefab;
        [SerializeField] private ParticleSystemController _particleSystemController;

        public override void InstallBindings()
        {
            BindSettings();
            BindEffects();
            BindPlatformSystem();
            BindInput();

            BindStateMachines();
            BindGameStates();

            BindCameraInitializer();

            Container.Resolve<GameStateMachine>().Enter<GameLoopState>();
        }

        private void BindSettings()
        {
            Container.BindInstance(_platformSettings).AsSingle();
            Container.BindInstance(gameColorConfig).AsSingle();
            Container.BindInstance(_effectIconConfig).AsSingle();
            
            Container.Bind<GameColorService>().AsSingle();

            Container.BindInstance(_platformParent).WithId("PlatformParent");
            Container.BindInstance(_ballController).AsSingle();
            
            Container.BindMemoryPool<EffectIconView, EffectIconView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_effectIconPrefab);

            Container.Bind<EffectIconService>().AsSingle();
            
            Container.BindMemoryPool<FloatingParticle, FloatingParticle.Pool>()
                .WithInitialSize(50)
                .FromComponentInNewPrefab(_floatingParticlePrefab)
                .UnderTransformGroup("Particles");
    
            Container.Bind<ParticleSystemController>().FromInstance(_particleSystemController).AsSingle();
        }

        private void BindEffects()
        {
            Container.Bind<IEffectStrategy>().To<NoEffect>().AsSingle();
            Container.Bind<IEffectStrategy>().To<SpeedBoostEffect>().AsSingle();
            Container.Bind<IEffectStrategy>().To<SlowDownEffect>().AsSingle();
            Container.Bind<IEffectStrategy>().To<ExtraPointsEffect>().AsSingle();

            Container.Bind<EffectStrategyFactory>().AsSingle();
        }

        private void BindPlatformSystem()
        {
            Container.BindMemoryPool<PlatformController, PlatformController.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_platformPrefab)
                .UnderTransform(_platformParent);

            Container.Bind<RandomEffectSelector>().AsSingle();

            Container.Bind<PlatformSpawner>().AsSingle()
                .WithArguments(
                    Container.Resolve<PlatformController.Pool>(),
                    _platformParent,
                    Container.Resolve<RandomEffectSelector>()
                );

            Container.Bind<PlatformMover>().AsSingle()
                .WithArguments(
                    _platformSettings.ShiftDistance,
                    _platformSettings.ShiftDuration
                );

            Container.BindInterfacesAndSelfTo<PlatformManager>().FromInstance(_platformManager).AsSingle();
        }

        private void BindInput()
        {
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        }

        private void BindStateMachines()
        {
            Container.BindInterfacesAndSelfTo<BallStateMachine>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();
        }

        private void BindGameStates()
        {
            Container.BindAndRegisterState<BallIdleState, BallStateMachine>();
            Container.BindAndRegisterState<BallFallState, BallStateMachine>();
            Container.BindAndRegisterState<BallDeadState, BallStateMachine>();

            Container.BindAndRegisterState<GameLoopState, GameStateMachine>();
            Container.BindAndRegisterState<GameOverState, GameStateMachine>();
        }

        private void BindCameraInitializer()
        {
            Container.BindInstance(_cameraSizeInitializer).AsSingle();
        }
    }
}
