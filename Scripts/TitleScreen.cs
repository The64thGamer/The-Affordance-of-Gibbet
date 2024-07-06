using Godot;
using System;

public partial class TitleScreen : Node
{
	[Export] UIButton startingMenu;
	[Export] Control cursor;
	[Export] Control paletteMenu;
	[Export] UIButton palleteGoButton;
	[Export] UIButton palleteBackButton;
	[Export] PackedScene uiButton;
	const int maxMenuCount = 7;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startingMenu.GetCursor(cursor);

		string translation;
		UIButton prevButton = null;
		int paletteCount = 0;
		while(true)
		{
			translation = Tr("PALETTE_" + paletteCount + "_NAME");
			if(translation == "PALETTE_" + paletteCount + "_NAME")
			{
				break;
			}
			paletteCount++;
		}
		UIButton button;
		int i = 0;
		while(true)
		{
			translation = Tr("PALETTE_" + i + "_NAME");
			if(translation == "PALETTE_" + i + "_NAME")
			{
				break;
			}
			button = uiButton.Instantiate() as UIButton;
			paletteMenu.AddChild(button);
			button.GlobalPosition = new Vector2(16,64+(i * 8));
			button.Text = translation;
			button.setPalette = true;
			button.setSpritePalette = Convert.ToInt32(Tr("PALETTE_" + i + "_SPRITES"));
			button.setTilemapPalette = Convert.ToInt32(Tr("PALETTE_" + i + "_TILEMAP"));
			button.index = i;
			button.backMenu = palleteBackButton.backMenu;
			button.maxList = paletteCount;
			if(prevButton == null)
			{
				palleteGoButton.subMenu = button;
			}
			else
			{
				prevButton.downMenu = button;
				button.upMenu = prevButton;
			}
			prevButton = button;
			i++;

			if(i > maxMenuCount)
			{
				button.Visible = false;
			}
		}
		prevButton.downMenu = palleteBackButton;
		palleteBackButton.upMenu = prevButton;
	}
}