using Godot;
using System;

public partial class Bullet: Area2D
{
	public Vector2 Direction = Vector2.Zero;
	public float Speed = 600f;
	
	public override void _Process(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}
	
	private void OnBodyEntered (Node body)
	{
		if (body is Enemy)
		{
			body.QueueFree();
			QueueFree();
		}
	}
}
