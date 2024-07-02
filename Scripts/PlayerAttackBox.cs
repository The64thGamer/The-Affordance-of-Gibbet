using Godot;
using System;

public partial class PlayerAttackBox : Node2D
{
	public float timer = 999;
	bool firstFrame = false;
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
			GetTree().Paused = false;
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
				GetTree().Paused = true;
				timer = Mathf.Max(timer,0.1f);
			}
		}
    }
}
