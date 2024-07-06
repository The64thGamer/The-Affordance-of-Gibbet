using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIButton : Label
{
	[Export] public UIButton subMenu;
	[Export] public UIButton upMenu;
	[Export] public UIButton downMenu;
	[Export] public UIButton backMenu;
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
		if(Input.IsActionJustPressed("Menu Accept"))
		{
			GiveCursor();
		}
		if(Input.IsActionJustPressed("Menu Up"))
		{
			MoveUp();
		}
		if(Input.IsActionJustPressed("Menu Down"))
		{
			MoveDown();
		}
		if(Input.IsActionJustPressed("Menu Back"))
		{
			MoveBack();
		}
	}



	public void GetCursor(Control newCursor)
	{
		(this.GetParent() as Control).Visible = true;
		cursor = newCursor;
		if(index != -1)
		{
			AdjustList();
		}
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

	void MoveBack()
	{
		if(cursor == null)
			{
				return;
			}

			if(backMenu != null)
			{
				(this.GetParent() as Control).Visible = false;
				backMenu.GetCursor(cursor);
				cursor = null;
			}
	}

	void AdjustList()
	{

		List<UIButton> buttonList = new List<UIButton>()
        {
            this
        };
		UIButton button = downMenu;
		while(true)
		{
			if(button == null || button.downMenu == null)
			{
				break;
			}
			buttonList.Add(button);
			button = button.downMenu;
		}
		button = upMenu;
		while(true)
		{
			if(button == null)
			{
				break;
			}
			buttonList.Add(button);
			button = button.upMenu;
		}

		buttonList = buttonList.OrderBy(f => f.index).ToList();

		if(index < 3)
		{
			for (int i = 0; i < buttonList.Count; i++)
			{
				if(i < 7)
				{
					buttonList[i].Visible = true;
				}	
				else
				{
					buttonList[i].Visible = false;
				}
				buttonList[i].GlobalPosition = new Vector2(16,64+(i * 8));
			}
		}
		else if(index > maxList-4)
		{
			for (int i = 0; i < buttonList.Count; i++)
			{
				if(i > maxList-8)
				{
					buttonList[i].Visible = true;
				}	
				else
				{
					buttonList[i].Visible = false;
				}
			}		
		}
		else
		{
			for (int i = 0; i < buttonList.Count; i++)
			{
				if(i >= index-3 && i <= index+3)
				{
					buttonList[i].Visible = true;
				}	
				else
				{
					buttonList[i].Visible = false;
				}
				buttonList[i].GlobalPosition = new Vector2(16,64+((i - (index-3)) * 8));
			}
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
}
