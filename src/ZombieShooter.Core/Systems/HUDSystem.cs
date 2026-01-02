using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using System.Collections.Generic;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems;

public class HUDSystem : EntityUpdateSystem
{
    IGame _game;
    PlayerManager _playerManager;
    ComponentMapper<Transform2> _transformMapper;
    Dictionary<int, bool> _lifes;
    SpriteComponent _lifeSpriteComponent;
    HUDComponent _hudComponent;
    DisabledComponent _disabledComponent;
    public HUDSystem(IGame game, PlayerManager playerManager, Sprite lifeSprite) : base(Aspect.All(typeof(Transform2), typeof(HUDComponent)))
    {
        _game = game;
        _playerManager = playerManager;
        _lifes = new();
        _hudComponent = new();
        _lifeSpriteComponent =  new(lifeSprite, 1);
        _disabledComponent = new();
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Update(GameTime gameTime)
    {
        CreateHUDLife();

        int count = 1;
        float y = 32;
        float x = 32;

        foreach (int hudId in ActiveEntities)
        {
            Entity hudEntity = GetEntity(hudId);

            Transform2 transform = _transformMapper.Get(hudId);
            
            transform.Position = _game.Camera.ScreenToWorld(x, y);
            x += 64;

            if (count > _playerManager.Health)
            {
                hudEntity.Attach(_disabledComponent);
            }
            else
            {
                hudEntity.Detach<DisabledComponent>();
            }

            count++;
        }
    }
    void CreateHUDLife()
    {
        if(_lifes.Count >= _playerManager.Health)
            return;

        int totalToCreate = _playerManager.Health - _lifes.Count;

        for (int i = 0; i < totalToCreate; i++)
        {
            Entity entity = CreateEntity();
            entity.Attach(_hudComponent);
            entity.Attach(_lifeSpriteComponent);
            entity.Attach(new Transform2());
            _lifes.Add(entity.Id, true);
        }
    }
}
