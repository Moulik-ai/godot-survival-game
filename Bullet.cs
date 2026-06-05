using Godot;
using System;

public partial class Bullet: Area2D
{
	public Vector2 Direction = Vector2.Zero;
	public float Speed = 600f;
	public int Damage = 1;
	
	public override void _Process(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}
	
	private void OnBodyEntered (Node body)
	{
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			QueueFree();
		}
		
		else if (body is FastEnemy fastEnemy)
		{
			fastEnemy.TakeDamage(Damage);
			QueueFree();
		}
		
		else if (body is TankEnemy tankEnemy)
		{
			tankEnemy.TakeDamage(Damage);
			QueueFree();
		}
		
		else if (body is BossEnemy bossEnemy)
		{
			bossEnemy.TakeDamage(Damage);
			QueueFree();
		}
		
		else if (body is ExploderEnemy exploderEnemy)
		{
			exploderEnemy.TakeDamage(Damage);
			QueueFree();
		}
	}
}
