using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems;

public class CollisionSystem : EntityUpdateSystem
{
    ComponentMapper<CircleColliderComponent> _circleColliderMapper;
    ComponentMapper<Transform2> _transformMapper;
    ComponentMapper<PlayerComponent> _playerMapper;
    ComponentMapper<EnemyComponent> _enemyMapper;
    ComponentMapper<BulletComponent> _bulletMapper;
    PlayerManager _playerManager;
    EnemyManager _enemyManager;
    BulletManager _bulletManager;
    IGame _game;
    public CollisionSystem(IGame game, PlayerManager playerManager, EnemyManager enemyManager, BulletManager bulletManager) : 
        base(Aspect.All(typeof(CircleColliderComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)))
    {
        _game = game;
        _playerManager = playerManager;
        _enemyManager = enemyManager;
        _bulletManager = bulletManager;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _circleColliderMapper = mapperService.GetMapper<CircleColliderComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
        _playerMapper = mapperService.GetMapper<PlayerComponent>();
        _enemyMapper = mapperService.GetMapper<EnemyComponent>();
        _bulletMapper = mapperService.GetMapper<BulletComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        var activeEntitiesList = ActiveEntities.ToList();               
        int playerId = activeEntitiesList.FirstOrDefault(e => _playerMapper.Has(e));
        Transform2 playerTransform = _transformMapper.Get(playerId);
        CircleColliderComponent playerCollider = _circleColliderMapper.Get(playerId);

        if (playerCollider is null || playerTransform is null)
            return;

        List<int> enemyIds = activeEntitiesList.Where(e => _enemyMapper.Has(e)).ToList();
        List<int> bulletIds = activeEntitiesList.Where(e => _bulletMapper.Has(e)).ToList();

        // Enemy-to-enemy collision (separation)
        for (int i = 0; i < enemyIds.Count; i++)
        {
            int enemyIdA = enemyIds[i];
            Transform2 transformA = _transformMapper.Get(enemyIdA);
            CircleColliderComponent colliderA = _circleColliderMapper.Get(enemyIdA);

            for (int j = i + 1; j < enemyIds.Count; j++)
            {
                int enemyIdB = enemyIds[j];
                Transform2 transformB = _transformMapper.Get(enemyIdB);
                CircleColliderComponent colliderB = _circleColliderMapper.Get(enemyIdB);

                Vector2 difference = transformA.Position - transformB.Position;
                float distanceSq = difference.LengthSquared();
                float combinedRadius = colliderA.Collider.Radius + colliderB.Collider.Radius;

                if (distanceSq < combinedRadius * combinedRadius)
                {
                    float distance = MathF.Sqrt(distanceSq);
                    if (distance == 0f)
                    {
                        difference = Vector2.UnitX;
                        distance = 1f;
                    }
                    Vector2 collisionNormal = difference / distance;
                    float overlap = combinedRadius - distance;
                    Vector2 separationVector = collisionNormal * overlap;
                    transformA.Position += separationVector / 2f;
                    transformB.Position -= separationVector / 2f;
                }
            }

            // Enemy-to-player collision
            Vector2 playerDifference = transformA.Position - playerTransform.Position;
            float playerDistanceSq = playerDifference.LengthSquared();
            float playerCombinedRadius = colliderA.Collider.Radius + playerCollider.Collider.Radius;
            if (playerDistanceSq < playerCombinedRadius * playerCombinedRadius)
                _playerManager.Hit(1);

            foreach(int bulletId in bulletIds)
            {
                Transform2 bulletTransform = _transformMapper.Get(bulletId);
                CircleColliderComponent bulletCollider = _circleColliderMapper.Get(bulletId);
                Vector2 bulletDifference = transformA.Position - bulletTransform.Position;
                float bulletDistanceSq = bulletDifference.LengthSquared();
                float bulletCombinedRadius = colliderA.Collider.Radius + bulletCollider.Collider.Radius;
                if (bulletDistanceSq < bulletCombinedRadius * bulletCombinedRadius)
                {
                    Entity enemy = GetEntity(enemyIdA);
                    _enemyManager.HitEnemy(enemy);
                    Entity bullet = GetEntity(bulletId);
                    _bulletManager.HitBullet(bullet);
                }
            }
        }
    }
}
