using Godot;
using System;

public partial class Player : Entity
{
	float Speed = 80;
	float JumpVelocity = -37;
	float gravity = 300;

	[Export] Curve JumpHoldCurve;
	[Export] float minCopyTime = 1.0f;

	[Export] CopyUI zapHitbox;
	[Export] float walkAnimSpeed;
	[Export] float copyAnimSpeed;

	float animTimer;
	float jumpTimer;
	float copyTimer;
	float copyCooldownTimer;
	bool isJumping;

	const float copyCooldown = 0.25f;
	const int initialJumpMult = 3;
	const int slowdownSpeed = 2;
	const float minCopyTimeHitboxSpawn = 0.1f;
	Vector2 currentInput;

	PlayerState playerState = PlayerState.standard;
	CopyAbility currentCopyAbility;
	
	public enum PlayerState
	{
		standard,
		copying,
		takingAbility,
		uncopying,
	}
	public enum CopyAbility
	{
		none,
	}

	public override void _PhysicsProcess(double delta)
	{
		StateCheck(delta);
		SetPhysics(delta);
		MoveAndSlide();
		UpdateSprites(delta);
	}

	void StateCheck(double delta)
	{
		switch (playerState)
		{
			case PlayerState.standard:
				if(Input.IsActionPressed("Copy") && copyCooldownTimer <= 0)
				{
					playerState = PlayerState.copying;
					animTimer = 0;
					copyTimer = 0;
				}
				copyCooldownTimer = Mathf.Max(0,copyCooldownTimer - (float)delta);
				break;
			case PlayerState.copying:
				copyTimer += (float)delta;
				if(copyTimer >= minCopyTime)
				{
					playerState = PlayerState.uncopying;
					animTimer = 0;
					copyTimer = 0;
				}
				if(copyTimer > minCopyTimeHitboxSpawn)
				{
					zapHitbox.Position = new Vector2(16 * (sprite.FlipH ? -1 : 1),0);
					zapHitbox.Monitoring = true;
				}
			break;
			case PlayerState.uncopying:
				zapHitbox.Monitoring = false;
				copyCooldownTimer = copyCooldown;
			break;
			case PlayerState.takingAbility:
				zapHitbox.Monitoring = false;
				if(currentCopyAbility == CopyAbility.none)
				{
					playerState = PlayerState.standard;
				}
				copyCooldownTimer = copyCooldown;
			break;
			default:
			break;
		}
	}

	void SetPhysics(double delta)
	{
		bool jumpInput = false;
		Vector2 input = Vector2.Zero;
		switch (playerState)
		{
			case PlayerState.copying:
				if(!IsOnFloor())
				{
					input = currentInput.Lerp(Vector2.Zero,Mathf.Min(1,copyTimer * slowdownSpeed));
					if(Input.GetVector("Left", "Right", "Up", "Down").Dot(currentInput) <= -0.5)
					{
						currentInput = Vector2.Zero;
						input = currentInput;
					}
				}
				else
				{
					input = Vector2.Zero;
				}
				break;
			case PlayerState.uncopying:
				break;
			default:
				input = Input.GetVector("Left", "Right", "Up", "Down");
				jumpInput = CheckJump();
				currentInput = input;
				break;
		}

		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		if(isJumping)
		{
			if(!Input.IsActionPressed("Up"))
			{
				isJumping = false;
			}
			else
			{
				jumpTimer = Mathf.Min(1,jumpTimer + (float)delta);
				velocity.Y += JumpVelocity * JumpHoldCurve.SampleBaked(jumpTimer);
				if(jumpTimer >= 1)
				{
					isJumping = false;
					jumpTimer = 0;
				}
			}
		}

		if(jumpInput)
		{
			isJumping = true;
			velocity.Y = JumpVelocity * initialJumpMult;
			jumpTimer = 0;
		}

		if (input != Vector2.Zero)
		{
			velocity.X = input.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
	}
	
	bool CheckJump()
	{
		if (Input.IsActionJustPressed("Up") && IsOnFloor())
		{
			return true;
		}	
		return false;
	}

	void UpdateSprites(double delta)
	{
		if(Velocity.X != 0)
		{
			sprite.FlipH = Velocity.X < 0 ? true : false;
		}

		switch (playerState)
		{
			case PlayerState.copying:
				switch (Mathf.FloorToInt(animTimer))
				{
					case 0:
						sprite.SetSprite("Copy A");
					break;
					case 1:
						sprite.SetSprite("Copy A");
					break;
					case 2:
						sprite.SetSprite("Copy B");
					break;
					case 3:
						sprite.SetSprite("Copy C");
					break;
					default:
					animTimer = 2;
					break;
				}
				animTimer += (float)delta * copyAnimSpeed;
				return;
			case PlayerState.uncopying:
				switch (Mathf.FloorToInt(animTimer))
				{
					case 0:
						sprite.SetSprite("Copy A");
					break;
					case 1:
						sprite.SetSprite("Copy A");
					break;
					default:
						playerState = PlayerState.standard;
						break;
				}
				animTimer += (float)delta * copyAnimSpeed;
				return;
			default:
			break;
		}


		
		if(Velocity.Y == 0)
		{
			if(Velocity == Vector2.Zero)
			{
				sprite.SetSprite("Idle");
				animTimer = 0;
			}
			if(Velocity.X != 0)
			{
				switch (Mathf.FloorToInt(animTimer))
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
					animTimer = 0;
					break;
				}
				animTimer += (float)delta * walkAnimSpeed;
			}
		}
		else
		{
			sprite.SetSprite("Jump");
		}
	}

	public void SetCopyAbility(CopyAbility ability, int slot)
	{
		currentCopyAbility = ability;
		playerState = PlayerState.takingAbility;
	}
}
