using Godot;
using System;

public partial class UIButton : Node
{
	[Export] UIButton subMenu;

	public void GetCursor()
	{
		(this.GetParent() as Control).Visible = true;
	}
	public void GiveCursor()
	{
		(this.GetParent() as Control).Visible = false;
	}
}
