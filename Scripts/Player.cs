using Godot;
using System;

public partial class Player : BoardPiece
{
	[Signal]
    public delegate void PlayerTurnLocksInEventHandler();

	public override void _Process(double delta)
	{
		base._Process(delta);
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

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
