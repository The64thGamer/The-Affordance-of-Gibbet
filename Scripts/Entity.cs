using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	protected SpriteAnim sprite;
	public override void _Ready()
	{
		Velocity = Vector2.Zero;
		sprite = GetNode("Sprite") as SpriteAnim;
	}
}
