using Godot;
using System;

public partial class Waypoint : Node3D
{
	[Export] Waypoint[] waypoints;
 	
	const float maxDegreesValidMoveCorrection = 45;

	public Waypoint GetWaypoint(Vector2 moveDirection)
	{
		if(waypoints.Length == 0)
		{
			GD.Print("No Waypoints to move to.");
			return null;
		}

		int closestWaypoint = -1;
		float lowestAngle = Mathf.DegToRad(maxDegreesValidMoveCorrection);
		for (int i = 0; i < waypoints.Length; i++)
		{
			float currentAngle = Mathf.Abs(moveDirection.AngleTo(new Vector2(waypoints[i].GlobalPosition.X - GlobalPosition.X,waypoints[i].GlobalPosition.Z - GlobalPosition.Z)));
			GD.Print(i + " " + Mathf.RadToDeg(currentAngle));
			if(currentAngle < lowestAngle)
			{
				lowestAngle = currentAngle;
				closestWaypoint = i;
			}
		}
		if(closestWaypoint == -1)
		{
			GD.Print("Direction pressed wasn't near a valid path. ("+ moveDirection + ")");
			return null;
		}

		GD.Print("Selected waypoint with " + Mathf.RadToDeg(lowestAngle) + "degrees of closeness.");
		return waypoints[closestWaypoint];
	}
}
