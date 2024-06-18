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

	public void CreateGenericEffect(Effect.EffectType type, Effect.EffectMovement movement, Vector2 pos)
	{
		Effect cloud = GD.Load<PackedScene>("res://Prefabs/Effects/GenericEffect.tscn").Instantiate() as Effect;
		cloud.effect = type;
		cloud.movement = movement;
		cloud.GlobalPosition = pos;
		GetParent().AddChild(cloud);
	}
}
