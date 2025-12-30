using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using System;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core;

public abstract class GameECSBase : Game, IGame
{
    protected GraphicsDeviceManager _graphics;
    protected Color _clearColor = Color.CornflowerBlue;
    protected IServiceProvider _serviceProvider;
    protected ScreenManager _screenManager;
    public GraphicsDeviceManager GraphicsDeviceManager => _graphics;
    public IServiceProvider ServiceProvider => _serviceProvider;
    public ScreenManager ScreenManager => _screenManager;
    public GameECSBase Game => this;
    public GameECSBase()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _screenManager = new();
        ServiceCollection services = new();
        services.AddSingleton<IGame>(this);
        services.AddSingleton(_screenManager);
        OnInitialize(services);
        _serviceProvider = services.BuildServiceProvider();
        base.Initialize();
    }
    protected abstract void OnInitialize(IServiceCollection services);
    protected abstract World CreateWolrd();
    protected override void LoadContent()
    {
        OnLoadContent();
    }
    protected abstract void OnLoadContent();
    protected override void Update(GameTime gameTime)
    {
        _world.Update(gameTime);

        OnUpdate(gameTime);
        
        base.Update(gameTime);
    }
    protected abstract void OnUpdate(GameTime gameTime);

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_clearColor);

        _world.Draw(gameTime);

        OnDraw(gameTime);
        
        base.Draw(gameTime);
    }
    protected abstract void OnDraw(GameTime gameTime);
}
