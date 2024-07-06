using Godot;
using System;

public partial class LevelRootMover : Node
{
	[Export] Node level;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		level.GetParent().CallDeferred("remove_child",level);	
	}

	public override void _Process(double delta)
	{
		if(level.GetParent() == null)
		{
			this.AddChild(level);
			SetScript(new Variant());
		}
	}
}
