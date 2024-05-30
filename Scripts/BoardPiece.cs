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
	protected float actionTimeLeft;
	protected float turnExecutionTime;

	public override void _Ready()
	{
		AddToGroup("Board Pieces");
	}

	public override void _Process(double delta)
	{
		//Before Turn
		actionTimeLeft -= (float)delta;
		
		//On Turn
		if(currentlyOnTurn)
		{
			//Calculate Lerp
			currentTurnTimeLerp += (float)delta / turnExecutionTime;
			if(currentTurnTimeLerp > 1)
			{
				currentTurnTimeLerp = 1;
				currentlyOnTurn = false;
			}

			//Move
			if(futureWaypoint != null)
			{
				GlobalPosition = currentWaypoint.GlobalPosition.Lerp(futureWaypoint.GlobalPosition,1-((1 - currentTurnTimeLerp) * (1 - currentTurnTimeLerp)));


				//Change Waypoints
				if(!currentlyOnTurn)
				{
					currentWaypoint = futureWaypoint;
					futureWaypoint = null;
				}
			}
			else
			{
				GlobalPosition = currentWaypoint.GlobalPosition;
			}
		}
	}

	protected bool Move(Vector2 moveDirection)
	{
		if(currentWaypoint == null)
		{
			return false;
		}

		futureWaypoint = currentWaypoint.GetWaypoint(moveDirection);

		if(futureWaypoint == null)
		{
			return false;
		}
		return true;
	}

	public void StartTurn(float minTurnTime,float turnExecTime)
	{
		currentlyOnTurn = true;
		currentTurnTimeLerp = 0;
		actionTimeLeft = minTurnTime;
		turnExecutionTime = turnExecTime;
	}
}