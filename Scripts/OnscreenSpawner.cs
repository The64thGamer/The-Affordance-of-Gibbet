using Godot;
using System;

public partial class OnscreenSpawner : Entity
{
	[Export] PackedScene spawntype;

	[Export] Node spawned = null;
	bool spawnSuccess = false;

	public override void _Process(double delta)
	{
		if(isVisibletoCamera && !spawnSuccess)
		{
			spawned = spawntype.Instantiate();
			AddChild(spawned);
			spawnSuccess = true;
		}

		if(!IsInstanceValid(spawned) && spawnSuccess && !isVisibletoCamera)
		{
			spawnSuccess = false;
		}
	}
}
