using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using ZombieShooter.Core.Contracts;
using ZombieShooter.Core.Scene;

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
    public int ScreenWidth { get; private set; }
    public int ScreenHeight { get; private set; }
    public GameECSBase(int width = 1280, int heigth = 720)
    {
        _graphics = new GraphicsDeviceManager(this);
        SetScreenSize(width, heigth);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    protected override void Initialize()
    {
        _screenManager = new();
        ServiceCollection services = new();
        services.AddSingleton<IGame>(this);
        services.AddSingleton<GameECSBase>(this);
        services.AddSingleton(_screenManager);
        services.AddSingleton(Services.GetRequiredService<IGraphicsDeviceService>());
        services.AddSingleton(GraphicsDeviceManager);
        OnInitialize(services);
        _serviceProvider = services.BuildServiceProvider();
        AfterInitialize();
        base.Initialize();
    }
    protected abstract void OnInitialize(IServiceCollection services);
    protected abstract void AfterInitialize();
    protected override void LoadContent()
    {
        OnLoadContent();
    }
    protected abstract void OnLoadContent();
    protected override void Update(GameTime gameTime)
    {
        ProcessScreenToLoad();
        _screenManager.Update(gameTime);

        OnUpdate(gameTime);
        
        base.Update(gameTime);
    }
    protected abstract void OnUpdate(GameTime gameTime);

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_clearColor);

        _screenManager.Draw(gameTime);

        OnDraw(gameTime);
        
        base.Draw(gameTime);
    }
    protected abstract void OnDraw(GameTime gameTime);
    protected T GetService<T>() => _serviceProvider.GetRequiredService<T>();
    public void SetScreenSize(int width, int height)
    {
        ScreenHeight = height;
        ScreenWidth = width;
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
    }
    record ScreenLoad(Type Screen, Transition Transition, bool ClearScenes);
    public void LoadScreen<T>(Transition transition, bool clearScenes = true) where T : SceneBase
    {
        _currentScreenToLoadDetails = new(typeof(T), transition, clearScenes);
    }
    ScreenLoad _currentScreenToLoadDetails;
    SceneBase _currentScreenToLoad;
    void ProcessScreenToLoad()
    {
        if (_currentScreenToLoadDetails is null)
            return;

        if (_currentScreenToLoad is null)
            _currentScreenToLoad = _serviceProvider.GetRequiredService(_currentScreenToLoadDetails.Screen) as SceneBase;

        _currentScreenToLoad.LoadContent(2);

        if (_currentScreenToLoad.IsInitialized)
        {
            if (_currentScreenToLoadDetails.ClearScenes)
                _screenManager.ClearScreens();

            _screenManager.ShowScreen(_currentScreenToLoad, _currentScreenToLoadDetails.Transition);

            _currentScreenToLoad = null;
            _currentScreenToLoadDetails = null;
        }
    }
}
