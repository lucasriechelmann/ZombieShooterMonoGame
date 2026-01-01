using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core.Systems;

public class RenderDebugSystem : EntityDrawSystem
{
    ComponentMapper<CircleColliderComponent> _circleColliderMapper;
    ComponentMapper<Transform2> _transform2Mapper;
    SpriteBatch _spriteBatch;
    IGame _game;
    public RenderDebugSystem(IGame game) : base(Aspect.All(typeof(CircleColliderComponent), typeof(Transform2)))
    {
        _spriteBatch = new(game.GraphicsDevice);
        _game = game;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _circleColliderMapper = mapperService.GetMapper<CircleColliderComponent>();
        _transform2Mapper = mapperService.GetMapper<Transform2>();
    }
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(transformMatrix: _game.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

        foreach (int entityId in ActiveEntities)
        {
            Transform2 transform = _transform2Mapper.Get(entityId);
            CircleColliderComponent circleCollider = _circleColliderMapper.Get(entityId);
            _spriteBatch.DrawCircle(transform.Position + circleCollider.Collider.Position, circleCollider.Collider.Radius, 32, Color.Red, 1);
        }

        _spriteBatch.End();
    }
}
