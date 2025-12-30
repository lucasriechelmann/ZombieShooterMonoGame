using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using ZombieShooter.Core.Scene;

namespace ZombieShooter.Core.Contracts;

public interface IGame
{
    GraphicsDeviceManager GraphicsDeviceManager { get; }
    GraphicsDevice GraphicsDevice { get; }
    IServiceProvider ServiceProvider { get; }
    GameECSBase Game { get; }
    ScreenManager ScreenManager { get; }
    int ScreenWidth { get; }
    int ScreenHeight { get; }
    void SetScreenSize(int width, int height);
    void LoadScreen<T>(Transition transition, bool clearScenes = true) where T : SceneBase;
    void Exit();
}
