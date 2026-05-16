using Godot;
using System;

public partial class Node2d: Godot.Node2D
{
	public override void _Process (double delta)
	{
		Vector2 movement = Vector2.Zero;
		
		if (Input.IsActionPressed("ui_right"))
			movement.X += 1;
			
		if (Input.IsActionPressed("ui_left"))
			movement.X -= 1;
		
		if (Input.IsActionPressed("ui_down"))
			movement.Y += 1;
			
		if (Input.IsActionPressed("ui_up"))
			movement.Y -= 1;
			
		Position += movement * 200 * (float) delta;
	}
}
