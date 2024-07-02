using Godot;
using System;

public partial class Player : Entity
{
	float currentDamage = 0;
	float currentSpeed = 80;
	float JumpVelocity = -37;
	float gravity = 300;

	[Export] Curve JumpHoldCurve;
	[Export] float minCopyTime = 1.0f;

	[Export] CopyUI zapHitbox;
	[Export] float walkAnimSpeed;
	[Export] float copyAnimSpeed;
	[Export] float rollAnimSpeed;
	[Export] PlayerCam playerCam;

	[Export] float animTimer;
	[Export] int oldAnimTimer;
	[Export] float jumpTimer;
	[Export] float copyTimer;
	[Export] float physicsTimer;
	[Export] float flungTimer;
	[Export] float copyCooldownTimer;
	[Export] float attackCooldownTimer;
	[Export] bool isJumping;
	[Export] bool firstFrameOnGround;

	const float standardSpeed = 80;
	const float sodaSideAnimSpeed = 20;
	const float copyCooldown = 0.5f;
	const float attackCooldown = 0.3f;
	const int zapHitboxXPosition = 14;
	const int initialJumpMult = 3;
	const int slowdownSpeed = 2;
	const float minCopyTimeHitboxSpawn = 0.15f;
	const float damageToFlungTimeMultiplier = 0.03f;
	const float damageToFlungVelocityMultiplier = 5f;
	const float horizontalMoveDeadzone = 0.2f;
	const float attackMaxVerticalVelocity = 50;
	const float attackDashSpeed = 200;
	const float attackDashDeceleration = 3;
	const float velocityRedirectPenalty = 0.5f;
	Vector2 currentInput;
	Vector2 previousCloudPlacement;
	bool inInvincibilityFrames = false;

	[Export] PlayerState playerState = PlayerState.standard;
	CopyAbility[] copyAbility = new CopyAbility[4];

	FloorState floorState;
	
	public enum PlayerState
	{
		standard,
		copying,
		uncopying,
		flung,
		sideAttack,
		neutralAttack,
		upAttack,
		downAttack,
		turningAround,
	}
	public enum CopyAbility
	{
		none,
		stargazer,
		builder,
		drinker,
		cowboy,
		gambler,

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
		switch (playerState)
		{
			case PlayerState.standard:
				if(Input.IsActionPressed("Copy") && copyCooldownTimer <= 0)
				{
					ChangeState(PlayerState.copying);
				}
				if(Input.IsActionPressed("Attack") && attackCooldownTimer <= 0 && currentInput.X != 0 && copyAbility[1] != CopyAbility.none)
				{
					ChangeState(PlayerState.sideAttack);
				}
				attackCooldownTimer = Mathf.Max(0,attackCooldownTimer - (float)delta);
				copyCooldownTimer = Mathf.Max(0,copyCooldownTimer - (float)delta);
				Vector2 input = Input.GetVector("Left", "Right", "Up", "Down");
				if(Mathf.Abs(Mathf.Sign(input.X) + Mathf.Sign(Velocity.X)) != 2 && input.X != 0 && Velocity.X != 0 && IsOnFloor())
				{
					ChangeState(PlayerState.turningAround);
				}
				break;
			case PlayerState.copying:
				copyTimer += (float)delta;
				if(copyTimer >= minCopyTime)
				{
					copyTimer = 0;
					ChangeState(PlayerState.uncopying);
				}
				if(copyTimer > minCopyTimeHitboxSpawn)
				{
					zapHitbox.Position = new Vector2(zapHitboxXPosition * (sprite.FlipH ? -1 : 1),0);
					zapHitbox.Monitoring = true;
				}
			break;
			case PlayerState.uncopying:
				zapHitbox.SetDeferred("monitoring",false);
				copyCooldownTimer = copyCooldown;
			break;
			case PlayerState.flung:
				if(previousCloudPlacement.DistanceTo(GlobalPosition) > 16)
				{
					previousCloudPlacement = GlobalPosition;
					CreateGenericEffect(Effect.EffectType.cloud,Effect.EffectMovement.none,GlobalPosition,Effect.SpriteDirection.notFlipped);
				}
				flungTimer -= (float)delta;
				if(flungTimer <= 0)
				{
					ChangeState(PlayerState.standard);
					flungTimer = 0;
				}
				if(!isVisibletoCamera)
				{
					GetTree().ChangeSceneToFile("res://Scenes/Level1.tscn");
				}
				break;
			case PlayerState.turningAround:
				if(physicsTimer >= 0.2f || !IsOnFloor())
				{
					ChangeState(PlayerState.standard);
				}
				break;
			default:
			break;
		}
	}

	void CreateDashEffect()
	{
		if(!IsOnFloor())
		{
			return;
		}
		if(Velocity.X < 0)
		{
			CreateGenericEffect(Effect.EffectType.dash,Effect.EffectMovement.moveSlightlyRight,GlobalPosition,Effect.SpriteDirection.flipped);
		}
		else
		{
			CreateGenericEffect(Effect.EffectType.dash,Effect.EffectMovement.moveSlightlyLeft,GlobalPosition,Effect.SpriteDirection.notFlipped);
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
				currentSpeed = standardSpeed;
				Velocity = CalculateStandardVelocity(input,jumpInput,delta);
				break;
			case PlayerState.uncopying:
				currentSpeed = standardSpeed;
				Velocity = CalculateStandardVelocity(currentInput,false,delta);
				break;
			case PlayerState.flung:
				if (!IsOnFloor())
				{
					Velocity = new Vector2(Velocity.X, Velocity.Y + (gravity * (float)delta));
				}
				break;
			case PlayerState.sideAttack:
				switch (copyAbility[1])
				{
					case CopyAbility.drinker:
						input = Input.GetVector("Left", "Right", "Up", "Down");
						if((input.X < 0 && !sprite.FlipH) || (input.X > 0 && sprite.FlipH))
						{
							currentSpeed = standardSpeed * velocityRedirectPenalty;
						}
						else
						{
							currentSpeed = standardSpeed;
						}
						currentInput = input;
						Velocity = CalculateStandardVelocity(input,false,delta);
						if(IsOnFloor())
						{
							Velocity = new Vector2(
							Mathf.Lerp(attackDashSpeed * input.X,0,Mathf.Min(physicsTimer * attackDashDeceleration,1))
							,Velocity.Y
							);
						}
						else
						{
							Velocity = new Vector2(
							Velocity.X,
							Mathf.Lerp(Velocity.Y,Mathf.Clamp(Velocity.Y,-attackMaxVerticalVelocity,attackMaxVerticalVelocity),Mathf.Min(physicsTimer,1))
							);
						}
					break;
					default:
					ChangeState(PlayerState.standard);
					break;
				}
				break;
			case PlayerState.turningAround:
				if(physicsTimer == 0)
				{
					CreateDashEffect();
				}
				currentSpeed = standardSpeed;
				jumpInput = CheckJump();
				if(jumpInput)
				{
					ChangeState(PlayerState.standard);
				}
				currentInput = Vector2.Zero;
				Velocity = CalculateStandardVelocity(currentInput,jumpInput,delta);
				Velocity = new Vector2(
					Mathf.Lerp(Velocity.X,0,Mathf.Min(physicsTimer,1))
					,Velocity.Y
				);
				break;
			default:
				currentSpeed = standardSpeed;
				input = Input.GetVector("Left", "Right", "Up", "Down");
				jumpInput = CheckJump();
				currentInput = input;
				Velocity = CalculateStandardVelocity(input,jumpInput,delta);
				break;
		}
		physicsTimer += (float)delta;

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
				CreateGenericEffect(Effect.EffectType.landing,Effect.EffectMovement.none,GlobalPosition,Effect.SpriteDirection.notFlipped);
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
			CreateGenericEffect(Effect.EffectType.jumping,Effect.EffectMovement.none,GlobalPosition,Effect.SpriteDirection.notFlipped);
		}

		if (input.X > horizontalMoveDeadzone || input.X < -horizontalMoveDeadzone)
		{
			float newDelta = (float)delta * 7;
			velocity.X = velocity.X * (1 - newDelta) + (currentSpeed * Mathf.Sign(input.X)) * newDelta;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currentSpeed);
		}
		return velocity;
	}
	
	bool CheckJump()
	{
		if ((Input.IsActionJustPressed("Up")||Input.IsActionPressed("Up")) && IsOnFloor())
		{
			return true;
		}	
		return false;
	}

	void UpdateSprites(double delta)
	{


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
							ChangeState(PlayerState.standard);
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
			case PlayerState.sideAttack:
				switch (copyAbility[1])
				{
					case CopyAbility.drinker:
						if(oldAnimTimer != Mathf.FloorToInt(animTimer))
							{
								switch (Mathf.FloorToInt(animTimer))
								{
									case 0:
										inInvincibilityFrames = true;
										CreateDashEffect();
										sprite.SetSprite("Soda Side C");
									break;
									case 1:
										sprite.SetSprite("Soda Side C");
									break;
									case 2:
										sprite.SetSprite("Soda Side B");
									break;
									case 3:
										sprite.SetSprite("Soda Side A");
										SpawnHitbox(GlobalPosition + new Vector2((sprite.FlipH ? -1 : 1) * 8,0), new Vector2(32,16),1 / sodaSideAnimSpeed,true);
									break;
									case 4:
										inInvincibilityFrames = false;
										sprite.SetSprite("Soda Side A");
									break;
									case 5:
										sprite.SetSprite("Soda Side B");
									break;
									case 6:
										sprite.SetSprite("Soda Side A");
										SpawnHitbox(GlobalPosition + new Vector2((sprite.FlipH ? -1 : 1) * 8,0), new Vector2(32,16),1 / sodaSideAnimSpeed,true);
									break;
									case 7:
										sprite.SetSprite("Soda Side A");
									break;
									case 8:
										sprite.SetSprite("Soda Side B");
									break;
									case 9:
										sprite.SetSprite("Soda Side A");
										SpawnHitbox(GlobalPosition + new Vector2((sprite.FlipH ? -1 : 1) * 8,0), new Vector2(32,16),1 / sodaSideAnimSpeed,true);
									break;
									case 10:
										sprite.SetSprite("Soda Side A");
									break;
									case 11:
										sprite.SetSprite("Soda Side B");
									break;
									case 12:
										sprite.SetSprite("Soda Side C");
									break;
									default:
									ChangeState(PlayerState.standard);
									attackCooldownTimer = attackCooldown;
									return;
								}
							}
						oldAnimTimer = Mathf.FloorToInt(animTimer);
						animTimer += (float)delta * sodaSideAnimSpeed;
						break;
					default:
					break;
				}
				return;
				case PlayerState.turningAround:
					sprite.SetSprite("Turn Around");
					return;
			default:
				if(Velocity.X != 0)
				{
					sprite.FlipH = Velocity.X < 0 ? true : false;
				}
				break;
		}


		
		if(IsOnFloor())
		{			
			if(Velocity == Vector2.Zero)
			{
				if(currentInput.Y > 0.1f)
				{
					if(animTimer != -1)
					{
						sprite.SetSprite("Crouch");
						Input.StartJoyVibration(0,0.7f,0,0.15f);
						animTimer = -1;
					}
					
				}
				else
				{
					sprite.SetSprite("Idle");
					animTimer = 0;
				}
			}
			else if(Velocity.X != 0)
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

	public void SpawnHitbox(Vector2 position, Vector2 size, float time, bool stuckToPlayer)
	{
		PlayerAttackBox attackBox = GD.Load<PackedScene>("res://Prefabs/Triggers/Hurtbox From Player.tscn").Instantiate() as PlayerAttackBox;
		((attackBox.GetChild(0) as CollisionShape2D).Shape as RectangleShape2D).Size = size;
		attackBox.timer = time;
		if(stuckToPlayer)
		{
			AddChild(attackBox);
		}
		else
		{
			GetParent().AddChild(attackBox);
		}

		attackBox.GlobalPosition = position;

	}

	public void ChangeState(PlayerState state)
	{
		physicsTimer = 0;
		animTimer = 0;
		playerState = state;
		inInvincibilityFrames = false;
	}

	public void SetCopyAbility(CopyAbility ability, int slot)
	{
		zapHitbox.SetDeferred("monitoring",false);
		Input.StartJoyVibration(0,0.5f,1,0.3f);
		if(ability != CopyAbility.none)
		{
			GD.Print("Player got " + ability + " ability in slot " + slot + "!");
			copyAbility[slot] = ability;
			switch (slot)
			{
				case 0:
					ChangeState(PlayerState.neutralAttack);
					break;
				case 1:
					ChangeState(PlayerState.sideAttack);
					break;
				case 2:
					ChangeState(PlayerState.upAttack);
					break;
				case 3:
					ChangeState(PlayerState.downAttack);
					break;
				default:
					ChangeState(PlayerState.uncopying);
				break;
			}
		}
		else
		{
			ChangeState(PlayerState.uncopying);
		}	
	}

	public PlayerState GetPlayerState()
	{
		return playerState;
	}

	public void ApplyDamage(float attackDamage, bool launch, Vector2 globalHitPos)
	{
		if(inInvincibilityFrames)
		{
			return;
		}
		zapHitbox.Monitoring = false;
		currentDamage += attackDamage;
		if(launch)
		{
			ChangeState(PlayerState.flung);
			previousCloudPlacement = GlobalPosition;
			flungTimer = currentDamage * damageToFlungTimeMultiplier;
			Velocity = (GlobalPosition - globalHitPos).Normalized() * currentDamage * damageToFlungVelocityMultiplier;
			Input.StartJoyVibration(0,1,1,Mathf.Clamp(currentDamage/75.0f,0.2f,1.0f));
			playerCam.Flung(flungTimer);
		}
		
	}
}
