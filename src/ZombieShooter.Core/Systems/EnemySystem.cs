using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using System;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems;

public class EnemySystem : EntityUpdateSystem, IDisposable
{
    IGame _game;
    PlayerManager _playerManager;
    EnemyManager _enemyManager;
    ComponentMapper<MovementComponent> _movementMapper;
    ComponentMapper<Transform2> _transformMapper;
    Random _rand;
    
    // Flyweight: Shared immutable components
    readonly EnemyComponent _sharedEnemyComponent;
    readonly SpriteComponent _sharedSpriteComponent;
    readonly DisabledComponent _sharedDisabledComponent;
    
    public EnemySystem(IGame game, Sprite enemySprite, PlayerManager playerManager, EnemyManager enemyManager) : 
        base(Aspect.All(typeof(EnemyComponent), typeof(MovementComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)).Exclude(typeof(DisabledComponent)))
    {
        _game = game;
        _playerManager = playerManager;
        _enemyManager = enemyManager;
        _rand = new();
        
        // Flyweight: Create shared instances
        _sharedEnemyComponent = new EnemyComponent();
        _sharedSpriteComponent = new SpriteComponent(enemySprite);
        _sharedDisabledComponent = new DisabledComponent();
        
        _enemyManager.OnCreateEnemy = CreateEnemy;
        _enemyManager.OnResetEnemy = ResetEnemy;
        _enemyManager.OnHitEnemy = HitEnemy;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _movementMapper = mapperService.GetMapper<MovementComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        _enemyManager.Spawn();
        foreach (int entityId in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            MovementComponent movement = _movementMapper.Get(entityId);
            movement.MoveDirection = (_playerManager.Position - transform.Position);
            movement.NormalizeMoveDirection();
            movement.Direction = movement.MoveDirection;
        }
    }
    Entity CreateEnemy()
    {
        Entity enemy = CreateEntity();
        
        // Flyweight: Attach shared immutable components
        enemy.Attach(_sharedEnemyComponent);
        enemy.Attach(_sharedSpriteComponent);
        
        // Extrinsic state: Create unique instances for mutable data
        enemy.Attach(new MovementComponent(_rand.Next(50,65)));
        enemy.Attach(new Transform2(GetEnemyPosition()));
        enemy.Attach(new CircleColliderComponent(7));

        return enemy;
    }
    
    void ResetEnemy(Entity entity)
    {
        entity.Detach<DisabledComponent>();
        _transformMapper.Get(entity.Id).Position = GetEnemyPosition();
    }
    
    void HitEnemy(Entity entity)
    {
        entity.Attach(_sharedDisabledComponent);
    }
    
    Vector2 GetEnemyPosition()
    {
        int margin = 32;
        int screenHeight = (int)(_game.ScreenHeight * 1.5f);
        int screenWidth = (int)(_game.ScreenWidth * 1.5f);

        int side = _rand.Next(4);
        float x = 0;
        float y = 0;

        switch (side)
        {
            case 0: // Top
                x = _rand.Next(-margin, screenWidth + margin);
                y = -margin;
                break;
            case 1: // Bottom
                x = _rand.Next(-margin, screenWidth + margin);
                y = screenHeight + margin;
                break;
            case 2: // Left
                x = -margin;
                y = _rand.Next(-margin, screenHeight + margin);
                break;
            case 3: // Right
                x = screenWidth + margin;
                y = _rand.Next(-margin, screenHeight + margin);
                break;
        }

        return new Vector2(x, y);
    }
    public new void Dispose()
    {
        base.Dispose();
        _enemyManager.OnCreateEnemy = null;
        _enemyManager.OnResetEnemy = null;
        _enemyManager.OnHitEnemy = null;
    }
}
