using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieShooter.Core.Components;

public class SpriteComponent
{
    public Sprite Sprite { get; }
    public SpriteComponent(Sprite sprite, float depth = 0f)
    {
        Sprite = sprite;
        Sprite.Depth = depth;
        Sprite.Origin = new(Sprite.TextureRegion.Width / 2, Sprite.TextureRegion.Height / 2);
    }
}
