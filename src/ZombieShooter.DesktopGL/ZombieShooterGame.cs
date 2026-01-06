using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens.Transitions;
using ZombieShooter.Core;
using ZombieShooter.Core.Managers;
using ZombieShooter.DesktopGL.Scenes.Menus;
using ZombieShooter.DesktopGL.Scenes.Playing;

namespace ZombieShooter.DesktopGL
{
    public class ZombieShooterGame : GameECSBase
    {
        protected override void OnInitialize(IServiceCollection services)
        {
            #region Scenes Registration
            services.AddTransient<MainMenu>();
            services.AddTransient<Stage1>();
            #endregion
            #region Managers Registration
            services.AddSingleton<PlayerManager>();
            services.AddSingleton<EnemyManager>();
            services.AddSingleton<BulletManager>();
            services.AddSingleton<SoundManager>();
            #endregion
            _cameraZoom = 3f;
        }
        protected override void AfterInitialize()
        {
            LoadScreen<MainMenu>(new FadeTransition(_graphics.GraphicsDevice, Color.Black));
        }

        protected override void OnLoadContent()
        {
            
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            
        }
        protected override void OnDraw(GameTime gameTime)
        {
            
        }
    }
}
