using Godot;
using System;

public partial class Player : BoardPiece
{
	//Camera
	[Export] Node3D playerHead;
	[Export] Node3D fakePlayerHead;
	public override void _Ready()
	{
		base._Ready();
		AddToGroup("Players");
	}
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
		base._Process(delta);

		if(actionTimeLeft <= 0)
		{
			GetTree().CallGroup("Game Masters", "PlayerReadyForThisTurn");		
		}

		//Head Rotation
		playerHead.Basis = playerHead.Basis.Orthonormalized().Slerp(fakePlayerHead.Basis.Orthonormalized(),(float)delta * 6);

		//Player Movement
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (playerHead.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if(direction != Vector3.Zero)
		{
			if(!currentlyOnTurn)
			{			
				if(Move(new Vector2(direction.X,direction.Z)))
				{
    				GetTree().CallGroup("Game Masters", "PlayerReadyForThisTurn");	
				}	
			}
		}
	}

	 
}
