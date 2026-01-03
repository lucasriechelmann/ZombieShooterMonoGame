using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS;
using System;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Managers;

public class EnemyManager
{
    public Func<Entity> OnCreateEnemy;
    public Action<Entity> OnResetEnemy;
    Pool<Entity> _enemies;
    DisabledComponent _disabledComponent;
    bool _isSpawning;
    
    public EnemyManager()
    {
        _isSpawning = true;
        _enemies = new(CreateEnemy, ResetEntity);
        _disabledComponent = new();
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
        enemy.Attach(_disabledComponent);
    }
}
