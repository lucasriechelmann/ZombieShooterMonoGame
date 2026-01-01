using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core.Systems;

public class CollisionSystem : EntityUpdateSystem
{
    ComponentMapper<CircleColliderComponent> _circleColliderMapper;
    ComponentMapper<MovementComponent> _movementMapper;
    ComponentMapper<Transform2> _transformMapper;
    IGame _game;
    public CollisionSystem(IGame game) : base(Aspect.All(typeof(CircleColliderComponent), typeof(MovementComponent), typeof(Transform2)))
    {
        _game = game;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _circleColliderMapper = mapperService.GetMapper<CircleColliderComponent>();
        _movementMapper = mapperService.GetMapper<MovementComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach(int entityIdA in ActiveEntities)
        {
            Transform2 transformA = _transformMapper.Get(entityIdA);
            CircleColliderComponent colliderA = _circleColliderMapper.Get(entityIdA);
            MovementComponent movementA = _movementMapper.Get(entityIdA);
            Vector2 futurePositionA = transformA.Position + movementA.MoveDirection * movementA.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            CircleF futureColliderA = new CircleF(futurePositionA + colliderA.Collider.Center, colliderA.Collider.Radius);

            foreach (int entityIdB in ActiveEntities)
            {
                if (entityIdA == entityIdB)
                    continue;

                Transform2 transformB = _transformMapper.Get(entityIdB);
                CircleColliderComponent colliderB = _circleColliderMapper.Get(entityIdB);
                MovementComponent movementB = _movementMapper.Get(entityIdB);
                Vector2 futurePositionB = transformB.Position + movementB.MoveDirection * movementB.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                CircleF futureColliderB = new CircleF(futurePositionB + colliderB.Collider.Center, colliderB.Collider.Radius);

                Vector2 difference = futurePositionA - futurePositionB;
                float distanceSq = difference.LengthSquared();
                float combinedRadius = futureColliderA.Radius + futureColliderB.Radius;

                if (distanceSq < combinedRadius * combinedRadius)
                {
                    float distance = MathF.Sqrt(distanceSq);

                    if(distance == 0f)
                    {
                        difference = Vector2.UnitX;
                        distance = 1f;
                    }

                    Vector2 collisionNormal = difference / distance;
                    float overlap = combinedRadius - distance;
                    Vector2 separationVector = collisionNormal * overlap;
                    movementA.SetMoveDirectionX(movementA.MoveDirection.X + separationVector.X / 2f);
                    movementA.SetMoveDirectionY(movementA.MoveDirection.Y + separationVector.Y / 2f);
                    movementB.SetMoveDirectionX(movementB.MoveDirection.X - separationVector.X / 2f);
                    movementB.SetMoveDirectionY(movementB.MoveDirection.Y - separationVector.Y / 2f);
                }
            }
        }
    }
}
