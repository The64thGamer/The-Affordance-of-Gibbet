using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIButton : Label
{
	[ExportGroup("Menus")]
	[Export] public UIButton subMenu;
	[Export] public UIButton upMenu;
	[Export] public UIButton downMenu;
	[Export] public UIButton backMenu;
	[ExportGroup("Actions")]
	[Export] PackedScene scene;
	[Export] public bool resetValues;

	[ExportGroup("Change Palette")]
	[Export] public int setSpritePalette;
	[Export] public int setTilemapPalette;
	[Export] public bool setPalette;
	[ExportGroup("Adjustable Setting")]
	[Export] public string savePrefName;
	public int savePrefValue;
	[Export] public int savePrefNotches;
	[Export] public float savePrefRemapMin;
	[Export] public float savePrefRemapMax;
	[ExportGroup("Ect")]
	[Export] Control cursor;
	public int index = -1;
	public int maxList = -1;
	bool stopMovement;
	string oldText;
	SoundManager soundManager;

	public override void _Ready()
	{
		soundManager = (GetNode("/root/SoundManager") as SoundManager);
		if(cursor != null && !PlayerPrefs.GetBool("NotTheFirstTimeLoading_A"))
		{
			ResetValues();
		}
		oldText = Text;
	}

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
		if(!String.IsNullOrEmpty(savePrefName))
		{
			if(Visible)
			{
				UpdateSavePrefValueFromDisc();
				UpdateSliderText();
			}
			if(Input.IsActionJustPressed("Menu Left"))
			{
				SetPlayerPref(-1);
			}
			if(Input.IsActionJustPressed("Menu Right"))
			{
				SetPlayerPref(1);
			}
		}
	}

	void UpdateSavePrefValueFromDisc()
	{
		//Hey btw possible floating point error reconverting the value back into an int.
		//Amazing future bug fix for whoever finds this.
		savePrefValue = (int)(Remap(PlayerPrefs.GetFloat(savePrefName),savePrefRemapMin,savePrefRemapMax,0,1)*savePrefNotches);
	}

	void UpdateSliderText()
	{
		Text = oldText + " ";
		for (int i = 0; i < savePrefNotches+1; i++)
		{
			if(i == savePrefValue)
			{
				Text += "^";
			}
			else
			{
				Text += "-";
			}
		}
	}

	void SetPlayerPref(int addedValue)
	{
		if(cursor == null)
		{
			return;
		}

		savePrefValue = Mathf.Clamp(savePrefValue + addedValue,0,savePrefNotches);

		PlayerPrefs.SetFloat(savePrefName,Remap((float)savePrefValue / (float)savePrefNotches,0,1,savePrefRemapMin,savePrefRemapMax));
	}

	public float Remap(float value, float aIn1, float aIn2, float aOut1, float aOut2)
    {
        float t = (value - aIn1) / (aIn2 - aIn1);
        t = Mathf.Clamp(t,0,1);
        return aOut1 + (aOut2 - aOut1) * t;
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
			soundManager.PlaySound("Select Sound");
		}
		else if(scene != null)
		{
			GetTree().ChangeSceneToPacked(scene);
		}
		else if(setPalette)
		{
			soundManager.PlaySound("PaletteSelect");
			PlayerPrefs.SetValue<int>("Tilemap Palette",setTilemapPalette);
			PlayerPrefs.SetValue<int>("Sprite Palette",setSpritePalette);
		}
		else if(resetValues)
		{
			ResetValues();
		}
	}

	void ResetValues()
	{
		PlayerPrefs.SetValue<int>("Tilemap Palette",Convert.ToInt32(Tr("PALETTE_0_TILEMAP")));
		PlayerPrefs.SetValue<int>("Sprite Palette",Convert.ToInt32(Tr("PALETTE_0_SPRITES")));
		PlayerPrefs.SetValue<float>("RumbleIntensity",1);
		PlayerPrefs.SetValue<float>("RumbleTime",1);
		PlayerPrefs.SetValue<float>("SoundVolume",1);
		PlayerPrefs.SetValue<float>("MusicVolume",1);
		PlayerPrefs.SetValue<bool>("NotTheFirstTimeLoading",true);
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
			soundManager.PlaySound("ChangeSelect");
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
			soundManager.PlaySound("ChangeSelect");
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
			soundManager.PlaySound("ChangeSelect");
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
