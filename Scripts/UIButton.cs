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
	public int index = -1;
	public int maxList = -1;
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
				ShiftLabels(8,2,3);
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
				ShiftLabels(-8,3,2);
			}

			(this.GetParent() as Control).Visible = false;
			downMenu.GetCursor(cursor);
			cursor = null;
		}
	}

	bool CheckLabels()
	{
		if(index > 2 && index < maxList-2)
		{
			return true;
		}
		return false;
	}

	void ShiftLabels(int shift, int lowIndex, int highIndex)
	{
		Godot.Collections.Array<UIButton> buttonList = new Godot.Collections.Array<UIButton>
        {
            this
        };
		UIButton button = downMenu;
		int indexPath = 0;
		bool abortVis = false;

		if (index == 0)
		{
    		lowIndex = 6;
    		highIndex = 0;
		}
		else if (index == 1)
		{
			lowIndex = 5;
			highIndex = 1;
		}
		else if (index == 2)
		{
			lowIndex = 4;
			highIndex = 2;
		}
		else if (index == maxList - 1)
		{
			lowIndex = 0;
			highIndex = 6;
		}
		else if (index == maxList - 2)
		{
			lowIndex = 1;
			highIndex = 5;
		}
		else if (index == maxList - 3)
		{
			lowIndex = 2;
			highIndex = 4;
		}

		while(true)
		{
			if(button == null || button.downMenu == null)
			{
				break;
			}
			if(indexPath < lowIndex)
			{
				button.Visible = true;
				if(button.index == maxList-1)
				{
					abortVis = true;
				}
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
			if(indexPath < highIndex)
			{
				button.Visible = true;
				if(button.index == 0)
				{
					abortVis = true;
				}
			}
			else
			{
				button.Visible = false;
			}
			buttonList.Add(button);
			button = button.upMenu;
			indexPath++;
		}
		if(!abortVis)
		{
		for (int i = 0; i < buttonList.Count; i++)
		{
			buttonList[i].GlobalPosition += new Vector2(0,shift);
		}
		}
	}
}
