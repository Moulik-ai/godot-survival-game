using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 300f;
	
	private bool isDead = false;
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("ui_right"))
			direction.X += 1;

		if (Input.IsActionPressed("ui_left"))
			direction.X -= 1;

		if (Input.IsActionPressed("ui_down"))
			direction.Y += 1;

		if (Input.IsActionPressed("ui_up"))
			direction.Y -= 1;

		direction = direction.Normalized();

		Velocity = direction * Speed;

		MoveAndSlide();
		
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			
			if (collision.GetCollider() is Enemy)
			{
				Die();
			}
		}
	}

private void Die()
{
	GD.Print("Touched");
	if (isDead)        
		return;
		
	isDead = true;
	GD.Print ("GAME OVER");
	GetTree().ReloadCurrentScene();
	}
}
