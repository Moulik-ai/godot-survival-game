using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 300f;
	private float survivalTime = 0f;
	private Label scoreLabel;
	private PackedScene bulletScene;
	private Vector2 lastDirection = Vector2.Right;
	private bool isDead = false;
	private bool canDash = true;
	private bool isDashing = false;
	private float dashSpeed = 900f;
	private float dashDuration = 0.15f;
	private float dashCooldown = 1f;
	
	public override void _Ready()
	{
		scoreLabel = GetTree().Root.GetNode<Label>("Main/UI/ScoreLabel");
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
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
			
		if (Input.IsActionJustPressed("ui_accept"))
		{
			Shoot();
		}
		
		if (Input.IsActionJustPressed("ui_select") && canDash)
		{
			Dash();
		}

		direction = direction.Normalized();
		if (direction != Vector2.Zero)
		{
			lastDirection = direction;
		}

		if (isDashing)
		{
			Velocity = lastDirection * dashSpeed;
		}
		else
		{
			Velocity = direction * Speed;
		}

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

private void Shoot()
{
	Bullet bullet = bulletScene.Instantiate<Bullet>();
	
	bullet.Position = Position;
	bullet.Direction = lastDirection;
	GetTree().CurrentScene.AddChild(bullet);
}

private async void Dash()
{
	canDash = false;
	isDashing = true;
	
	await ToSignal(GetTree().CreateTimer(dashDuration), "timeout");
	isDashing = false;
	
	await ToSignal(GetTree().CreateTimer(dashCooldown), "timeout");
	canDash = true;
}
}
