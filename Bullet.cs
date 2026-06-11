using Godot;
using System;

public partial class Bullet: Area2D
{
	public Vector2 Direction = Vector2.Zero;
	public float Speed = 600f;
	public int Damage = 1;
	public bool IsCrit = false;
	
	public override void _Process(double delta)
	{
		Position += Direction * Speed * (float)delta;
		Rotation = Direction.Angle();
	}
	
	private void OnBodyEntered (Node body)
	{
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage, IsCrit);
			QueueFree();
		}
		
		else if (body is FastEnemy fastEnemy)
		{
			fastEnemy.TakeDamage(Damage, IsCrit);
			QueueFree();
		}
		
		else if (body is TankEnemy tankEnemy)
		{
			tankEnemy.TakeDamage(Damage, IsCrit);
			QueueFree();
		}
		
		else if (body is BossEnemy bossEnemy)
		{
			bossEnemy.TakeDamage(Damage, IsCrit);
			QueueFree();
		}
		
		else if (body is ExploderEnemy exploderEnemy)
		{
			exploderEnemy.TakeDamage(Damage, IsCrit);
			QueueFree();
		}
		
		else if (body is RangedEnemy rangedEnemy)
		{
			rangedEnemy.TakeDamage(Damage, IsCrit);
			QueueFree();
		}
	}
}
