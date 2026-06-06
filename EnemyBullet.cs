using Godot;
using System;

public partial class EnemyBullet: Area2D
{
	public Vector2 Direction;
	public float Speed = 300f;
	
	public override void _Process(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.TakeDamage();
			QueueFree();
		}
	}
}
