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
    EnemyManager _enemyManager;
    BulletManager _bulletManager;
    public Stage1(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        AddContentToLoad<Texture2D>("spritesheets/Sprites");
        _playerManager = serviceProvider.GetRequiredService<PlayerManager>();
        _enemyManager = serviceProvider.GetRequiredService<EnemyManager>();
        _bulletManager = serviceProvider.GetRequiredService<BulletManager>();
    }
    protected override void OnInitialize()
    {
        _textureAtlas = Texture2DAtlas.Create("Sprites", _sceneContent.Load<Texture2D>("spritesheets/Sprites"), 32, 32);
    }
    protected override World CreateWorld()
    {
        World world = new WorldBuilder()
            .AddSystem(new UpdateSystem(_playerManager, _enemyManager, _bulletManager))
            .AddSystem(new PlayerInputSystem(_game, _playerManager, _bulletManager))
            .AddSystem(new BulletSystem(_game, _bulletManager, _playerManager, _textureAtlas.CreateSprite(2)))
            .AddSystem(new EnemySystem(_game, _textureAtlas.CreateSprite(1), _playerManager, _enemyManager))
            .AddSystem(new MovementSystem())
            .AddSystem(new CollisionSystem(_game, _playerManager, _enemyManager, _bulletManager))
            .AddSystem(new CameraFollowSystem(_game))
            .AddSystem(new HUDSystem(_game, _playerManager, _textureAtlas.CreateSprite(5)))
            .AddSystem(new RenderSystem(_game))
            //.AddSystem(new RenderDebugSystem(_game))
            .Build();

        CreateTerrain(world);

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
        entity.Attach(new CircleColliderComponent(7));

        return world;
    }
    void CreateTerrain(World world)
    {
        int columns = (int)Math.Round(_game.ScreenWidth / 32 * 1.5f);
        int rows = (int)Math.Round(_game.ScreenHeight / 32 * 1.5f);

        Sprite terrainSprite = _textureAtlas.CreateSprite(4);

        for (int row = 0; row < rows; row++)
        {
            for(int column = 0; column < columns; column++)
            {
                Entity terrain = world.CreateEntity();
                terrain.Attach(new SpriteComponent(terrainSprite, 0));
                terrain.Attach(new Transform2(column * 32, row * 32));
            }
        }
    }
    protected override void OnUnloadContent()
    {
        _textureAtlas = null;
    }
}
