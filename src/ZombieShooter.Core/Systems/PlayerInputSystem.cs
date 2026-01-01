using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using ZombieShooter.Core.Components;
using ZombieShooter.Core.Contracts;

namespace ZombieShooter.Core.Systems;

public class PlayerInputSystem : EntityProcessingSystem
{
    ComponentMapper<MovementComponent> _movementMapper;
    IGame _game;
    public PlayerInputSystem(IGame game) : base(Aspect.All(typeof(PlayerComponent), typeof(MovementComponent)))
    {
        _game = game;
    }
    public override void Initialize(IComponentMapperService mapperService)
    {
        _movementMapper = mapperService.GetMapper<MovementComponent>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
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
        movement.Direction = _game.Camera.ScreenToWorld(MouseExtended.GetState().Position.ToVector2());
    }
}
