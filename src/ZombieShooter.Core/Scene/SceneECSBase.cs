using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using System;

namespace ZombieShooter.Core.Scene;

public abstract class SceneECSBase : SceneBase
{
    World _world;
    protected SceneECSBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }
    public override void Initialize()
    {
        base.Initialize();
        _world = CreateWorld();
    }
    protected abstract World CreateWorld();
    public override void Update(GameTime gameTime)
    {
        _world?.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        _world?.Draw(gameTime);
    }
    public override void Dispose()
    {
        _world?.Dispose();
        base.Dispose();
    }
}
