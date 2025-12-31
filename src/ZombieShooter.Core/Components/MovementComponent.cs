using Microsoft.Xna.Framework;

namespace ZombieShooter.Core.Components;
public class MovementComponent
{
    public MovementComponent(float speed = 100)
    {
        _moveDirection = Vector2.Zero;
        Direction = Vector2.Zero;
        Speed = speed;
    }
    Vector2 _moveDirection;
    public Vector2 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    public Vector2 Direction { get; set; }
    public float Speed { get; set; } 
    public void SetMoveDirectionY(float y) => _moveDirection.Y = y;
    public void SetMoveDirectionX(float x) => _moveDirection.X = x;
    public void NormalizeMoveDirection()
    {
        if (_moveDirection != Vector2.Zero)
            _moveDirection.Normalize();
    }
}
