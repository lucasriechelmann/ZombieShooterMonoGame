using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace ZombieShooter.Core.Components;

public class CircleColliderComponent
{
    public CircleF Collider { get; set; }
    public CircleColliderComponent(float radius)
    {
        Collider = new(Vector2.Zero, radius);
    }
}
