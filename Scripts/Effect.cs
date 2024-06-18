using Godot;
using System;

public partial class Effect : Entity
{
	[Export] public EffectType effect;
	[Export] public EffectMovement movement;
	public enum EffectType
	{
		cloud,
	}

	public enum EffectMovement
	{
		none,
	}

	float timer;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
			default:
				break;
		}
		base._Process(delta);
	}
}
