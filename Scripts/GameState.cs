using Godot;
using System;

public partial class GameState : Node
{
	[Export] Node2D level;
	[Export] Player Player;
	[Export] int defaultLevel = 3;
	int currentid;
	Vector2 currentPos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LevelChange(defaultLevel,Vector2.Zero);
	}

	public int GetCurrentArea()
	{
		return currentid;
	}

	public Vector2 GetLastDoorPosition()
	{
		return currentPos;
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

		int childCount = area.GetChildCount();

        for (int i = 0; i < childCount; i++)
        {
            Node child = area.GetChild(i);
            if (child is TileMap)
                Player.ChangeLevel(child as TileMap);
        }
		currentid = id;
		currentPos = position;
	}

}
