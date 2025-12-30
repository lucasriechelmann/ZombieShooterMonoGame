using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens.Transitions;
using ZombieShooter.Core;
using ZombieShooter.DesktopGL.Scenes.Menus;

namespace ZombieShooter.DesktopGL
{
    public class ZombieShooterGame : GameECSBase
    {
        protected override World CreateWolrd()
        {
            return new WorldBuilder()
                .Build();
        }
        protected override void OnInitialize(IServiceCollection services)
        {
            services.AddTransient<MainMenu>();
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
