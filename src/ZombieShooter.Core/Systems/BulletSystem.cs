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

public class BulletSystem : EntityUpdateSystem, IDisposable
{
    IGame _game;
    BulletManager _bulletManager;
    PlayerManager _playerManager;
    ComponentMapper<Transform2> _transformMapper;
    ComponentMapper<MovementComponent> _movementMapper;
    
    // Flyweight: Shared immutable components (marker types that don't hold state)
    readonly BulletComponent _sharedBulletComponent;
    readonly SpriteComponent _sharedSpriteComponent;
    readonly CircleColliderComponent _sharedColliderComponent;
    readonly DisabledComponent _sharedDisabledComponent;
    
    readonly Vector2 _offset;
    readonly float _bulletSpeed = 200f;
    
    public BulletSystem(IGame game, BulletManager bulletManager, PlayerManager playerManager, Sprite sprite) : 
        base(Aspect.All(typeof(BulletComponent), typeof(MovementComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)))
    {
        _game = game;
        _bulletManager = bulletManager;
        _playerManager = playerManager;
        
        // Flyweight: Create shared instances for immutable marker components
        _sharedBulletComponent = new BulletComponent();
        _sharedSpriteComponent = new SpriteComponent(sprite, 0.4f);
        _sharedColliderComponent = new CircleColliderComponent(0.2f);
        _sharedDisabledComponent = new DisabledComponent();
        
        _bulletManager.OnCreateBullet = CreateBullet;
        _bulletManager.OnResetBullet = ResetBullet;
        _bulletManager.OnFreeBullet = FreeBullet;
        _offset = Vector2.Zero;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _movementMapper = mapperService.GetMapper<MovementComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        _bulletManager.ProcessShooting(gameTime);

        foreach (int entityId in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            
            int widht = (int)(_game.ScreenWidth * 1.5f);
            int height = (int)(_game.ScreenHeight * 1.5f);

            if(transform.Position.X - 32 < 0 || transform.Position.X + 32 > widht || 
               transform.Position.Y - 32 < 0 || 
               transform.Position.Y + 32 > height)
            {
                Entity entity = GetEntity(entityId);
                _bulletManager.OutsiteWorld(entity);
            }
        }
    }
    Entity CreateBullet()
    {
        Entity bullet = CreateEntity();
        
        // Flyweight: Attach shared immutable components
        bullet.Attach(_sharedBulletComponent);
        bullet.Attach(_sharedSpriteComponent);
        bullet.Attach(_sharedColliderComponent);
        
        // Extrinsic state: Only create new instances for components with mutable state
        bullet.Attach(new Transform2(_playerManager.Position + _offset));
        MovementComponent movement = new(_bulletSpeed);
        movement.MoveDirection = _playerManager.Direction;
        movement.Direction = movement.MoveDirection;
        movement.NormalizeMoveDirection();
        bullet.Attach(movement);

        return bullet;
    }
    
    void ResetBullet(Entity entity)
    {
        entity.Detach<DisabledComponent>();
        Transform2 transform = entity.Get<Transform2>();
        transform.Position = _playerManager.Position + _offset;
        MovementComponent movement = entity.Get<MovementComponent>();
        movement.MoveDirection = _playerManager.Direction;
        movement.NormalizeMoveDirection();
        movement.Direction = movement.MoveDirection;
    }
    
    void FreeBullet(Entity entity)
    {
        entity.Attach(_sharedDisabledComponent);
    }
    
    public new void Dispose()
    {
        base.Dispose();
        _bulletManager.OnCreateBullet = null;
        _bulletManager.OnResetBullet = null;
        _bulletManager.OnFreeBullet = null;
    }
}
