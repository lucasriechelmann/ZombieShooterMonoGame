using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS;
using System;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Managers;

public class BulletManager
{
    public Func<Entity> OnCreateBullet;
    public Action<Entity> OnResetBullet;
    public Action<Entity> OnFreeBullet;
    Pool<Entity> _bullets;
    float _bulletSpawnInterval = 0.3f;
    float _bulletSpawnTimer = 0.0f;
    const int MAX_BULLETS = 200;
    public BulletManager()
    {
        _bullets = new(CreateBullet, ResetEntity, MAX_BULLETS);
    }
    Entity CreateBullet() => OnCreateBullet?.Invoke();
    void ResetEntity(Entity entity) => OnResetBullet?.Invoke(entity);
    public void HitBullet(Entity bullet)
    {
        _bullets.Free(bullet);
        OnFreeBullet?.Invoke(bullet);
    }
    public void OutsiteWorld(Entity bullet) => HitBullet(bullet);
    public void ProcessShooting(GameTime gameTime)
    {
        if(_bulletSpawnTimer < 0f)
        {
            _bulletSpawnTimer = 0f;
            return;
        }

        _bulletSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void Shoot()
    {
        if (_bulletSpawnTimer > 0f)
            return;

        _bullets.Obtain();
        _bulletSpawnTimer = _bulletSpawnInterval;
    }
}
