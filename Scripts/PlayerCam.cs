using Godot;
using System;

public partial class PlayerCam : Camera2D
{
	[Export] Player player;
	[Export] float cameraLookaheadMultiplier = 1;
	[Export] float cameraSpeed = 0.23f;
	[Export] float maxDeadzoneX = 10;

	Vector2 deadzonePoint;
	Vector2 targetPoint;
	

	public override void _Ready()
	{
		deadzonePoint = player.GlobalPosition;
		targetPoint = deadzonePoint;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(player.Velocity.X == 0)
		{
			targetPoint.X = GlobalPosition.X;
		}

		Vector2 distance = new Vector2(player.GlobalPosition.X - deadzonePoint.X,player.GlobalPosition.Y - deadzonePoint.Y);

		if(distance.X > maxDeadzoneX)
		{		
			deadzonePoint.X = player.GlobalPosition.X - maxDeadzoneX+1;
			targetPoint.X = player.GlobalPosition.X + (maxDeadzoneX * cameraLookaheadMultiplier);
		}
		if(distance.X < -maxDeadzoneX)
		{		
			deadzonePoint.X = player.GlobalPosition.X + maxDeadzoneX-1;
			targetPoint.X = player.GlobalPosition.X + (-maxDeadzoneX * cameraLookaheadMultiplier);
		}

		GlobalPosition = GlobalPosition.Lerp(targetPoint,Mathf.Pow(Mathf.Min(1,Mathf.Abs(targetPoint.X - GlobalPosition.X) * cameraSpeed* (float)delta),2));
	}
}
