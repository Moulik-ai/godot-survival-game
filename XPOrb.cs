using Godot;
using System;
public partial class XPOrb : Area2D
{
	public int XPValue = 1;
	
	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.GainXP(XPValue);
			QueueFree();
		}
	}
}
