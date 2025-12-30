using MonoGame.Extended.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieShooter.Core;

namespace ZombieShooter.DesktopGL.Scenes.Menus;

public class MainMenu : SceneBase
{
    public MainMenu(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override World CreateWorld()
    {
        return new WorldBuilder()
            .Build();
    }

    protected override void OnInitialize()
    {
        
    }

    protected override void OnLoadContent()
    {
        
    }

    protected override void OnUnloadContent()
    {
        
    }
}
