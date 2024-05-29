using Godot;
using System;

public partial class BoardPiece : Node3D
{
	//Waypoints
	[Export] 
	protected Waypoint currentWaypoint;
	protected Waypoint futureWaypoint;

	//Turns
	protected bool currentlyOnTurn;
	protected float currentTurnTimeLerp;

	public override void _Ready()
	{
		AddToGroup("Board Pieces");
	}

	public override void _Process(double delta)
	{
		if(currentlyOnTurn)
		{
			//Calculate Lerp
			currentTurnTimeLerp += (float)delta;
			if(currentTurnTimeLerp > 1)
			{
				currentTurnTimeLerp = 1;
				currentlyOnTurn = false;
			}

			//Move
			if(futureWaypoint != null)
			{
				GlobalPosition = currentWaypoint.GlobalPosition.Lerp(futureWaypoint.GlobalPosition,currentTurnTimeLerp);


				//Change Waypoints
				if(!currentlyOnTurn)
				{
					currentWaypoint = futureWaypoint;
					futureWaypoint = null;
				}
			}
		}
	}

	protected void Move(Vector2 moveDirection)
	{
		if(currentWaypoint == null)
		{
			return;
		}

		futureWaypoint = currentWaypoint.GetWaypoint(moveDirection);
	}

	public void StartTurn()
	{
		currentlyOnTurn = true;
		currentTurnTimeLerp = 0;
	}
}