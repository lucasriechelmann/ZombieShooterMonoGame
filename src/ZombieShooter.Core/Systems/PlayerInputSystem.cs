using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems;

public class PlayerInputSystem : EntityProcessingSystem
{
    ComponentMapper<MovementComponent> _movementMapper;
    ComponentMapper<Transform2> _transformMapper;
    IGame _game;
    PlayerManager _playerManager;
    public PlayerInputSystem(IGame game, PlayerManager playerManager) : base(Aspect.All(typeof(PlayerComponent), typeof(MovementComponent), typeof(Transform2)))
    {
        _game = game;
        _playerManager = playerManager;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _movementMapper = mapperService.GetMapper<MovementComponent>();
        _transformMapper = mapperService.GetMapper<Transform2>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        _playerManager.ProcessImmunity(gameTime);
        KeyboardExtended.Update();
        MouseExtended.Update();

        KeyboardStateExtended currentKeyboardState = KeyboardExtended.GetState();
        MovementComponent movement = _movementMapper.Get(entityId);
        movement.MoveDirection = Vector2.Zero;

        if(currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            movement.SetMoveDirectionY(-1);

        if (currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            movement.SetMoveDirectionY(1);

        if (currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            movement.SetMoveDirectionX(-1);

        if (currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            movement.SetMoveDirectionX(1);

        movement.NormalizeMoveDirection();

        Transform2 transform = _transformMapper.Get(entityId);

        movement.Direction =  _game.Camera.ScreenToWorld(MouseExtended.GetState().Position.ToVector2()) - transform.Position;
    }
}
