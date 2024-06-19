using Godot;
using System;

public partial class IntegerScaleScreen : SubViewportContainer
{
	Vector2 last_size;

	public override void _Ready()
	{
		Visible = true;
		last_size = DisplayServer.WindowGetSize();

		if(last_size.X < last_size.Y)
		{
			int scale = Mathf.Max(1,Mathf.FloorToInt(last_size.X / 160.0f));
			DisplayServer.WindowSetSize(new Vector2I(scale * 160, scale * 144));
		}
		else
		{
			int scale = Mathf.Max(1,Mathf.FloorToInt(last_size.Y / 144.0f));
			DisplayServer.WindowSetSize(new Vector2I(scale * 160, scale * 144));
		}

		WindowChange();
	}
 
	public override void _Process(double delta)
	{
		if (last_size != DisplayServer.WindowGetSize())
		{
        	last_size = DisplayServer.WindowGetSize();
			if(last_size.X < 160 || last_size.Y < 144)
			{
				last_size = new Vector2(160,144);
				DisplayServer.WindowSetSize((Vector2I)last_size);
			}
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

		Position = new Vector2(((int)last_size.X / 2) - ((int)(Scale.X*160) / 2),((int)last_size.Y / 2) - ((int)(Scale.Y*144) / 2));
	}
}
