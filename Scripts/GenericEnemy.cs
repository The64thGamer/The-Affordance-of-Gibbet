using Godot;
using System;

[Tool]
public partial class GenericEnemy : Entity
{

    [Export]
    public AIPattern AIType
    {
        get => aiType;
        set
        {
            aiType = value;
            NotifyPropertyListChanged();
        }
    }
	[ExportSubgroup("Generic Values")]
	[Export] float gravity = 20f;
	[Export] float walkAnimSpeed = 20f;
	[Export] float idleAnimSpeed = 20f;
	
	float walkTimer;
	float idleTimer;

	public enum AIPattern
	{
		none,
		walkLeft,
		walkRight,
		jump,
	}

    AIPattern aiType;
    float walkSpeed;
    float jumpHeight;
	float timeBetweenJumps;

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        var properties = new Godot.Collections.Array<Godot.Collections.Dictionary>
        {
            new Godot.Collections.Dictionary(){
                { "name", "walkSpeed" },
                { "type", (int)Variant.Type.Float },
                { "usage", (AIType == AIPattern.walkLeft || AIType == AIPattern.walkRight) ? (int)PropertyUsageFlags.Default : (int)PropertyUsageFlags.NoEditor}},
            new Godot.Collections.Dictionary(){
                { "name", "jumpHeight" },
                { "type", (int)Variant.Type.Float },
                { "usage",AIType == AIPattern.jump ? (int)PropertyUsageFlags.Default : (int)PropertyUsageFlags.NoEditor }},
			new Godot.Collections.Dictionary(){
                { "name", "timeBetweenJumps" },
                { "type", (int)Variant.Type.Float },
                { "usage",AIType == AIPattern.jump ? (int)PropertyUsageFlags.Default : (int)PropertyUsageFlags.NoEditor }}
        };

        return properties;
    }
	

	public override void _PhysicsProcess(double delta)
	{
		if (!Engine.IsEditorHint())
		{
		switch (AIType)
		{
			case AIPattern.walkLeft:
				GenericMove(-Vector2.Right,false,delta);
			break;
			case AIPattern.walkRight:
				GenericMove(Vector2.Right,false,delta);
			break;
			default:
				GenericMove(Vector2.Zero,false,delta);
			break;
		}
		}
	}

	void GenericMove(Vector2 direction, bool jump, double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (jump && IsOnFloor())
			velocity.Y = jumpHeight;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * walkSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed);
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
				switch (Mathf.FloorToInt(idleTimer))
				{
					case 0:
						sprite.SetSprite("Idle A");
					break;
					case 1:
						sprite.SetSprite("Idle B");
					break;
					default:
					idleTimer = 0;
					break;
				}
				idleTimer += (float)delta * idleAnimSpeed;
			}
			if(Velocity.X != 0)
			{
				switch (Mathf.FloorToInt(walkTimer))
				{
					case 0:
						sprite.SetSprite("Walk A");
					break;
					case 1:
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
