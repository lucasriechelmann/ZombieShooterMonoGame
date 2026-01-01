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
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core.Systems
{
    public class CameraFollowSystem : EntityProcessingSystem
    {
        ComponentMapper<Transform2> _transformMapper;
        IGame _game;
        public CameraFollowSystem(IGame game) : base(Aspect.All(typeof(PlayerComponent), typeof(Transform2)))
        {
            _game = game;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            float lerpSpeed = 0.1f;
            _game.Camera.LookAt(Vector2.Lerp(_game.Camera.Center, transform.Position, lerpSpeed));

            ApplyClamping();
        }
        void ApplyClamping()
        {
            float visibleWidth = _game.ScreenWidth / _game.Camera.Zoom;
            float visibleHeight = _game.ScreenHeight / _game.Camera.Zoom;

            float mapWidth = _game.ScreenWidth;
            float mapHeight = _game.ScreenHeight;

            float minX = 0;
            float minY = 0;
            float maxX = mapWidth - visibleWidth;
            float maxY = mapHeight - visibleHeight;

            _game.Camera.Position = new Vector2(
                MathHelper.Clamp(_game.Camera.Position.X, minX, maxX),
                MathHelper.Clamp(_game.Camera.Position.Y, minY, maxY)
            );
        }
    }
}
