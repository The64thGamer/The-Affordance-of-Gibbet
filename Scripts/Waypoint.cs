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

		moveDirection += new Vector2(GlobalPosition.X,GlobalPosition.Z);

		int closestWaypoint = -1;
		float lowestAngle = Mathf.DegToRad(maxDegreesValidMoveCorrection);
		for (int i = 0; i < waypoints.Length; i++)
		{
			float currentAngle = moveDirection.AngleToPoint(new Vector2(waypoints[i].GlobalPosition.X,waypoints[i].GlobalPosition.Z));
			if(currentAngle < lowestAngle)
			{
				//GD.Print("Angle " + i + " was closer at " + currentAngle + " radians.");
				lowestAngle = currentAngle;
				closestWaypoint = i;
			}
		}
		if(closestWaypoint == -1)
		{
			GD.Print("Direction pressed wasn't near a valid path.");
			return null;
		}

		GD.Print("Selected waypoint with " + Mathf.RadToDeg(lowestAngle) + "degrees of closeness.");
		return waypoints[closestWaypoint];
	}
}
