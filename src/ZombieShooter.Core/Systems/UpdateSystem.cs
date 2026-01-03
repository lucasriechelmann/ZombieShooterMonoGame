using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieShooter.Core.Managers;

namespace ZombieShooter.Core.Systems
{
    public class UpdateSystem : EntityUpdateSystem
    {
        PlayerManager _playerManager;
        EnemyManager _enemyManager;
        BulletManager _bulletManager;
        public UpdateSystem(PlayerManager playerManager, EnemyManager enemyManager, BulletManager bulletManager) : base(Aspect.All())
        {
            _playerManager = playerManager;
            _enemyManager = enemyManager;
            _bulletManager = bulletManager;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardExtended.Update();
            MouseExtended.Update();

            _playerManager.ProcessImmunity(gameTime);
            _enemyManager.ProcessSpawning(gameTime);
            _bulletManager.ProcessShooting(gameTime);
        }
    }
}
