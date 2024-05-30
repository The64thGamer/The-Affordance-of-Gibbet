using Godot;

public partial class SimpleAI : BoardPiece
{
	public void AIDecideTurn()
	{
		Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Players");
		if(players.Count == 1)
		{
			Vector3 direction = ((players[0] as Node3D).GlobalPosition - GlobalPosition).Normalized();
			Move(new Vector2(direction.X,direction.Z));
		}
	}
}
