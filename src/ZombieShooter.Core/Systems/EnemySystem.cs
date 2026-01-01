using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems;

public class EnemySystem : EntityUpdateSystem
{
    World _world;
    PlayerManager _playerManager;
    EnemyManager _enemyManager;
    ComponentMapper<MovementComponent> _movementMapper;
    ComponentMapper<Transform2> _transformMapper;
    public EnemySystem(World world, PlayerManager playerManager, EnemyManager enemyManager) : 
        base(Aspect.All(typeof(EnemyComponent), typeof(MovementComponent), typeof(Transform2)))
    {
        _world = world;
        _playerManager = playerManager;
        _enemyManager = enemyManager;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _movementMapper = mapperService.GetMapper<MovementComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (int entityId in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            MovementComponent movement = _movementMapper.Get(entityId);
            movement.MoveDirection = (_playerManager.Position - transform.Position);
            movement.NormalizeMoveDirection();
            movement.Direction = movement.MoveDirection;
        }
    }
    protected override void OnEntityAdded(int entityId)
    {
        
    }
    protected override void OnEntityRemoved(int entityId)
    {
        
    }

}
