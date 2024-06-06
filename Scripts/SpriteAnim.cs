using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpriteAnim : Sprite2D
{
	[Export] string[] spriteNames;
	Dictionary<string,int> spriteHash;

	const int horizontalItems = 10;

	public override void _Ready()
	{
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
		}
	}
}
