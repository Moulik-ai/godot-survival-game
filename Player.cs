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
	private bool canShoot = true;
	private float ShootCooldown = 0.4f;
	private float upgradeTimer = 0f;
	private int currentXP = 0;
	private int currentLevel = 1;
	private int xpToNextLevel = 1;
	private Panel levelUpPanel;
	private Button attackSpeedButton;
	private Button moveSpeedButton;
	private Button dashButton;
	private int bulletCount = 1;
	private Button multiShotButton;
	private AudioStreamPlayer shootSound;
	private AudioStreamPlayer coinSound;
	private AudioStreamPlayer dieSound;
	
	public override void _Ready()
	{
		scoreLabel = GetTree().Root.GetNode<Label>("Main/UI/ScoreLabel");
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		levelUpPanel = GetTree().Root.GetNode<Panel>("Main/UI/LevelUpPanel");
		attackSpeedButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/VBoxContainer/AttackSpeedButton");
		moveSpeedButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/VBoxContainer/MoveSpeedButton");
		dashButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/VBoxContainer/DashButton");
		multiShotButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/VBoxContainer/MultiShotButton");
		attackSpeedButton.Pressed += UpgradeAttackSpeed;
		moveSpeedButton.Pressed += UpgradeMoveSpeed;
		dashButton.Pressed +=UpgradeDash;
		multiShotButton.Pressed += UpgradeMultiShot;
		shootSound = GetNode<AudioStreamPlayer>("Shootsound");
		coinSound = GetNode<AudioStreamPlayer>("Coinsound");
		dieSound = GetNode<AudioStreamPlayer>("Diesound");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		survivalTime += (float)delta;
		scoreLabel.Text = "Score: " + ((int)survivalTime).ToString();
		
		upgradeTimer += (float)delta;
		if(upgradeTimer >= 3f)
		{
			upgradeTimer = 0f;
			
			if(ShootCooldown > 0.1f)
			{
				ShootCooldown -= 0.05f;
				GD.Print ("Fire Rate Increased!");
				
			}
		}
		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("ui_right"))
			direction.X += 1;

		if (Input.IsActionPressed("ui_left"))
			direction.X -= 1;

		if (Input.IsActionPressed("ui_down"))
			direction.Y += 1;

		if (Input.IsActionPressed("ui_up"))
			direction.Y -= 1;
			
		if (Input.IsActionJustPressed("ui_accept") && canShoot)
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
			
			if (collision.GetCollider() is Enemy || collision.GetCollider() is FastEnemy || collision.GetCollider() is TankEnemy || collision.GetCollider() is BossEnemy)
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

private async void Shoot()
{
	
	for (int i = 0; i < bulletCount; i++){
		Bullet bullet = bulletScene.Instantiate<Bullet>();
		bullet.Position = Position;
		float spreadAngle = Mathf.DegToRad((i - (bulletCount-1)/2.0f)*15);
		bullet.Direction = lastDirection.Rotated(spreadAngle);
		GetTree().CurrentScene.AddChild(bullet);
	}
	shootSound.Play();
	canShoot = false;
	await ToSignal (GetTree().CreateTimer(ShootCooldown), "timeout");
	canShoot = true;
}

public void GainXP(int amount)
{
	
	currentXP += amount;
	GD.Print("XP: " + currentXP);
	coinSound.Play();
	if(currentXP >= xpToNextLevel)
	{
		LevelUp();
	}
}

private void LevelUp()
{
	currentXP = 0;
	currentLevel++;
	xpToNextLevel += 1;
	GD.Print("LEVEL UP! Level: " + currentLevel);
	levelUpPanel.Visible = true;
	GetTree().Paused = true;
}

	private void UpgradeAttackSpeed()
	{
			ShootCooldown *= 0.8f;
			ResumeGame();
	}
	
	private void UpgradeMoveSpeed()
	{
			Speed += 50f;
			ResumeGame();
	}
	
	private void UpgradeDash()
	{
			dashCooldown *= 0.8f;
			ResumeGame();
	}
	
	private void UpgradeMultiShot()
	{
		bulletCount++;
		ResumeGame();
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

private void ResumeGame()
{
	levelUpPanel.Visible = false;
	GetTree().Paused = false;
}
}
