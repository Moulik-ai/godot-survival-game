using Godot;
using System;

public partial class DamageText : Label
{
	private float lifetime = 1f;
	
	public override void _Process(double delta)
	{
		Position += new Vector2(0, -50) * (float)delta;
		lifetime -= (float)delta;
		
		Modulate = new Color(
			1,
			1,
			1,
			lifetime);
			
			if (lifetime <= 0)
			{
				QueueFree();
			}
	}
}
