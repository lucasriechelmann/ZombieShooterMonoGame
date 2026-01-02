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
    PlayerManager _playerManager;
    EnemyManager _enemyManager;
    IGame _game;
    public CollisionSystem(IGame game, PlayerManager playerManager, EnemyManager enemyManager) : base(Aspect.All(typeof(CircleColliderComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)))
    {
        _game = game;
        _playerManager = playerManager;
        _enemyManager = enemyManager;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _circleColliderMapper = mapperService.GetMapper<CircleColliderComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
        _playerMapper = mapperService.GetMapper<PlayerComponent>();
        _enemyMapper = mapperService.GetMapper<EnemyComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        int playerId = ActiveEntities.Where(e => _playerMapper.Has(e)).FirstOrDefault();
        Transform2 playerTransform = _transformMapper.Get(playerId);
        CircleColliderComponent playerCollider = _circleColliderMapper.Get(playerId);

        if (playerCollider is null || playerTransform is null)
            return;

        List<int> enemyIds = ActiveEntities.Where(e => _enemyMapper.Has(e)).ToList();

        foreach (int enemyIdA in enemyIds)
        {
            Transform2 transformA = _transformMapper.Get(enemyIdA);
            CircleColliderComponent colliderA = _circleColliderMapper.Get(enemyIdA);

            foreach (int enemyIdB in enemyIds)
            {
                if (enemyIdA == enemyIdB)
                    continue;

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

            Vector2 playerDifference = transformA.Position - playerTransform.Position;
            float playerDistanceSq = playerDifference.LengthSquared();
            float playerCombinedRadius = colliderA.Collider.Radius + playerCollider.Collider.Radius;
            if (playerDistanceSq < playerCombinedRadius * playerCombinedRadius)
                _playerManager.Hit(1);

        }
    }
}
