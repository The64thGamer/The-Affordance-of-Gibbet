using Godot;
using System;

public partial class PlayerCam : Camera2D
{
	[Export] Player player;
	[Export] float cameraLookaheadMultiplier = 1;
	[Export] float cameraSpeed = 0.23f;
	[Export] float maxDeadzoneX = 10;
	[Export] Curve camSpeedCurve;
	[Export] float minDeadzoneRepositionDistance = 16;

	Vector2 deadzonePoint;
	Vector2 targetPoint;
	bool oldDeadZone = false;
	float camRampupTimer = 0;

	public override void _Ready()
	{
		deadzonePoint = player.GlobalPosition;
		targetPoint = deadzonePoint;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		Vector2 distance = new Vector2(player.GlobalPosition.X - deadzonePoint.X,player.GlobalPosition.Y - deadzonePoint.Y);
		camRampupTimer += (float)delta;

		if(distance.X > maxDeadzoneX)
		{	
			if(!oldDeadZone){camRampupTimer = 0;}
			oldDeadZone = true;	
			deadzonePoint.X = player.GlobalPosition.X - maxDeadzoneX+1;
			targetPoint.X = player.GlobalPosition.X + (maxDeadzoneX * cameraLookaheadMultiplier);
		}
		else if(distance.X < -maxDeadzoneX)
		{	
			if(oldDeadZone){camRampupTimer = 0;}
			oldDeadZone = false;
			deadzonePoint.X = player.GlobalPosition.X + maxDeadzoneX-1;
			targetPoint.X = player.GlobalPosition.X + (-maxDeadzoneX * cameraLookaheadMultiplier);
		}

		GlobalPosition = GlobalPosition.Lerp(targetPoint,camSpeedCurve.SampleBaked(camRampupTimer) * Mathf.Min(1,Mathf.Abs(targetPoint.X - GlobalPosition.X) * cameraSpeed* (float)delta));
		if(Mathf.Abs(targetPoint.X - GlobalPosition.X) < minDeadzoneRepositionDistance)
		{
			deadzonePoint = deadzonePoint.Lerp(player.GlobalPosition,Mathf.Min(1,Mathf.Abs(player.GlobalPosition.X - deadzonePoint.X)) * (float)delta);
		}
	}
}
