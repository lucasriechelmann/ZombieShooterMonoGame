using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS;
using System;
using System.Diagnostics;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Managers;

public class BulletManager
{
    public Func<Vector2, Entity> OnCreateBullet;
    public Action<Entity> OnResetBullet;
    Pool<Entity> _bullets;
    float _bulletSpawnInterval = 0.3f;
    float _bulletSpawnTimer = 0.0f;
    const int MAX_BULLETS = 200;
    readonly Vector2 _offset;
    PlayerManager _playerManager;
    public BulletManager(PlayerManager playerManager)
    {
        _bullets = new(CreateBullet, ResetEntity, MAX_BULLETS);
        _playerManager = playerManager;
        _offset = Vector2.Zero;
    }
    Entity CreateBullet() => OnCreateBullet?.Invoke(_offset);
    void ResetEntity(Entity entity) => OnResetBullet?.Invoke(entity);
    public void HitBullet(Entity bullet)
    {
        _bullets.Free(bullet);
    }
    public void OutsiteWorld(Entity bullet) => HitBullet(bullet);
    public void ProcessShooting(GameTime gameTime)
    {
        _bulletSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if(_bulletSpawnTimer < 0f)
            _bulletSpawnTimer = 0f;
    }
    int bulletCount = 0;
    public void Shoot()
    {
        if (_bulletSpawnTimer > 0f)
            return;

        Debug.WriteLine("Shoot Bullet");
        Entity bullet = _bullets.Obtain();

        if (bullet is null)
            return;

        bulletCount++;
        Debug.WriteLine($"Bullet got. {bulletCount}");

        Transform2 transform = bullet.Get<Transform2>();
        transform.Position = _playerManager.Position + _offset;
        MovementComponent movement = bullet.Get<MovementComponent>();
        movement.MoveDirection = _playerManager.Direction;
        movement.NormalizeMoveDirection();
        movement.Direction = movement.MoveDirection;
        bullet.Detach<DisabledComponent>();
        _bulletSpawnTimer = _bulletSpawnInterval;
    }
}
