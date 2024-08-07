using Godot;
using System;

public partial class CopyUI : Area2D
{
	[Export] Player player;
	[Export] SpriteAnim buttonB;
	[Export] SpriteAnim arrowUp;
	[Export] SpriteAnim arrowRight;
	[Export] SpriteAnim arrowDown;
	[Export] SpriteAnim arrowLeft;

	[Export] Curve moveOutCurve;
	
	CopyUIState uiState;
	float uiStateTimer;
	const int uiMaxOutPosition = 20;
	const float uiOutTimer = .2f;
	const int uiNotSelectedPosterization = -2;
	const float uiSelectedTimer = 0.3f;
	const float minNoCopyAbilityTime = 0.1f;
	Player.CopyAbility heldAbility;

	enum CopyUIState
	{
		off,
		starting,
		finished,
		upSelected,
		sidesSelected,
		downSelected,
		goingOff,
		noCopyAbility,
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
		if(uiState == CopyUIState.off)
		{
			return;
		}

		uiStateTimer = Mathf.Min(1, uiStateTimer + (float)delta);

		switch (uiState)
		{
			case CopyUIState.starting:
				arrowUp.SetSprite("Button Up Arrow");
				arrowRight.SetSprite("Button Left Arrow");
				arrowDown.SetSprite("Button Down Arrow");
				arrowLeft.SetSprite("Button Left Arrow");

				arrowUp.Position = Vector2.Zero.Lerp(Vector2.Up * uiMaxOutPosition,uiStateTimer / uiOutTimer);
				arrowRight.Position = Vector2.Zero.Lerp(Vector2.Right * uiMaxOutPosition,uiStateTimer / uiOutTimer);
				arrowDown.Position = Vector2.Zero.Lerp(-Vector2.Up * uiMaxOutPosition,uiStateTimer / uiOutTimer);
				arrowLeft.Position = Vector2.Zero.Lerp(-Vector2.Right * uiMaxOutPosition,uiStateTimer / uiOutTimer);
				
				if(uiStateTimer >= uiOutTimer)
				{
					uiStateTimer = 0;
					uiState = CopyUIState.finished;
				}
			break;
			case CopyUIState.finished:
			if (Input.IsActionJustPressed("Attack") || Input.IsActionJustPressed("Jump"))
			{
				uiStateTimer = 0;
				
				if(Input.IsActionPressed("Up"))
				{
					player.SetCopyAbility(heldAbility,1);
					uiState = CopyUIState.upSelected;
					break;
				}
				if(Input.IsActionPressed("Down"))
				{
					player.SetCopyAbility(heldAbility,2);
					uiState = CopyUIState.downSelected;
					break;
				}
				else
				{
					player.SetCopyAbility(heldAbility,0);
					uiState = CopyUIState.sidesSelected;
					break;
				}
			}
			break;
			case CopyUIState.upSelected:
				arrowDown.Posterize(uiNotSelectedPosterization);
				arrowLeft.Posterize(uiNotSelectedPosterization);
				arrowRight.Posterize(uiNotSelectedPosterization);
				if(uiStateTimer >= uiSelectedTimer)
				{
					uiStateTimer = 0;
					uiState = CopyUIState.goingOff;
				}
			break;
			case CopyUIState.downSelected:
				arrowUp.Posterize(uiNotSelectedPosterization);
				arrowLeft.Posterize(uiNotSelectedPosterization);
				arrowRight.Posterize(uiNotSelectedPosterization);
				if(uiStateTimer >= uiSelectedTimer)
				{
					uiStateTimer = 0;
					uiState = CopyUIState.goingOff;
				}
			break;
			case CopyUIState.sidesSelected:
				arrowDown.Posterize(uiNotSelectedPosterization);
				arrowUp.Posterize(uiNotSelectedPosterization);
				if(uiStateTimer >= uiSelectedTimer)
				{
					uiStateTimer = 0;
					uiState = CopyUIState.goingOff;
				}
			break;
			case CopyUIState.goingOff:
				arrowUp.Visible = false;
				arrowRight.Visible = false;
				arrowDown.Visible = false;
				arrowLeft.Visible = false;
				buttonB.Visible = false;
				uiStateTimer = 0;
				uiState = CopyUIState.off;
				(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(GetInstanceId());
			break;
			case CopyUIState.noCopyAbility:
				
				if(uiStateTimer > minNoCopyAbilityTime)
				{
					uiState = CopyUIState.off;
					(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).RemovePause(GetInstanceId());
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

	void _on_body_entered(PhysicsBody2D body)
    {
		if(body is GenericEnemy)
		{
			GenericEnemy enemy = body as GenericEnemy;
			if(!enemy.IsDead())
			{
				enemy.Die();

				if(player.GetPlayerState() != Player.PlayerState.sideAttack 
				&& player.GetPlayerState() != Player.PlayerState.downAttack 
				&& player.GetPlayerState() != Player.PlayerState.upAttack 
				&& uiState == CopyUIState.off
				)
				{
					(GetNode("/root/PauseBufferHandler") as PauseBufferHandler).AddPause(GetInstanceId());
					if(enemy.copyAbility != Player.CopyAbility.none)
					{
						heldAbility = enemy.copyAbility;
						CreateUI();
					}
					else
					{
						uiState = CopyUIState.noCopyAbility;
						uiStateTimer = 0;
					}
				}
			}
		}
    }
}
