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
		};
	static string[] spriteUI = new string[]{
		"Button B",
		"Button Left Arrow",
		"Button Up Arrow",
		"Button Down Arrow",
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

	public void SetSprite(string name)
	{
		if(spriteHash.TryGetValue(name,out int index))
		{
			Frame = index;
			return;
		}

		GD.PrintErr("Unknown Sprite '" + name + "'");
	}
}
