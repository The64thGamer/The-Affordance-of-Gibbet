using Godot;
using System;

public partial class GameMaster : Node
{
	int turnCounter;
	public override void _Ready()
	{
		AddToGroup("Game Masters");
	}

	public void PlayerReadyForThisTurn()
	{
		GD.Print("Turn " + turnCounter + " begins.");
		GetTree().CallGroup("Board Pieces", "StartTurn");
		turnCounter++;
	}
}
