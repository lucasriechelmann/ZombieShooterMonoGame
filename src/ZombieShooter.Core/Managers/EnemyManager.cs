using MonoGame.Extended.Collections;
using System;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Managers;

public class EnemyManager
{
    record Enemy(int entityId, EnemyComponent EnemyComponent);
    public Func<EnemyComponent> OnCreateEnemy;
    public Action<EnemyComponent> OnResetEnemy;
    Pool<EnemyComponent> _enemies;
    public EnemyManager()
    {
        _enemies = new(CreateEnemy, ResetEnemy);
    }
    EnemyComponent CreateEnemy() => OnCreateEnemy?.Invoke();
    void ResetEnemy(EnemyComponent enemyComponent) => OnResetEnemy?.Invoke(enemyComponent);
}
