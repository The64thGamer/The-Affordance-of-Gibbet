using Godot;
using System;

public partial class TileMapSprites : TileMap
{

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		(Material as ShaderMaterial).SetShaderParameter("current_palette", PlayerPrefs.GetValue<int>("Tilemap Palette"));

		if(Input.IsKeyPressed(Key.Key1))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",0);
			PlayerPrefs.SetValue<int>("Sprite Palette",0);
		}
		if(Input.IsKeyPressed(Key.Key2))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",5);
			PlayerPrefs.SetValue<int>("Sprite Palette",5);
		}
		if(Input.IsKeyPressed(Key.Key3))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",6);
			PlayerPrefs.SetValue<int>("Sprite Palette",6);
		}
		if(Input.IsKeyPressed(Key.Key4))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",7);
			PlayerPrefs.SetValue<int>("Sprite Palette",7);
		}
		if(Input.IsKeyPressed(Key.Key5))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",8);
			PlayerPrefs.SetValue<int>("Sprite Palette",14);
		}
		if(Input.IsKeyPressed(Key.Key6))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",3);
			PlayerPrefs.SetValue<int>("Sprite Palette",3);
		}
		if(Input.IsKeyPressed(Key.Key7))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",28);
			PlayerPrefs.SetValue<int>("Sprite Palette",27);
		}
		if(Input.IsKeyPressed(Key.Key8))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",27);
			PlayerPrefs.SetValue<int>("Sprite Palette",28);
		}
		if(Input.IsKeyPressed(Key.Key9))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",27);
			PlayerPrefs.SetValue<int>("Sprite Palette",0);
		}
		if(Input.IsKeyPressed(Key.Key0))
		{
			PlayerPrefs.SetValue<int>("Tilemap Palette",16);
			PlayerPrefs.SetValue<int>("Sprite Palette",23);
		}
	}
}
