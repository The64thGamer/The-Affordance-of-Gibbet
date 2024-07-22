using Godot;
using System;

public partial class PlayerAttackBox : Node2D
{
	public float timer = 999;
	public float delayTimer = 0;
	public float attackHitStun = 0;
	float removePauseTimer = 0;
	bool firstFrame = false;
	bool alreadyPaused = false;
	public override void _Process(double delta)
	{
		timer -= (float)delta;
		delayTimer -= (float)delta;
		removePauseTimer -= (float)delta;
		if(!firstFrame)
		{	
			firstFrame = true;
			return;
		}
		if(timer <= 0)
		{
			if(GetTree().Paused)
			{
				(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(GetInstanceId());
			}
			QueueFree();
		}
		if(removePauseTimer <= 0)
		{
			if(GetTree().Paused)
			{
				(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(GetInstanceId());
			}
		}
	}
	void _on_body_entered(PhysicsBody2D body)
    {			
		if(delayTimer > 0)
		{
			return;
		}
		
		if(body is GenericEnemy)
		{
			GenericEnemy enemy = body as GenericEnemy;
			if(!enemy.IsDead())
			{
				Input.StartJoyVibration(0,0,0.85f*PlayerPrefs.GetFloat("RumbleIntensity"),0.2f*PlayerPrefs.GetFloat("RumbleTime"));
				enemy.Die();
				if(!alreadyPaused)
				{
					alreadyPaused = true;
					(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).AddPause(GetInstanceId());
				}
				removePauseTimer = attackHitStun;
			}
		}
    }
}
