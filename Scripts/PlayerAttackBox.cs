using Godot;
using System;

public partial class PlayerAttackBox : Node2D
{

	[ExportCategory("Timing")]
	[Export] public float timer = 999;
	[Export] public float delayTimer = 0;
	[Export] public bool stopIfInStandardState;


	[ExportCategory("Movement")]
	[Export] public Vector2 size = new Vector2(16,16);
	[Export] public Vector2 velocity;
	[Export] public float gravity;

	[ExportCategory("Damage")]
	[Export] public float attackHitStun = 0;

	[ExportCategory("Explode")]
	[Export] public bool explodeOnTimer = false;
	[Export] public float explodeForSec = 0.2f;
	[Export] public Vector2 explodeSize;

	[ExportCategory("Sprites")]
	[Export] SpriteAnim sprite;
	[Export] Godot.Collections.Array<string> animation;
	[Export] float animSpeed;
	float animTimer;
	float explodeTimer = -100;

	public Player player;
	float removePauseTimer = 0;
	bool firstFrame = false;
	bool alreadyPaused = false;					
		SoundManager soundManager;

	public override void _Ready()
	{
		soundManager = (GetNode("/root/SoundManager") as SoundManager);
		((GetChild(0) as CollisionShape2D).Shape as RectangleShape2D).Size = size;
	}

	public override void _Process(double delta)
	{
		if(explodeTimer != -100)
		{
			explodeTimer-= (float)delta;
		}
		velocity += new Vector2(0,gravity * (float)delta);
		timer -= (float)delta;
		delayTimer -= (float)delta;
		removePauseTimer -= (float)delta;
		if(!firstFrame)
		{	
			firstFrame = true;
			return;
		}



		if(explodeOnTimer && explodeTimer <= 0 && explodeTimer != -100) //maybe just make a bool for explodetimer
		{
			QueueFree();
		}
		if(timer <= 0 || (stopIfInStandardState && player.GetPlayerState() == Player.PlayerState.standard))
		{
			if(GetTree().Paused)
			{
				(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(GetInstanceId());
			}
			if(explodeOnTimer && explodeTimer == -100)
			{
				Explode();
			}

			
			if(!explodeOnTimer)
			{
				QueueFree();
			}
			
		}
		if(removePauseTimer <= 0)
		{
			if(GetTree().Paused)
			{
				(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(GetInstanceId());
			}
		}
		GlobalPosition += velocity * (float)delta;

		if(sprite != null)
		{
			animTimer += (float)delta * animSpeed;
			sprite.SetSprite(animation[Mathf.FloorToInt(animTimer) % animation.Count]);
		}
	}

	void _on_body_entered(Node2D body)
    {			
		if(delayTimer > 0)
		{
			return;
		}

		if(body is TileMap && explodeOnTimer)
		{
			Explode();
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
				if(explodeOnTimer)
				{
					Explode();
				}
			}
		}
    }

	void Explode()
	{
		if(explodeTimer != -100)
		{
			return;
		}
		if(sprite != null)
		{
			sprite.Visible = false;
		}
		RandomNumberGenerator random = new RandomNumberGenerator();
		for (int i = 0; i < 6; i++)
		{
			random.Randomize();
			int randX = random.RandiRange((int)(-explodeSize.X/4.0f),(int)(explodeSize.X/4.0f));
			random.Randomize();
			int randY = random.RandiRange((int)(-explodeSize.Y/4.0f),(int)(explodeSize.Y/4.0f));
			CreateGenericEffect(Effect.EffectType.cloud,Effect.EffectMovement.none,GlobalPosition + new Vector2(randX,randY),Effect.SpriteDirection.notFlipped);
		}

		
		velocity = Vector2.Zero;
		gravity = 0;
		((GetChild(0) as CollisionShape2D).Shape as RectangleShape2D).Size = explodeSize;
		explodeTimer = explodeForSec;
		timer = 0;
		soundManager.PlaySound("Bottle Explode");

	}

	public void CreateGenericEffect(Effect.EffectType type, Effect.EffectMovement movement, Vector2 pos, Effect.SpriteDirection spriteDir)
	{
		Effect cloud = GD.Load<PackedScene>("res://Prefabs/Effects/GenericEffect.tscn").Instantiate() as Effect;
		cloud.effect = type;
		cloud.direction = spriteDir;
		cloud.movement = movement;
		cloud.GlobalPosition = pos;
		GetParent().AddChild(cloud);
	}
}
