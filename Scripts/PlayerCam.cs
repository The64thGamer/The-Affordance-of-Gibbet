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
	public TileMap tileMap;

	Vector2 deadzonePoint;
	Vector2 targetPoint;
	Vector2 flungPos;
	bool oldDeadZone = false;
	float camRampupTimer = 0;
	float flungTimer;
	Vector2 oldCamPos;
	bool camReset;

	public override void _Ready()
	{
		oldCamPos = GlobalPosition;
		deadzonePoint = player.GlobalPosition;
		targetPoint = deadzonePoint;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = oldCamPos;
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

		flungTimer = Mathf.Max(0,flungTimer - (float)delta);

		if(flungTimer > 0)
		{
			GlobalPosition = GlobalPosition.Lerp(flungPos,Mathf.Min(flungTimer,1));
		}

		oldCamPos = GlobalPosition;
		GlobalPosition = new Vector2(Mathf.FloorToInt(GlobalPosition.X),Mathf.FloorToInt(GlobalPosition.Y));

		Player.PlayerState state = player.GetPlayerState();
		if(state == Player.PlayerState.enteringDoor)
		{
			camReset = true;
		}
		else if(camReset)
		{
			camReset = false;
			GlobalPosition = player.GlobalPosition;
			oldCamPos = GlobalPosition;
		}

		if(tileMap != null)
		{
			Rect2I rect = tileMap.GetUsedRect();
			float size = tileMap.TileSet.TileSize.X;
			GlobalPosition = new Vector2(
					Mathf.Clamp(
						GlobalPosition.X,
						(rect.Position.X * size) + (160 * 0.5f),
						(rect.Position.X * size) + (rect.Size.X * size) - (160 * 0.5f)
						),
					Mathf.Clamp(
						GlobalPosition.Y,
						(rect.Position.Y * size) + (144 * 0.5f),
						(rect.Position.Y * size) + (rect.Size.Y * size) - (144 * 0.5f)
						)
			);
		}
		
	}

	public void Flung(float time)
	{
		flungTimer = time;
		flungPos = GlobalPosition;
	}
}
