using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieShooter.Core.Managers;

public class PlayerManager
{
    Transform2 _playerTransform;
    public void SetPlayerTransform(Transform2 transform)
    {
        _playerTransform = transform;
    }
    public Vector2 Position => _playerTransform?.Position ?? Vector2.Zero;
}
