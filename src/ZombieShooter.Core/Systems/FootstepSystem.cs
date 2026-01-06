using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System;
using System.Collections.Generic;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems;

public class FootstepSystem : EntityUpdateSystem
{
    ComponentMapper<Transform2> _transformMapper;
    ComponentMapper<FootstepComponent> _footstepMapper;
    Dictionary<int, Vector2> _lastPositions = new();
    PlayerManager _playerManager;
    SoundManager _soundManager;
    public FootstepSystem(PlayerManager playerManager, SoundManager soundManager) : base(Aspect.All(typeof(Transform2), typeof(FootstepComponent)))
    {
        _playerManager = playerManager;
        _soundManager = soundManager;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _footstepMapper = mapperService.GetMapper<FootstepComponent>();
    }

    public override void Update(GameTime gameTime)
    {
        _soundManager.CleanSFX();
        _soundManager.SetPosition(new(_playerManager.Position.X, _playerManager.Position.Y, 0));

        foreach(int entityId in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            FootstepComponent footsteps = _footstepMapper.Get(entityId);
            footsteps.SetPosition(new(transform.Position.X, transform.Position.Y, 0));

            if(_lastPositions.TryGetValue(entityId, out Vector2 lastPosition))
            {
                float distanceMoved = Vector2.Distance(lastPosition, transform.Position);
                footsteps.AddToAcumulator(distanceMoved);

                if (footsteps.ShouldPlay())
                {
                    _soundManager.Play(footsteps.SFX, footsteps.Emitter);
                    footsteps.ResetAcumulator();
                }
            }

            _lastPositions[entityId] = transform.Position;
        }
    }
}
