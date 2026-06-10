using Godot;
using System;

public partial class ExploderEnemy: CharacterBody2D
{
	[Export]
	public float Speed = 250f;
	
	[Export]
	public int XPReward = 2;
	
	[Export]
	public int Health = 1;
	private Player player;
	private Color originalColor; 
	private GpuParticles2D deathParticles;
	private PackedScene xpOrbScene;
	private AudioStreamPlayer explosionSound;
	private bool hasExploded = false;
	private PackedScene damageTextScene;
	private AudioStreamPlayer criticalSound;
	
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Player>("Main/Player");
		originalColor = Modulate;
		deathParticles = GetNode<GpuParticles2D>("DeathParticles");
		xpOrbScene = GD.Load<PackedScene>("res://XPOrb.tscn");
		explosionSound = GetNode<AudioStreamPlayer>("Explosionsound");
		damageTextScene = GD.Load<PackedScene>("res://DamageText.tscn");
		criticalSound = GetNode<AudioStreamPlayer>("CriticalSound");
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
	
	public async void TakeDamage(int damage, bool isCrit = false)
	{
		
		DamageText damageText = damageTextScene.Instantiate<DamageText>();
		damageText.Text = damage.ToString();
		damageText.Position = GlobalPosition + new Vector2 (0, -40);
		GetTree().CurrentScene.AddChild(damageText);
		
		if (isCrit)
		{
			AchievementManager achievements = GetTree().Root.GetNode<AchievementManager>("Main/AchievementManager");
			achievements.RegisterCrit();
			criticalSound.Play();
			damageText.Text = "💥" + damage;
			damageText.Modulate = Colors.Gold;
		}
		else
		{
			damageText.Text = damage.ToString();
		}
		
		Health -= damage;
		Modulate = Colors.Red;
		
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		Modulate = originalColor;
		
		GD.Print("Enemy HP: " + Health);

		if (Health <= 0)
		{
			explosionSound.Play();
			CameraController camera = GetTree().Root.GetNode<CameraController>("Main/Player/Camera2D");
			camera.Shake(8f);
			deathParticles.Reparent(GetTree().CurrentScene);
			deathParticles.GlobalPosition = GlobalPosition;
			deathParticles.Emitting = true;
			AchievementManager achievements = GetTree().Root.GetNode<AchievementManager>("Main/AchievementManager");
			achievements.EnemyKilled();
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
