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
    private const float AudioDistanceScale = 45f;
    ComponentMapper<Transform2> _transformMapper;
    ComponentMapper<FootstepComponent> _footstepMapper;
    Dictionary<int, Vector2> _lastPositions = new();
    PlayerManager _playerManager;
    SoundManager _soundManager;
    public FootstepSystem(PlayerManager playerManager, SoundManager soundManager) : 
        base(Aspect.All(typeof(Transform2), typeof(FootstepComponent)).Exclude(typeof(DisabledComponent)))
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
        _soundManager.SetPosition(new(_playerManager.Position.X / AudioDistanceScale, _playerManager.Position.Y / AudioDistanceScale, 0));

        foreach(int entityId in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            FootstepComponent footsteps = _footstepMapper.Get(entityId);
            footsteps.SetPosition(new(transform.Position.X / AudioDistanceScale, transform.Position.Y / AudioDistanceScale, 0));

            if(_lastPositions.TryGetValue(entityId, out Vector2 lastPosition))
            {
                float distanceMoved = Vector2.Distance(lastPosition, transform.Position);
                
                // If the entity moved an unusually large distance in a single frame,
                // it likely teleported (e.g., spawned or reset). We reset the accumulator
                // without playing the sound.
                if (distanceMoved > 30f)
                {
                    footsteps.ResetAcumulator();
                }
                else
                {
                    footsteps.AddToAcumulator(distanceMoved);

                    if (footsteps.ShouldPlay())
                    {
                        _soundManager.Play(footsteps.SFX, footsteps.Emitter);
                        footsteps.ResetAcumulator();
                    }
                }
            }

            _lastPositions[entityId] = transform.Position;
        }
    }
}
