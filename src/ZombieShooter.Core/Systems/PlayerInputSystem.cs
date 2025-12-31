using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using ZombieShooter.Core.Components;

namespace ZombieShooter.Core.Systems;

public class PlayerInputSystem : EntityProcessingSystem
{
    ComponentMapper<MovementComponent> _movementMapper;
    public PlayerInputSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(MovementComponent)))
    {
        
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
        movement.Direction = MouseExtended.GetState().Position.ToVector2();
    }
}
