using Factura.CameraRig;
using Factura.Combat;
using Factura.Configs;
using Factura.Core;
using Factura.Enemies;
using Factura.Input;
using Factura.Level;
using Factura.Player;
using Factura.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Factura.Installers
{
    /// <summary>
    /// Composition root. Every dependency in the game is wired here and nowhere else.
    /// </summary>
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Configs")]
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private CarConfig _carConfig;
        [SerializeField] private WeaponConfig _weaponConfig;
        [SerializeField] private EnemyConfig _enemyConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigs(builder);
            RegisterServices(builder);
            RegisterSceneComponents(builder);

            builder.RegisterEntryPoint<GameFlow>();

            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<GroundTiler>();
                container.Resolve<HudView>();
                container.Resolve<GameResultView>();
            });
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelConfig);
            builder.RegisterInstance(_carConfig);
            builder.RegisterInstance(_weaponConfig);
            builder.RegisterInstance(_enemyConfig);
        }

        private static void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<PointerInputService>(Lifetime.Singleton).As<IInputService>();
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.Register<LevelProgress>(Lifetime.Singleton);
            builder.Register<ProjectileService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ScoreService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        /// <summary>
        /// Scene components are registered both as themselves and as their interfaces, so
        /// anything implementing <see cref="IResettable"/> is picked up by the restart flow
        /// automatically.
        /// </summary>
        private static void RegisterSceneComponents(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CarController>().AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<CarFollowCamera>().AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<TurretAim>().AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<TurretShooter>().AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<EnemySpawner>().AsSelf().AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<GroundTiler>().AsSelf();
            builder.RegisterComponentInHierarchy<HudView>().AsSelf();
            builder.RegisterComponentInHierarchy<GameResultView>().AsSelf();
        }
    }
}
