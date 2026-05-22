using Godot;
using System;

public partial class Enemy: CharacterBody2D
{
	[Export]
	public float Speed = 150f;
	public int Health = 3;
	private Player player;
	
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Player>("Main/Player");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
			return;
		Vector2 direction = (player.Position - Position).Normalized();
		Velocity = direction * Speed;
		
		MoveAndSlide();
	}
	
	public void TakeDamage(int damage)
	{
		Health -= damage;
		Modulate = Colors.Red;
		GD.Print("Enemy HP: " + Health);
		
		if (Health <= 0)
		{
			QueueFree();
		}
	}
}
