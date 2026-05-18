using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 300f;
	private float survivalTime = 0f;
	private Label scoreLabel;
	
	private bool isDead = false;
	
	public override void _Ready()
	{
		scoreLabel = GetTree().Root.GetNode<Label>("Main/UI/ScoreLabel");
	}
	public override void _PhysicsProcess(double delta)
	{
		survivalTime += (float)delta;
		scoreLabel.Text = "Score: " + ((int)survivalTime).ToString();
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
