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
	[Export] Control cursor;
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
			MoveUp();
		}
		if(Input.IsActionJustPressed("Down"))
		{
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
			if(CheckLabels())
			{
				ShiftLabels(8);
			}

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
			if(CheckLabels())
			{
				ShiftLabels(-8);
			}

			(this.GetParent() as Control).Visible = false;
			downMenu.GetCursor(cursor);
			cursor = null;
		}
	}

	bool CheckLabels()
	{
		bool firstCheck = false;
		bool abort = false;
		if(downMenu != null)
		{	
			UIButton button = downMenu;
			for (int i = 0; i < 2; i++)
			{
				UIButton test = button.downMenu;
				if(test == null)
				{
					abort = true;
					break;
				}
				button = test;
			}
			if((button.downMenu == null || !button.downMenu.Visible) && !abort)
			{
				firstCheck = true;
			}
		}
		abort = false;
		if(upMenu != null)
		{
			UIButton button = upMenu;
			for (int i = 0; i < 2; i++)
			{
				UIButton test = button.upMenu;
				if(test == null)
				{
					abort = true;
					break;
				}
				button = test;
			}
			if((button.upMenu == null || !button.upMenu.Visible) && firstCheck && !abort)
			{
				return true;
			}
		}
		return false;
	}

	void ShiftLabels(int shift)
	{
		Godot.Collections.Array<UIButton> buttonList = new Godot.Collections.Array<UIButton>
        {
            this
        };
		UIButton button = downMenu;
		int indexPath = 0;
		while(true)
		{
			if(button == null || button.downMenu == null)
			{
				break;
			}
			if(indexPath < 3)
			{
				button.Visible = true;
			}
			else
			{
				button.Visible = false;
			}
			buttonList.Add(button);
			button = button.downMenu;
			indexPath++;
		}
		button = upMenu;
		indexPath = 0;
		while(true)
		{
			if(button == null)
			{
				break;
			}
			if(indexPath < 3)
			{
				button.Visible = true;
			}
			else
			{
				button.Visible = false;
			}
			buttonList.Add(button);
			button = button.upMenu;
			indexPath++;
		}
		for (int i = 0; i < buttonList.Count; i++)
		{
			buttonList[i].GlobalPosition += new Vector2(0,shift);
		}
	}
}
