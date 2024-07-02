using Godot;
using System;
using System.Collections.Generic;

public partial class PauseBufferHandler : Node
{
	[Export] Godot.Collections.Array<ulong> ids = new Godot.Collections.Array<ulong>();
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
	}

	public void AddPause(ulong id)
	{
		GetTree().Paused = true;
		ids.Add(id);
	}

	public void RemovePause(ulong id)
	{
		ids.Remove(id);
		if(ids.Count == 0)
		{
			GetTree().Paused = false;
		}
	}
}
