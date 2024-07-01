using Godot;
using System;

public partial class TileMapSprites : TileMap
{

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		(Material as ShaderMaterial).SetShaderParameter("current_palette", PlayerPrefs.GetValue<int>("Tilemap Palette"));
	}
}