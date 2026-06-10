using Godot;
using System;

public partial class BossEnemy: CharacterBody2D
{
	[Export]
	public float Speed = 100f;
	
	[Export]
	public int Health = 30;

	
	public int XPReward = 10;
	private Player player;
	private Color originalColor; 
	private GpuParticles2D deathParticles;
	private PackedScene xpOrbScene;
	private AudioStreamPlayer explosionSound;
	private ProgressBar bossHealthBar;
	private bool issDead = false;
	private float chargeTimer = 0f;
	private bool isCharging = false;
	private Vector2 chargeDirection;
	private float chargeSpeed = 1000f;
	private float chargeDuration = 0.5f;
	private bool isElite = false;
	private PackedScene damageTextScene;
	private AudioStreamPlayer criticalSound;
	private Label bossDefeatedLabel;
	
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<Player>("Main/Player");
		originalColor = Modulate;
		deathParticles = GetNode<GpuParticles2D>("DeathParticles");
		xpOrbScene = GD.Load<PackedScene>("res://XPOrb.tscn");
		bossHealthBar = GetTree().Root.GetNode<ProgressBar>("Main/UI/BossHealthBar");
		bossHealthBar.Visible = true;
		bossHealthBar.MaxValue = Health;
		bossHealthBar.Value = Health;
		explosionSound = GetNode<AudioStreamPlayer>("Explosionsound");
		damageTextScene = GD.Load<PackedScene>("res://DamageText.tscn");
		bossDefeatedLabel = GetTree().Root.GetNode<Label>("Main/UI/BossDefeatedLabel");
		
		if(isElite)
		{
			bossHealthBar.Modulate = Colors.Gold;
		}
		criticalSound = GetNode<AudioStreamPlayer>("CriticalSound");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
			return;
		
		if(isCharging)
		{
			Velocity = chargeDirection * chargeSpeed;
		}
		else
		{
			chargeTimer += (float)delta;
		Vector2 direction = (player.Position - Position).Normalized();
		Velocity = direction * Speed;
		
		if (chargeTimer >= 4f)
		{
			chargeTimer= 0f;
			StartCharge();
		}
		}
		
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
			damageText.Text = "💥 CRITICAL" + damage;
			damageText.Modulate = Colors.Gold;
			criticalSound.Play();
		}
		else
		{
			damageText.Text = damage.ToString();
		}
		
		if (issDead)
		{
			return;
		}
		
		Health -= damage;
				
		GD.Print("Boss Enemy HP: " + Health);
		bossHealthBar.Value = Health;

		
		if (Health <= 0)
		{
			explosionSound.Play();
			issDead = true;
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
			bossHealthBar.Visible = false;
			EnemySpawner spawner = GetTree().Root.GetNode<EnemySpawner>("Main/EnemySpawner");
			spawner.BossKilled();
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
			ShowBossDefeatBanner();
			await ToSignal(GetTree().CreateTimer(2f), "timeout");
			bossDefeatedLabel.Visible = false;
			AchievementManager achievements = GetTree().Root.GetNode<AchievementManager>("Main/AchievementManager");
			achievements.BossKilled();
			QueueFree();
		}
		
		Modulate = Colors.Red;
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		Modulate = originalColor;
	}
	
	private async void StartCharge()
	{
		Modulate = Colors.Yellow;
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		chargeDirection = (player.Position - Position).Normalized();
		Modulate = Colors.Red;
		isCharging = true;
		await ToSignal(GetTree().CreateTimer(chargeDuration), "timeout");
		
		isCharging = false;
		Modulate = originalColor;
	}
	
	public void MakeElite()
	{
		isElite = true;
		Health *= 2;
		Speed *= 1.2f;
		XPReward *= 3;
		Scale *= 1.5f;
		Modulate = Colors.Gold;
	}
	
	private async void ShowBossDefeatBanner()
	{
		bossDefeatedLabel.Visible = true;
		bossDefeatedLabel.Scale = new Vector2(0.5f, 0.5f);
		Tween tween = CreateTween();
		tween.TweenProperty(
			bossDefeatedLabel,
			"scale",
			new Vector2 (1f, 1f),
			0.3f);
	}
}
