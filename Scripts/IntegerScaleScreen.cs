using Godot;
using System;

public partial class IntegerScaleScreen : SubViewportContainer
{
	Vector2 last_size;

	public override void _Ready()
	{
		last_size = DisplayServer.WindowGetSize();
	}
 
	public override void _Process(double delta)
	{
		if (last_size != DisplayServer.WindowGetSize())
		{
        	last_size = DisplayServer.WindowGetSize();
			WindowChange();
		}
	}

	void WindowChange()
	{
		if(last_size.X < last_size.Y)
		{
			Scale = Vector2.One * Mathf.Max(1,Mathf.FloorToInt(last_size.X / 160.0f));
			return;
		}

		Scale = Vector2.One * Mathf.Max(1,Mathf.FloorToInt(last_size.Y / 144.0f));
	}
}
