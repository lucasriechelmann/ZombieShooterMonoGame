using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS;
using System;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Managers;

public class EnemyManager
{
    public Func<Entity> OnCreateEnemy;
    public Action<Entity> OnResetEnemy;
    public Action<Entity> OnHitEnemy;
    Pool<Entity> _enemies;
    bool _isSpawning;
    
    public EnemyManager()
    {
        _isSpawning = true;
        _enemies = new(CreateEnemy, ResetEntity);
    }
    
    public void Spawn()
    {
        if (!_isSpawning)
            return;

        _isSpawning = false;

        for(int i = 0; i < 100; i++)
        {
            _enemies.Obtain();
        }
    }
    
    Entity CreateEnemy() => OnCreateEnemy?.Invoke();
    void ResetEntity(Entity entity) => OnResetEnemy?.Invoke(entity);
    
    public void HitEnemy(Entity enemy)
    {
        _enemies.Free(enemy);
        OnHitEnemy?.Invoke(enemy);
    }
}
