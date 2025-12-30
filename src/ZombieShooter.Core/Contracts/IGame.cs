using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using System;

namespace ZombieShooter.Core.Contracts;

public interface IGame
{
    GraphicsDeviceManager GraphicsDeviceManager { get; }
    GraphicsDevice GraphicsDevice { get; }
    IServiceProvider ServiceProvider { get; }
    GameECSBase Game { get; }
    ScreenManager ScreenManager { get; }
}
