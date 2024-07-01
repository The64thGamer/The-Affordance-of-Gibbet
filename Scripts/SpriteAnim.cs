using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpriteAnim : Sprite2D
{
	Dictionary<string,int> spriteHash = new Dictionary<string, int>();

	static string[] spritePlayer = new string[]{
		"Idle",
		"Walk A","Walk B","Walk C",
		"Jump",
		"Roll A","Roll B","Roll C",
		"Copy B","Copy C","Copy A",
		"Crouch",
		"Soda Side A","Soda Side B","Soda Side C",
		};
	static string[] spriteUI = new string[]{
		"Button B",
		"Button Left Arrow",
		"Button Up Arrow",
		"Button Down Arrow",
	};
	static string[] spriteEnemy = new string[]{
		"Wibbly Idle A","Wibbly Idle B","Wibbly Die","Wibbly Walk A", "Wibbly Walk B", 
		"Heavy Idle A", "Heavy Crouch", "Heavy Jump", "Heavy Die",
		"Tipsy Idle A", "Tipsy Walk A", "Tipsy Walk B", "Tipsy Die"
	};
	static string[] spriteEffects = new string[]{
		"Cloud A", "Cloud B", "Cloud C", "Cloud D",
	};

	public override void _Ready()
	{
		string[] spriteNames = new string[0];
		switch (Texture.ResourcePath.GetFile())
		{
			case "Sprite Player.png":
				spriteNames = spritePlayer;
				break;
			case "Sprite UI.png":
				spriteNames = spriteUI;
				break;
			case "Sprite Enemy.png":
				spriteNames = spriteEnemy;
				break;
			case "Sprite Effects.png":
				spriteNames = spriteEffects;
				break;
			default:
				GD.PrintErr("Unknown Sprite Set: " + Texture.ResourcePath.GetFile());
				break;
		}
		int e;
		for (int i = 0; i < spriteNames.Length; i++)
		{
			e = i;
			spriteHash.Add(spriteNames[i],e);
		}
	}
	public override void _Process(double delta)
	{
		(Material as ShaderMaterial).SetShaderParameter("current_palette", PlayerPrefs.GetValue<int>("Sprite Palette"));
	}

	public void SetSprite(string name)
	{
		if(spriteHash.Count == 0)
		{
			return;
		}
		if(spriteHash.TryGetValue(name,out int index))
		{
			Frame = index;
			return;
		}

		GD.PrintErr("Unknown Sprite '" + name + "'");
	}

	public void Posterize(int tier)
	{
		Visible = false;
	}
}
