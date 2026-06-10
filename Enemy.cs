using Godot;
using System;

public partial class Enemy: CharacterBody2D
{
	[Export]
	public float Speed = 150f;
	public int XPReward = 1;
	public int Health = 1;
	private Player player;
	private Color originalColor; 
	private GpuParticles2D deathParticles;
	private PackedScene xpOrbScene;
	private AudioStreamPlayer explosionSound;
	private bool isElite = false;
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
		explosionSound.Play();
		if (Health <= 0)
		{
			CameraController camera = GetTree().Root.GetNode<CameraController>("Main/Player/Camera2D");
			camera.Shake(8f);
			deathParticles.Reparent(GetTree().CurrentScene);
			deathParticles.GlobalPosition = GlobalPosition;
			deathParticles.Emitting = true;
			Visible = false;
			AchievementManager achievements = GetTree().Root.GetNode<AchievementManager>("Main/AchievementManager");
			achievements.EnemyKilled();
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
	
	public void MakeElite()
	{
		isElite = true;
		Health *= 3;
		Speed *= 1.3f;
		XPReward *= 3;
		Scale *= 1.5f;
		Modulate = Colors.Gold;
	}
}
