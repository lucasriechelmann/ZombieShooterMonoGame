using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGameGum;
using System;

namespace ZombieShooter.Core.Scene;

public abstract class SceneUIBase : SceneBase
{
    protected GumService _gumUI => GumService.Default;
    public SceneUIBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _gumUI.Initialize(serviceProvider.GetRequiredService<GameECSBase>(), Gum.Forms.DefaultVisualsVersion.V3);
    }
    public override void Update(GameTime gameTime)
    {
        _gumUI.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        _gumUI.Draw();
    }
}
