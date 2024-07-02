using Godot;
using System;

public partial class PlayerAttackBox : Node2D
{
	public float timer = 999;
	bool firstFrame = false;
	bool alreadyPaused = false;
	public override void _Process(double delta)
	{
		timer -= (float)delta;
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
	}
	void _on_body_entered(PhysicsBody2D body)
    {			
		if(body is GenericEnemy)
		{
			GenericEnemy enemy = body as GenericEnemy;
			if(!enemy.IsDead())
			{
				Input.StartJoyVibration(0,0,0.85f,0.2f);
				enemy.Die();
				if(!alreadyPaused)
				{
					alreadyPaused = true;
					(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).AddPause(GetInstanceId());
				}
				
				timer = Mathf.Max(timer,0.1f);
			}
		}
    }
}
