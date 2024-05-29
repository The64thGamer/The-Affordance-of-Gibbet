using Godot;
using System;

public partial class Player : BoardPiece
{
	//Camera
	[Export] Node3D playerHead;
	[Export] Node3D fakePlayerHead;

	//Signals
    [Signal] public delegate void PlayerTurnLocksInEventHandler();

  	public override void _UnhandledInput(InputEvent currentEvent)
    {
		//Camera Movement
		if(currentEvent is InputEventMouseMotion motion)
		{
            fakePlayerHead.RotateY(-motion.Relative.X * .001f);
        }
    }

	public override void _Process(double delta)
	{
		if(actionTimeLeft <= 0)
		{
			GetTree().CallGroup("Game Masters", "PlayerReadyForThisTurn");		
		}

		//Head Rotation
		playerHead.Basis = playerHead.Basis.Orthonormalized().Slerp(fakePlayerHead.Basis.Orthonormalized(),(float)delta * 6);

		//Player Movement
		base._Process(delta);
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (playerHead.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if(direction != Vector3.Zero)
		{
			if(!currentlyOnTurn)
			{			
				Move(new Vector2(direction.X,direction.Z));
    			GetTree().CallGroup("Game Masters", "PlayerReadyForThisTurn");		
			}
		}
	}

	 
}
