using Godot;
using System;

public partial class AttackTrigger : Area2D
{
	[Export] float attackDamage = 15f;
	[Export] bool attackLaunches = true;

	void _on_body_entered(PhysicsBody2D body)
    {
		if(body is Player)
		{
			(body as Player).ApplyDamage(attackDamage,attackLaunches,GlobalPosition);
		}
    }
}
