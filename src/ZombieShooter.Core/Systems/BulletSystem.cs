using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using System;
using System.Diagnostics;
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
    
    // Store sprite reference for creating SpriteComponents
    readonly Sprite _bulletSprite;
    readonly float _bulletSpriteDepth = 0.3f;
    readonly float _bulletColliderRadius = 0.4f;
    readonly float _bulletScale = 2f;
    
    readonly float _bulletSpeed = 200f;
    
    public BulletSystem(IGame game, BulletManager bulletManager, PlayerManager playerManager, Sprite sprite) : 
        base(Aspect.All(typeof(BulletComponent), typeof(MovementComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)))
    {
        _game = game;
        _bulletManager = bulletManager;
        _playerManager = playerManager;
        _bulletSprite = sprite;
        
        _bulletManager.OnCreateBullet = CreateBullet;
        _bulletManager.OnResetBullet = ResetBullet;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _movementMapper = mapperService.GetMapper<MovementComponent>();
    }

    public override void Update(GameTime gameTime)
    {
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
    Entity CreateBullet(Vector2 offset)
    {
        Entity bullet = CreateEntity();
        
        // Create individual component instances for each bullet
        bullet.Attach(new BulletComponent());
        bullet.Attach(new SpriteComponent(_bulletSprite, _bulletSpriteDepth));
        bullet.Attach(new CircleColliderComponent(_bulletColliderRadius));
        Transform2 transform = new Transform2(_playerManager.Position + offset);
        transform.Scale = new(_bulletScale);
        bullet.Attach(transform);
        
        MovementComponent movement = new(_bulletSpeed);
        movement.MoveDirection = _playerManager.Direction;
        movement.Direction = movement.MoveDirection;
        movement.NormalizeMoveDirection();
        bullet.Attach(movement);

        return bullet;
    }
    
    void ResetBullet(Entity entity)
    {
        entity.Attach(new DisabledComponent());
    }
    
    public new void Dispose()
    {
        base.Dispose();
        _bulletManager.OnCreateBullet = null;
        _bulletManager.OnResetBullet = null;
    }
}
