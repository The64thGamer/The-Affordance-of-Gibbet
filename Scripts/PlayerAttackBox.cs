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
				enemy.Die();
			}
		}
    }
}
