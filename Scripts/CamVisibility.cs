using Godot;
using System;

public partial class CamVisibility : Area2D
{
	void _on_body_entered(PhysicsBody2D body)
    {
		if(body is Entity)
		{
			(body as Entity).SetCamVisibility(true);
		}
    }

	void _on_body_exit(PhysicsBody2D body)
    {
		if(body is Entity)
		{
			(body as Entity).SetCamVisibility(false);
		}
    }
}
