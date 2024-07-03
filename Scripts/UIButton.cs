using Godot;
using System;

public partial class UIButton : Label
{
	[Export] public UIButton subMenu;
	[Export] public UIButton upMenu;
	[Export] public UIButton downMenu;
	[Export] PackedScene scene;
	[Export] public int setSpritePalette;
	[Export] public int setTilemapPalette;
	[Export] public bool setPalette;
	[Export] public TitleScreen regenPalleteList;
	[Export] public int regenPalleteindex;
	Control cursor;
	bool stopMovement;
	public override void _Process(double delta)
	{
		(Material as ShaderMaterial).SetShaderParameter("current_palette", PlayerPrefs.GetValue<int>("Tilemap Palette"));
		if(stopMovement)
		{
			stopMovement = false;
			return;
		}
		if(Input.IsActionJustPressed("Attack"))
		{
			GiveCursor();
		}
		if(Input.IsActionJustPressed("Up"))
		{
			if(regenPalleteList != null)
			{
				cursor = null;
			}
			MoveUp();
		}
		if(Input.IsActionJustPressed("Down"))
		{
			if(regenPalleteList != null)
			{
				cursor = null;
			}
			MoveDown();
		}
	}
	public void GetCursor(Control newCursor)
	{
		(this.GetParent() as Control).Visible = true;
		cursor = newCursor;
		cursor.GlobalPosition = GlobalPosition;
		stopMovement = true;
	}
	public void GiveCursor()
	{
		if(cursor == null)
		{
			return;
		}
		if(subMenu != null)
		{
			(this.GetParent() as Control).Visible = false;
			subMenu.GetCursor(cursor);
			cursor = null;
		}
		else if(scene != null)
		{
			GetTree().ChangeSceneToPacked(scene);
		}
		else if(setPalette)
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",setTilemapPalette);
			PlayerPrefs.SetValue<int>("Sprite Palette",setSpritePalette);
		}
	}
	public void MoveUp()
	{
		if(cursor == null)
		{
			return;
		}
		if(upMenu != null)
		{
			(this.GetParent() as Control).Visible = false;
			upMenu.GetCursor(cursor);
			cursor = null;
		}
	}
	public void MoveDown()
	{
		if(cursor == null)
		{
			return;
		}
		if(downMenu != null)
		{
			(this.GetParent() as Control).Visible = false;
			downMenu.GetCursor(cursor);
			cursor = null;
		}
	}
	
}
