using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core;

public abstract class SceneBase : GameScreen
{
    protected IServiceProvider _services;
    protected ContentManager _screenContent;
    protected Queue<Func<object>> _jobs = new();
    World _world;
    public bool IsInitialized { get; private set; }
    protected SceneBase(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<IGame>().Game)
    {
        _services = serviceProvider;
        _screenContent = new(serviceProvider, "Content");
    }
    public override void Initialize()
    {
        base.Initialize();
        OnInitialize();
        _world = CreateWorld();
    }
    protected abstract void OnInitialize();
    protected abstract World CreateWorld();
    public void AddContentToLoad<T>(string key) => _jobs.Enqueue(() => _screenContent.Load<T>(key));
    public void LoadContent(int maxPerFrame = 2)
    {
        int processed = 0;
        while (processed < maxPerFrame && _jobs.Count > 0)
        {
            Func<object> job = _jobs.Dequeue();
            try
            {
                _ = job();
            }
            catch
            {

            }
        }

        IsInitialized = true;
    }
    public override void Update(GameTime gameTime)
    {
        _world?.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        _world?.Draw(gameTime);
    }
    public override void UnloadContent()
    {
        base.UnloadContent();
        OnUnloadContent();
    }
    protected abstract void OnUnloadContent();
    public override void Dispose()
    {
        base.Dispose();
        _world?.Dispose();
        UnloadContent();
    }
}
