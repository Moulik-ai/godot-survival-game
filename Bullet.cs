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
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(1);
			QueueFree();
		}
		
		else if (body is FastEnemy fastEnemy)
		{
			fastEnemy.TakeDamage(1);
			QueueFree();
		}
		
		else if (body is TankEnemy tankEnemy)
		{
			tankEnemy.TakeDamage(1);
			QueueFree();
		}
		
		else if (body is BossEnemy bossEnemy)
		{
			bossEnemy.TakeDamage(1);
			QueueFree();
		}
	}
}
