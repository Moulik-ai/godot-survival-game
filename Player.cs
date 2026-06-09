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
	private int maxHealth = 3;
	private int currentHealth = 3;
	private bool isInvincible = false;
	private Label healthLabel;
	private Panel gameOverPanel;
	private Label statsLabel;
	private Button retryButton;
	private float edgeDamageTimer = 0f;
	private float edgeDamageInterval = 1f;
	private float critChance = 10f;
	private float critMultiplier = 2f;
	private Button healthButton;
	private Button critChanceButton;
	private Button critDamageButton;
	private Button bulletDamageButton;
	private int bulletDamage = 1;
	private int bestWave = 0;
	private int bestTime = 0;
	private PackedScene damageTextScene;
	private Panel pausePanel;
	private Button resumeButton;
	private Button restartButton;
	private Button settingsPauseButton;
	private Button quitPauseButton;
	private Button backButton;
	private Panel settingsPanel;
	
	private bool isPaused = false;
	
	
	public override void _Ready()
	{
		scoreLabel = GetTree().Root.GetNode<Label>("Main/UI/ScoreLabel");
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		levelUpPanel = GetTree().Root.GetNode<Panel>("Main/UI/LevelUpPanel");
		attackSpeedButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/AttackSpeedButton");
		moveSpeedButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/MoveSpeedButton");
		dashButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/DashButton");
		multiShotButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/MultiShotButton");
		attackSpeedButton.Pressed += UpgradeAttackSpeed;
		moveSpeedButton.Pressed += UpgradeMoveSpeed;
		dashButton.Pressed +=UpgradeDash;
		multiShotButton.Pressed += UpgradeMultiShot;
		shootSound = GetNode<AudioStreamPlayer>("Shootsound");
		coinSound = GetNode<AudioStreamPlayer>("Coinsound");
		dieSound = GetNode<AudioStreamPlayer>("DieSound");
		healthLabel = GetTree().Root.GetNode<Label>("Main/UI/HealthLabel");
		gameOverPanel = GetTree().Root.GetNode<Panel>("Main/UI/GameOverPanel");
		statsLabel = GetTree().Root.GetNode<Label>("Main/UI/GameOverPanel/CenterContainer/VBoxContainer/StatsLabel");
		retryButton = GetTree().Root.GetNode<Button>("Main/UI/GameOverPanel/CenterContainer/VBoxContainer/RetryButton");
		retryButton.Pressed += RetryGame;
		healthButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/HealthButton");
		critChanceButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/CritChanceButton");
		critDamageButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/CritDamageButton");
		bulletDamageButton = GetTree().Root.GetNode<Button>("Main/UI/LevelUpPanel/CenterContainer/VBoxContainer/BulletDamageButton");
		healthButton.Pressed += UpgradeHealth;
		critChanceButton.Pressed += UpgradeCritChance;
		critDamageButton.Pressed += UpgradeCritDamage;
		bulletDamageButton.Pressed += UpgradeBulletDamage;
		LoadHighScore();
		damageTextScene = GD.Load<PackedScene>("res://DamageText.tscn");
		pausePanel = GetTree().Root.GetNode<Panel>("Main/UI/PausePanel");
		resumeButton = GetTree().Root.GetNode<Button>("Main/UI/PausePanel/CenterContainer/VBoxContainer/ResumeButton");
		restartButton = GetTree().Root.GetNode<Button>("Main/UI/PausePanel/CenterContainer/VBoxContainer/RestartButton");
		settingsPauseButton = GetTree().Root.GetNode<Button>("Main/UI/PausePanel/CenterContainer/VBoxContainer/SettingsButton");
		quitPauseButton = GetTree().Root.GetNode<Button>("Main/UI/PausePanel/CenterContainer/VBoxContainer/QuitButton");
		backButton = GetTree().Root.GetNode<Button>("Main/UI/SettingsPanel/CenterContainer/VBoxContainer/BackButton");
		settingsPanel = GetTree().Root.GetNode<Panel>("Main/UI/SettingsPanel");
		resumeButton.Pressed += ResumePauseGame;
		restartButton.Pressed += RestartPauseGame;
		quitPauseButton.Pressed += QuitToMenu;
		settingsPauseButton.Pressed += OpenSettings;
		backButton.Pressed += CloseSettings;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		survivalTime += (float)delta;
		scoreLabel.Text = "Score: " + ((int)survivalTime).ToString();
		CheckEdgeDamage ((float)delta);
		
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
			
		if (Input.IsActionJustPressed("ui_space") && canShoot)
		{
			Shoot();
		}
		
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			TogglePause();
		}
		
		if (Input.IsActionJustPressed("ui_shift") && canDash)
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
			
			if (collision.GetCollider() is Node2D enemyNode)
			{
				
				if (enemyNode is Enemy || enemyNode is FastEnemy || enemyNode is TankEnemy || enemyNode is BossEnemy)
				{
				TakeDamage();

				}
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
	gameOverPanel.Visible = true;
	EnemySpawner spawner = GetTree().Root.GetNode<EnemySpawner>("Main/EnemySpawner");
	int WaveReached = spawner.GetCurrentWave();
	
	if (WaveReached > bestWave)
	{
		bestWave = WaveReached;
	}
	
	if((int)survivalTime > bestTime)
	{
		bestTime = (int)survivalTime;
	}
	SaveHighScore();
	statsLabel.Text = "Wave Reached : " + WaveReached + "\nTime Survived : " + ((int)survivalTime) + "\n\n Best Wave: " + bestWave + "\n Best Time: " + bestTime;
	GetTree().Paused = true;
	}

private async void Shoot()
{
	
	for (int i = 0; i < bulletCount; i++){
		Bullet bullet = bulletScene.Instantiate<Bullet>();
		bool isCrit = GD.Randf() < (critChance / 100f);
		
		if (isCrit)
		{
			bullet.IsCrit = true;
			bullet.Damage = (int)(bulletDamage*critMultiplier);
			GD.Print("💥 CRITICAL HIT!");
		}
		else
		{
			bullet.Damage = bulletDamage;
		}
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

private void UpdateHealthUI()
{
	string hearts = "";
	for (int i = 0; i < currentHealth; i++)
	{
		hearts += "❤️";
	}
	healthLabel.Text = hearts;
}

public async void TakeDamage()
{
	
	if (isInvincible)
	{
		return;
	}
	
	isInvincible = true;
	currentHealth--;
	UpdateHealthUI();
	GD.Print("Health: " +currentHealth);
	
	if (currentHealth <= 0)
	{
		dieSound.Play();
		Die();
		return;
	}
	Modulate = Colors.Red;
	
	await ToSignal(GetTree().CreateTimer(1f), "timeout");
	Modulate = Colors.Cyan;
	isInvincible = false;
	
}

private void RetryGame(){
	GetTree().Paused = false;
	GetTree().ReloadCurrentScene();
}

private void CheckEdgeDamage(float delta)
{
	bool touchingEdge = Position.X <= 20 || Position.X >= 780 || Position.Y <= 20 || Position.Y >= 580;
	
	if (touchingEdge)
	{
		edgeDamageTimer += delta;
		
		if (edgeDamageTimer >= edgeDamageInterval)
		{
			edgeDamageTimer = 0f;
			TakeDamage();
			GD.Print("EDGE DAMAGE!");
		}
	}
	
	else
	{
		edgeDamageTimer = 0f;
	}
	}
	
	private void UpgradeHealth()
	{
		maxHealth++;
		currentHealth++;
		
		UpdateHealthUI();
		GD.Print("MAX HEALTH: " + maxHealth);
		ResumeGame();
	}
	
	private void UpgradeCritChance()
	{
		critChance += 5f;
		GD.Print("CRIT CHANCE: " + critChance);
		
		ResumeGame();
	}
	
	private void UpgradeCritDamage()
	{
		critMultiplier += 0.5f;
		GD.Print("CRIT DAMAGE: " + critMultiplier);
		ResumeGame();
	}
	
	private void UpgradeBulletDamage()
	{
		bulletDamage++;
		GD.Print("Bullet Damage: " + bulletDamage);
		ResumeGame();
	}
	
	private void SaveHighScore()
	{
		using var file = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Write);
		file.StoreLine(bestWave.ToString());
		file.StoreLine(bestTime.ToString());
	}
	
	private void LoadHighScore()
	{
		if(!FileAccess.FileExists("user://highscore.save"))
		{
			return;
		}
		
		using var file = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Read);
		bestWave = int.Parse(file.GetLine());
		bestTime = int.Parse(file.GetLine());
	}
	
	private void TogglePause()
	{
		isPaused = !isPaused;
		pausePanel.Visible = isPaused;
		GetTree().Paused = isPaused;
	}
	
	private void ResumePauseGame()
	{
		isPaused = false;
		pausePanel.Visible = false;
		GetTree().Paused = false;
	}
	
	private void RestartPauseGame()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
	
	private void QuitToMenu()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
	
	private void OpenSettings()
	{
		pausePanel.Visible = false;
		settingsPanel.Visible = true;
	}
	
	private void CloseSettings()
	{
		pausePanel.Visible = true;
		settingsPanel.Visible = false;
	}
}
