using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace ZombieShooter.Core.Managers;

public class PlayerManager
{
    public Action OnDeath;
    float _immunityTime = 1.0f;
    float _immunityTimer = 0.0f;
    int _maxHealth = 10;
    public int Health { get; private set; }
    Transform2 _playerTransform;
    public PlayerManager()
    {
        Health = _maxHealth;
    }
    public void SetPlayerTransform(Transform2 transform)
    {
        _playerTransform = transform;
    }
    public Vector2 Position => _playerTransform?.Position ?? Vector2.Zero;
    public void ProcessImmunity(GameTime gameTime)
    {
        if(_immunityTimer < 0f)
        {
            _immunityTimer = 0f;
            return;
        }

        _immunityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void Hit(int damage)
    {
        if(_immunityTimer > 0f)
            return;
        
        _immunityTimer = _immunityTime;

        Health -= damage;
        if (Health > 0)
            return;

        OnDeath?.Invoke();
    }

}
