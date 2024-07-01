using Godot;
using System;

public partial class TitleScreen : Node
{
	[Export] UIButton startingMenu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

class PaletteType
{
	public uint tileMap;
	public uint sprites;
}