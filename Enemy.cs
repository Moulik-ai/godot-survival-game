using Godot;
using System;

public partial class Enemy: CharacterBody2D
{
	[Export]
	public float Speed = 150f;
	public int Health = 1;
	private Player player;
	private Color originalColor; 
	private GpuParticles2D deathParticles;
	
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Player>("Main/Player");
		originalColor = Modulate;
		deathParticles = GetNode<GpuParticles2D>("DeathParticles");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
			return;
		Vector2 direction = (player.Position - Position).Normalized();
		Velocity = direction * Speed;
		
		MoveAndSlide();
	}
	
	public async void TakeDamage(int damage)
	{
		Health -= damage;
		Modulate = Colors.Red;
		
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		Modulate = originalColor;
		
		GD.Print("Enemy HP: " + Health);
		
		if (Health <= 0)
		{
			deathParticles.Reparent(GetTree().CurrentScene);
			deathParticles.GlobalPosition = GlobalPosition;
			deathParticles.Emitting = true;
			Visible = false;
			await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
			QueueFree();
		}
	}
}
