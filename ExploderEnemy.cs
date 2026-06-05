using Godot;
using System;

public partial class ExploderEnemy: CharacterBody2D
{
	[Export]
	public float Speed = 250f;
	public int XPReward = 2;
	public int Health = 1;
	private Player player;
	private Color originalColor; 
	private GpuParticles2D deathParticles;
	private PackedScene xpOrbScene;
	private AudioStreamPlayer explosionSound;
	private bool hasExploded = false;
	
	
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Player>("Main/Player");
		originalColor = Modulate;
		deathParticles = GetNode<GpuParticles2D>("DeathParticles");
		xpOrbScene = GD.Load<PackedScene>("res://XPOrb.tscn");
		explosionSound = GetNode<AudioStreamPlayer>("Explosionsound");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Position.DistanceTo(player.Position) < 50 && !hasExploded)
		{
			Explode();
		}
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
		explosionSound.Play();
		if (Health <= 0)
		{
			CameraController camera = GetTree().Root.GetNode<CameraController>("Main/Player/Camera2D");
			camera.Shake(8f);
			deathParticles.Reparent(GetTree().CurrentScene);
			deathParticles.GlobalPosition = GlobalPosition;
			deathParticles.Emitting = true;
			Visible = false;
			await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
			XPOrb orb = xpOrbScene.Instantiate<XPOrb>();
			orb.Position = Position;
			orb.XPValue = XPReward;
			GetTree().CurrentScene.AddChild(orb);
			EnemySpawner spawner = GetTree().Root.GetNode<EnemySpawner>("Main/EnemySpawner");
			spawner.EnemyKilled();
			QueueFree();
		}
	}
	
	private async void Explode()
	{
		explosionSound.Play();
		GD.Print("BOOM!");
		player.TakeDamage();
		
		CameraController camera = GetTree().Root.GetNode<CameraController>("Main/Player/Camera2D");
		camera.Shake(12f);
		
		deathParticles.Reparent(GetTree().CurrentScene);
		deathParticles.GlobalPosition = GlobalPosition;
		
		deathParticles.Emitting = true;
		Visible = false;
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		
		XPOrb orb = xpOrbScene.Instantiate<XPOrb>();
		orb.Position = Position;
		orb.XPValue = XPReward;
		
		GetTree().CurrentScene.AddChild(orb);
		EnemySpawner spawner = GetTree().Root.GetNode<EnemySpawner>("Main/EnemySpawner");
		spawner.EnemyKilled();
		QueueFree();
	}
}
