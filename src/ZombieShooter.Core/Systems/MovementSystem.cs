using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Systems;

public class MovementSystem : EntityUpdateSystem
{
    ComponentMapper<MovementComponent> _movementMapper;
    ComponentMapper<Transform2> _transformMapper;
    public MovementSystem() : base(Aspect.All(typeof(MovementComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)))
    {
        
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _movementMapper = mapperService.GetMapper<MovementComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var entity in ActiveEntities)
        {
            var movement = _movementMapper.Get(entity);
            var transform = _transformMapper.Get(entity);

            transform.Position += movement.MoveDirection * movement.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            transform.Rotation = MathF.Atan2(movement.Direction.Y, movement.Direction.X);
        }
    
    }
}
