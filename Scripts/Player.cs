using Godot;
using System;

public partial class Player : Entity
{
	float currentDamage = 0;
	float Speed = 80;
	float JumpVelocity = -37;
	float gravity = 300;

	[Export] Curve JumpHoldCurve;
	[Export] float minCopyTime = 1.0f;

	[Export] CopyUI zapHitbox;
	[Export] float walkAnimSpeed;
	[Export] float copyAnimSpeed;
	[Export] float rollAnimSpeed;
	[Export] PlayerCam playerCam;

	float animTimer;
	int oldAnimTimer;
	float jumpTimer;
	float copyTimer;
	float flungTimer;
	float copyCooldownTimer;
	bool isJumping;
	bool firstFrameOnGround;

	const float copyCooldown = 0.5f;
	const int zapHitboxXPosition = 14;
	const int initialJumpMult = 3;
	const int slowdownSpeed = 2;
	const float minCopyTimeHitboxSpawn = 0.15f;
	const float damageToFlungTimeMultiplier = 0.03f;
	const float damageToFlungVelocityMultiplier = 5f;
	const float horizontalMoveDeadzone = 0.2f;
	Vector2 currentInput;
	Vector2 previousCloudPlacement;

	PlayerState playerState = PlayerState.standard;
	CopyAbility currentCopyAbility;
	FloorState floorState;
	
	public enum PlayerState
	{
		standard,
		copying,
		takingAbility,
		uncopying,
		flung,
	}
	public enum CopyAbility
	{
		none,
		stargazer,
		builder,
		drinker,
		cowboy,

	}

	public enum FloorState
	{
		onFloor,
		firstFrameOffFloor,
		offFloor,
		firstFrameOnFloor,
	}

	public override void _Ready()
	{
		base._Ready();
		isVisibletoCamera = true;
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
		if(!isVisibletoCamera)
		{
			GetTree().ChangeSceneToFile("res://Scenes/Level1.tscn");
		}

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
					zapHitbox.Position = new Vector2(zapHitboxXPosition * (sprite.FlipH ? -1 : 1),0);
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
			case PlayerState.flung:
				if(previousCloudPlacement.DistanceTo(GlobalPosition) > 16)
				{
					previousCloudPlacement = GlobalPosition;
					CreateGenericEffect(Effect.EffectType.cloud,Effect.EffectMovement.none,GlobalPosition);
				}
				flungTimer -= (float)delta;
				if(flungTimer <= 0)
				{
					playerState = PlayerState.standard;
					flungTimer = 0;
				}
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
				Velocity = CalculateStandardVelocity(input,jumpInput,delta);
				break;
			case PlayerState.uncopying:
				Velocity = CalculateStandardVelocity(currentInput,false,delta);
				break;
			case PlayerState.flung:
				if (!IsOnFloor())
				{
					Velocity = new Vector2(Velocity.X, Velocity.Y + (gravity * (float)delta));
				}
				break;
			default:
				input = Input.GetVector("Left", "Right", "Up", "Down");
				jumpInput = CheckJump();
				currentInput = input;
				Velocity = CalculateStandardVelocity(input,jumpInput,delta);
				break;
		}
	}

	Vector2 CalculateStandardVelocity(Vector2 input, bool jumpInput, double delta)
	{
		switch (floorState)
		{
			case FloorState.firstFrameOnFloor:
			floorState = FloorState.onFloor;
			Input.StartJoyVibration(0,0.3f,0,0.15f);
			break;
			case FloorState.firstFrameOffFloor:
			floorState = FloorState.offFloor;
			Input.StartJoyVibration(0,0.01f,0,0.1f);
			break;
			case FloorState.offFloor:
			if(IsOnFloor())
			{
				floorState = FloorState.firstFrameOnFloor;
			}
			break;
			case FloorState.onFloor:
			if(!IsOnFloor())
			{
				floorState = FloorState.firstFrameOffFloor;
			}
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

		if (input.X > horizontalMoveDeadzone || input.X < -horizontalMoveDeadzone)
		{
			velocity.X = input.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		return velocity;
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
				if(oldAnimTimer != Mathf.FloorToInt(animTimer))
				{
					switch (Mathf.FloorToInt(animTimer))
					{
						case 0:
							sprite.SetSprite("Copy A");
							Input.StartJoyVibration(0,0.1f,0,minCopyTime);
						break;
						case 1:
							sprite.SetSprite("Copy A");
							Input.StartJoyVibration(0,0.1f,1,0.1f);
						break;
						case 2:
							sprite.SetSprite("Copy B");
						break;
						case 3:
							sprite.SetSprite("Copy C");
						break;
						case 4:
							sprite.SetSprite("Copy B");
						break;
						case 5:
							sprite.SetSprite("Copy C");
							Input.StartJoyVibration(0,0.1f,1,0.1f);
						break;
						default:
						oldAnimTimer = Mathf.FloorToInt(animTimer);
						animTimer = 2;
						return;
					}
				}
				oldAnimTimer = Mathf.FloorToInt(animTimer);
				animTimer += (float)delta * copyAnimSpeed;
				return;
			case PlayerState.uncopying:
				if(oldAnimTimer != Mathf.FloorToInt(animTimer))
				{
					switch (Mathf.FloorToInt(animTimer))
					{
						case 0:
							sprite.SetSprite("Copy A");
							Input.StopJoyVibration(0);
						break;
						case 1:
							sprite.SetSprite("Copy A");
						break;
						default:
							playerState = PlayerState.standard;
							break;
					}
				}
				oldAnimTimer = Mathf.FloorToInt(animTimer);
				animTimer += (float)delta * copyAnimSpeed;
				return;
			case PlayerState.flung:
				if(oldAnimTimer != Mathf.FloorToInt(animTimer))
				{
					switch (Mathf.FloorToInt(animTimer))
					{
						case 0:
							sprite.SetSprite("Roll A");
						break;
						case 1:
							sprite.SetSprite("Roll A");
						break;
						case 2:
							sprite.SetSprite("Roll B");
						break;
						case 3:
							sprite.SetSprite("Roll C");
						break;
						default:
						oldAnimTimer = Mathf.FloorToInt(animTimer);
						animTimer = 2;
						return;
					}
				}
				oldAnimTimer = Mathf.FloorToInt(animTimer);
				animTimer += (float)delta * rollAnimSpeed;
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
					oldAnimTimer = Mathf.FloorToInt(animTimer);
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
		zapHitbox.Monitoring = false;
		Input.StartJoyVibration(0,0.5f,1,0.3f);
		if(currentCopyAbility != CopyAbility.none)
		{
			currentCopyAbility = ability;
		}	
		playerState = PlayerState.takingAbility;
	}

	public void ApplyDamage(float attackDamage, bool launch, Vector2 globalHitPos)
	{
		zapHitbox.Monitoring = false;
		if(launch)
		{
			playerState = PlayerState.flung;
			animTimer = 0;
			previousCloudPlacement = GlobalPosition;
		}
		currentDamage += attackDamage;
		flungTimer = currentDamage * damageToFlungTimeMultiplier;
		Velocity = (GlobalPosition - globalHitPos).Normalized() * currentDamage * damageToFlungVelocityMultiplier;
		playerCam.Flung(flungTimer);
		Input.StartJoyVibration(0,1,1,Mathf.Clamp(currentDamage/75.0f,0.2f,1.0f));
	}
}
