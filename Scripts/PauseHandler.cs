using Godot;
using System;

public partial class PauseHandler : Node
{
	public float timer = 999;
	public ulong id;
	public override void _Process(double delta)
	{
		if(timer <= 0)
		{
			(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(id);
			QueueFree();
		}
		timer -= (float)delta;
	}
}
