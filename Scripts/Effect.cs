using Godot;
using System;

public partial class Effect : Entity
{
	[Export] public EffectType effect;
	[Export] public EffectMovement movement;
	[Export] public SpriteDirection direction;
	public enum EffectType
	{
		cloud,
		dash,
		jumping,
		landing,
	}

	public enum EffectMovement
	{
		none,
		moveSlightlyLeft,
		moveSlightlyRight,
	}

	public enum SpriteDirection
	{
		notFlipped,
		flipped,
	}

	float timer;
	Vector2 initialPosition;
	bool firstFrame = true;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(firstFrame)
		{
			initialPosition = GlobalPosition;
			firstFrame = false;
		}

		if(direction == SpriteDirection.flipped)
		{
			sprite.FlipH = true;
		}

		switch (effect)
		{
			case EffectType.cloud:
				switch (Mathf.FloorToInt(timer))
				{
					case 0:
						sprite.SetSprite("Cloud A");
					break;
					case 1:
						sprite.SetSprite("Cloud B");
					break;
					case 2:
						sprite.SetSprite("Cloud A");
					break;
					case 3:
						sprite.SetSprite("Cloud B");
					break;
					case 4:
						sprite.SetSprite("Cloud A");
					break;
					case 5:
						sprite.SetSprite("Cloud C");
					break;
					case 6:
						sprite.SetSprite("Cloud D");
					break;
					default:
					QueueFree();
					break;
				}
				timer += (float) delta*10;
				break;
			case EffectType.dash:

				switch (Mathf.FloorToInt(timer))
				{
					case 0:
						sprite.SetSprite("Dash Lines");
					break;
					default:
					QueueFree();
					break;
				}
				timer += (float) delta*3;
				break;
			case EffectType.landing:

				switch (Mathf.FloorToInt(timer))
				{
					case 0:
						sprite.SetSprite("Land Ripple");
					break;
					default:
					QueueFree();
					break;
				}
				timer += (float) delta*5;
				break;
			case EffectType.jumping:

				switch (Mathf.FloorToInt(timer))
				{
					case 0:
						sprite.SetSprite("Speed Lines A");
					break;
					case 1:
						sprite.SetSprite("Speed Lines B");
					break;
					case 2:
						sprite.SetSprite("Speed Lines A");
					break;
					default:
					QueueFree();
					break;
				}
				timer += (float) delta*9;
				break;
			default:
				break;
		}

		switch (movement)
		{
			case EffectMovement.moveSlightlyLeft:
				GlobalPosition = initialPosition.Lerp(initialPosition + (Vector2.Right * -16),timer);
				break;
			case EffectMovement.moveSlightlyRight:
				GlobalPosition = initialPosition.Lerp(initialPosition + (Vector2.Right * 16),timer);
				break;
			default:
			break;
		}
		base._Process(delta);
	}
}
