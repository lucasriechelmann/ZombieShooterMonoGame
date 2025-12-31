using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core.Scene;

public abstract class SceneBase : GameScreen
{
    protected IServiceProvider _services;
    protected IGame _game;
    protected ContentManager _sceneContent;
    protected Queue<Func<object>> _jobs = new();
    
    public bool IsInitialized { get; private set; }
    protected SceneBase(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<IGame>().Game)
    {
        _services = serviceProvider;
        _game = _services.GetRequiredService<IGame>();
        _sceneContent = new(serviceProvider, "Content");
    }
    public override void Initialize()
    {
        base.Initialize();
        OnInitialize();
    }
    protected abstract void OnInitialize();
    protected void AddContentToLoad<T>(string key) => _jobs.Enqueue(() => _sceneContent.Load<T>(key));
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

        IsInitialized = _jobs.Count == 0;
    }
    
    public override void UnloadContent()
    {
        OnUnloadContent();
        base.UnloadContent();
    }
    protected abstract void OnUnloadContent();
}
