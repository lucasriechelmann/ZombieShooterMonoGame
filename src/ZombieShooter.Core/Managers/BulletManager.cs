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
        _offset = new(5,0);
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
    public void Shoot()
    {
        if (_bulletSpawnTimer > 0f)
            return;

        Entity bullet = _bullets.Obtain();

        if (bullet is null)
            return;

        Transform2 transform = bullet.Get<Transform2>();
        transform.Position = GetMuzzlePosition();
        MovementComponent movement = bullet.Get<MovementComponent>();
        movement.MoveDirection = _playerManager.Direction;
        movement.NormalizeMoveDirection();
        movement.Direction = movement.MoveDirection;
        bullet.Detach<DisabledComponent>();
        _bulletSpawnTimer = _bulletSpawnInterval;
    }    
    public Vector2 GetMuzzlePosition()
    {
        // How far in front of the center the gun is
        float forwardOffset = 10f;
        // How far to the right of the center the gun is (use negative for left hand)
        float sideOffset = 8f;

        float rotation = MathF.Atan2(_playerManager.Direction.Y, _playerManager.Direction.X);

        // playerRotation is in Radians
        float cos = (float)Math.Cos(rotation);
        float sin = (float)Math.Sin(rotation);

        // Calculate the rotated offset
        float rotatedX = (forwardOffset * cos) - (sideOffset * sin);
        float rotatedY = (forwardOffset * sin) + (sideOffset * cos);

        // Add the offset to the player's current center position
        return new Vector2(_playerManager.Position.X + rotatedX, _playerManager.Position.Y + rotatedY);
    }
}
