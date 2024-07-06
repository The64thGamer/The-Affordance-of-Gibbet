using Godot;
using System;

public partial class GameState : Node
{
	[Export] Node2D level;
	[Export] Player Player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LevelChange(0,Vector2.Zero);
	}

	public void LevelChange(int id, Vector2 position)
	{
		foreach(Node2D child in level.GetChildren())
		{
			if(child != Player)
			{
				level.RemoveChild(child);
			}
		}
		Node2D area = GD.Load<PackedScene>("res://Prefabs/Levels/area_" + id + ".tscn").Instantiate() as Node2D;
		area.GlobalPosition = Vector2.Zero;
		level.AddChild(area);
		level.RemoveChild(Player);
		level.AddChild(Player);
		Player.GlobalPosition = position;
	}
}
