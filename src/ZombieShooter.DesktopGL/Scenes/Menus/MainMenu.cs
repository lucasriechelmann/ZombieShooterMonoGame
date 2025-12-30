using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.ECS;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using System;
using System.IO;
using ZombieShooter.Core.Scene;

namespace ZombieShooter.DesktopGL.Scenes.Menus;

public class MainMenu : SceneUIBase
{
    StackPanel _stackPanel;
    public MainMenu(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void OnInitialize()
    {
        _stackPanel = new();
        _stackPanel.Spacing = 15;
        _stackPanel.WidthUnits = DimensionUnitType.PercentageOfParent;
        _stackPanel.Width = 30;
        _stackPanel.Anchor(Anchor.Center);
        _stackPanel.AddToRoot();
        
        _stackPanel.AddChild(CreateLabel("Zombie Shooter"));
        Button startBtn = CreateButton("Start");
        startBtn.Click += StartButton_Click;
        _stackPanel.AddChild(startBtn);
        Button exitBtn = CreateButton("Exit");
        exitBtn.Click += ExitButton_Click;
        _stackPanel.AddChild(exitBtn);
    }

    private void StartButton_Click(object sender, EventArgs e)
    {

        Console.WriteLine(nameof(StartButton_Click));
    }
    void ExitButton_Click(object sender, EventArgs e) => _game.Exit();
    Label CreateLabel(string text)
    {
        Label label = new();
        //label.Visual = new VIsual
        label.Text = "Zombie Shooter";
        label.Height = 50;
        label.WidthUnits = DimensionUnitType.PercentageOfParent;
        label.Width = 100;
        label.Dock(Dock.FillHorizontally);

        if (label.Visual is TextRuntime visual)
        {
            visual.UseCustomFont = true;
            visual.CustomFontFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content/fonts/arial/Arial.fnt");
            visual.IsBold = true;
            visual.FontScale = 0.5f;
        }

        return label;
    }
    Button CreateButton(string text)
    {
        Button button = new();
        button.WidthUnits = DimensionUnitType.PercentageOfParent;
        button.Width = 100;
        button.Text = text;
        button.Height = 50;

        return button;
    }
    protected override void OnUnloadContent()
    {
        _stackPanel.RemoveFromRoot();
        _stackPanel = null;
    }

}
