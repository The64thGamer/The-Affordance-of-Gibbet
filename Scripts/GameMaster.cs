using Godot;
using System;

public partial class GameMaster : Node
{
	float minTurnTime = 5;
	float turnExecutionTime = 1.5f;
	int turnCounter;
	public override void _Ready()
	{
		AddToGroup("Game Masters");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public void PlayerReadyForThisTurn()
	{
		GD.Print("Turn " + turnCounter + " begins.");
		GetTree().CallGroup("Board Pieces", "StartTurn",minTurnTime,turnExecutionTime);
		turnCounter++;
	}
}
