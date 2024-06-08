using Godot;
using System;

public partial class CopyUI : Node
{
	[Export] SpriteAnim buttonB;
	[Export] SpriteAnim arrowUp;
	[Export] SpriteAnim arrowRight;
	[Export] SpriteAnim arrowDown;
	[Export] SpriteAnim arrowLeft;

	[Export] Curve moveOutCurve;
	
	CopyUIState uiState;
	float uiStateTimer;
	const int uiMaxOutPosition = 24;

	enum CopyUIState
	{
		off,
		starting,
		finished,
		upSelected,
		sidesSelected,
		neutralSelected,
		downSelected,
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		arrowUp.Visible = false;
		arrowRight.Visible = false;
		arrowDown.Visible = false;
		arrowLeft.Visible = false;
		buttonB.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		if (Input.IsActionJustPressed("Copy"))
		{
			CreateUI();
		}

		if(uiState == CopyUIState.off)
		{
			return;
		}

		uiStateTimer = Mathf.Min(1, uiStateTimer + (float)delta);

		switch (uiState)
		{
			case CopyUIState.starting:
				arrowUp.Position = Vector2.Zero.Lerp(Vector2.Up * uiMaxOutPosition,uiStateTimer);
				arrowRight.Position = Vector2.Zero.Lerp(Vector2.Right * uiMaxOutPosition,uiStateTimer);
				arrowDown.Position = Vector2.Zero.Lerp(-Vector2.Up * uiMaxOutPosition,uiStateTimer);
				arrowLeft.Position = Vector2.Zero.Lerp(-Vector2.Right * uiMaxOutPosition,uiStateTimer);
				
				if(uiStateTimer >= 1)
				{
					uiStateTimer = 0;
					uiState = CopyUIState.finished;
				}
			break;
			default:
			break;
		}
	}

	public void CreateUI()
	{
		if(uiState == CopyUIState.off)
		{
			uiState = CopyUIState.starting;
			uiStateTimer = 0;
			arrowUp.Visible = true;
			arrowRight.Visible = true;
			arrowDown.Visible = true;
			arrowLeft.Visible = true;
			buttonB.Visible = true;
		}
	}
}
