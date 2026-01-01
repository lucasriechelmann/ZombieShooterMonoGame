namespace ZombieShooter.Core.Components;

public class BoxColliderComponent
{
    public float Width { get; set; }
    public float Height { get; set; }
    public BoxColliderComponent(float width, float height)
    {
        Width = width;
        Height = height;
    }
}
