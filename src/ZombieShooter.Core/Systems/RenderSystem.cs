using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core.Systems;

public class RenderSystem : EntityDrawSystem
{
    SpriteBatch _spriteBatch;
    ComponentMapper<Transform2> _transform2Mapper;
    ComponentMapper<SpriteComponent> _spriteComponentMapper;
    IGame _game;
    public RenderSystem(IGame game) : base(Aspect.All(typeof(SpriteComponent), typeof(Transform2)).Exclude(typeof(DisabledComponent)))
    {
        _spriteBatch = new(game.GraphicsDevice);
        _game = game;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _spriteComponentMapper = mapperService.GetMapper<SpriteComponent>();
        _transform2Mapper = mapperService.GetMapper<Transform2>();
    }
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(transformMatrix: _game.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

        foreach(int entityId in ActiveEntities)
        {
            Transform2 transform = _transform2Mapper.Get(entityId);
            SpriteComponent spriteComponent = _spriteComponentMapper.Get(entityId);
            _spriteBatch.Draw(spriteComponent.Sprite, transform);
        }

        _spriteBatch.End();
    }
}
