using Godot;
using System;

public partial class Waypoint : Node3D
{
	[Export] Waypoint[] waypoints;
 	
	public Waypoint GetWaypoint(Vector2 moveDirection)
	{
		if(waypoints.Length == 0)
		{
			return null;
		}

		int closestWaypoint = 0;
		float lowestAngle = float.MaxValue;
		for (int i = 0; i < waypoints.Length; i++)
		{
			float currentAngle = moveDirection.AngleToPoint(new Vector2(waypoints[i].GlobalPosition.X,waypoints[i].GlobalPosition.Y));
			if(currentAngle < lowestAngle)
			{
				//GD.Print("Angle " + i + " was closer at " + currentAngle + " radians.");
				lowestAngle = currentAngle;
				closestWaypoint = i;
			}
		}

		GD.Print("Selected waypoint with " + Mathf.RadToDeg(lowestAngle) + "degrees of closeness.");
		return waypoints[closestWaypoint];
	}
}
