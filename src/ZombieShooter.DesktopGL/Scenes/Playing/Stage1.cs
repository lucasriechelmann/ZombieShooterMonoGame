using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using System;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Managers;
using ZombieShooter.Core.Scene;
using ZombieShooter.Core.Systems;

namespace ZombieShooter.DesktopGL.Scenes.Playing;

public class Stage1 : SceneECSBase
{
    Texture2DAtlas _textureAtlas;
    PlayerManager _playerManager;
    public Stage1(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        AddContentToLoad<Texture2D>("spritesheets/Sprites");
        _playerManager = serviceProvider.GetRequiredService<PlayerManager>();
    }
    protected override void OnInitialize()
    {
        _textureAtlas = Texture2DAtlas.Create("Sprites", _sceneContent.Load<Texture2D>("spritesheets/Sprites"), 32, 32);
    }
    protected override World CreateWorld()
    {
        World world = new WorldBuilder()
            .AddSystem(new PlayerInputSystem(_game))
            .AddSystem(new MovementSystem())
            .AddSystem(new CameraFollowSystem(_game))
            .AddSystem(new RenderSystem(_game))
            .Build();


        Transform2 playerTransform = new Transform2(
            _game.ScreenWidth / 2 - 16,
            _game.ScreenHeight / 2 - 16
            );

        _playerManager.SetPlayerTransform(playerTransform);

        Entity entity = world.CreateEntity();
        entity.Attach(new PlayerComponent());
        entity.Attach(new MovementComponent());
        entity.Attach(new SpriteComponent(_textureAtlas.CreateSprite(0)));
        entity.Attach(playerTransform);

        return world;
    }
    protected override void OnUnloadContent()
    {
        _textureAtlas = null;
    }
}
