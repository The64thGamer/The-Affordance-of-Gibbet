using Godot;
using System;

public partial class Door : Area2D
{
	[Export] int toArea;
	[Export] Vector2 toPosition;
	Godot.Collections.Array<Player> players = new Godot.Collections.Array<Player>();
	const float doorTimer = 0.5f;
	float timer;
	bool doorGo;
	public override void _Process(double delta)
	{
		if(doorGo)
		{
			timer -= (float)delta;
			if(timer <= 0)
			{
				for (int i = 0; i < players.Count; i++)
				{
					players[i].ChangeState(Player.PlayerState.standard);
				}
				(GetTree().CurrentScene as GameState).LevelChange(toArea,toPosition);
			}
		}

		if(Input.IsActionJustPressed("Menu Up") && players.Count > 0)
		{
			for (int i = 0; i < players.Count; i++)
			{
				players[i].ChangeState(Player.PlayerState.enteringDoor);
				doorGo = true;
				timer = doorTimer;
			}
		}
	}

	void _on_body_entered(PhysicsBody2D body)
    {			
		if(body is Player)
		{
			Player player = body as Player;
			if(!players.Contains(player))
			{
				players.Add(player);
			}
			else
			{
				GD.PrintErr("Player already in door collision somehow");
				players.Remove(player);
			}
		}
    }

	void _on_body_exited(PhysicsBody2D body)
    {			
		if(body is Player)
		{
			Player player = body as Player;
			if(players.Contains(player))
			{
				players.Remove(player);
			}
			else
			{
				GD.PrintErr("Player exited door collision without having first entered");
			}
		}
    }
}
