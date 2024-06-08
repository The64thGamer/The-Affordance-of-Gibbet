using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	[Export] SpriteAnim sprite;
	[Export] float Speed = 300.0f;
	[Export] float JumpVelocity = -400.0f;

	[Export] float gravity = 20f;
	
	[Export] float walkAnimSpeed;
	float walkTimer;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		UpdateSprites(delta);
	}

	void UpdateSprites(double delta)
	{
		if(Velocity.X != 0)
		{
			sprite.FlipH = Velocity.X < 0 ? true : false;
		}
		
		if(Velocity.Y == 0)
		{
			if(Velocity == Vector2.Zero)
			{
				sprite.SetSprite("Idle");
				walkTimer = 0;
			}
			if(Velocity.X != 0)
			{
				switch (Mathf.FloorToInt(walkTimer))
				{
					case 0:
						sprite.SetSprite("Walk C");
					break;
					case 1:
						sprite.SetSprite("Walk B");
					break;
					case 2:
						sprite.SetSprite("Walk A");
					break;
					case 3:
						sprite.SetSprite("Walk B");
					break;
					default:
					walkTimer = 0;
					break;
				}
				walkTimer += (float)delta * walkAnimSpeed;
			}
		}
		else
		{
			sprite.SetSprite("Jump");
		}
	}
}
