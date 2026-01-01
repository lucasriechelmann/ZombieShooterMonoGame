using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS;
using System;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Managers;

public class EnemyManager
{
    public Func<Entity> OnCreateEnemy;
    public Action<Entity> OnResetEntity;
    Pool<Entity> _enemies;
    public EnemyManager()
    {
        _isSpawning = true;
        _enemies = new(CreateEnemy, ResetEntity);
    }
    bool _isSpawning;
    public void Spawn()
    {
        if (!_isSpawning)
            return;

        _isSpawning = false;

        for(int i = 0; i < 30; i++)
        {
            _enemies.Obtain();
        }
    }
    Entity CreateEnemy() => OnCreateEnemy?.Invoke();
    void ResetEntity(Entity entity) => OnResetEntity?.Invoke(entity);
}
