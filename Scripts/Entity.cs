using Godot;
using System;
using System.Diagnostics;

public partial class Entity : CharacterBody2D
{
	protected bool isVisibletoCamera;
	protected SpriteAnim sprite;
	public override void _Ready()
	{
		if(HasNode("Sprite"))
		{
			sprite = GetNode("Sprite") as SpriteAnim;
			Velocity = Vector2.Zero;
		}
	}
		
	public void SetCamVisibility(bool set)
	{
		isVisibletoCamera = set;
	}
}
