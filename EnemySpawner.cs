using Godot;
using System;

public partial class EnemySpawner: Node
{
	private Timer spawnTimer;
	private float difficultyTimer = 0f;
	private float bossTimer = 0f;
	private PackedScene bossScene;
	private PackedScene enemyScene;
	private PackedScene fastEnemyScene;
	private PackedScene tankEnemyScene;
	private PackedScene exploderEnemyScene;
	private int currentWave = 1;
	private int enemiesToSpawn = 0;
	private int enemiesAlive = 0;
	private Label waveLabel;
	private bool bossAlive = false;
	private Player player;
	private PackedScene rangedEnemyScene;
	
	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
		spawnTimer = GetNode<Timer>("SpawnTimer");
		spawnTimer.Timeout += SpawnEnemy;
		fastEnemyScene = GD.Load<PackedScene>("res://FastEnemy.tscn");
		tankEnemyScene = GD.Load<PackedScene>("res://TankEnemy.tscn");
		exploderEnemyScene = GD.Load<PackedScene>("res://ExploderEnemy.tscn");
		bossScene = GD.Load<PackedScene>("res://BossEnemy.tscn");
		rangedEnemyScene = GD.Load<PackedScene>("res://RangedEnemy.tscn");
		waveLabel = GetTree().Root.GetNode<Label>("Main/UI/WaveLabel");
		player = GetTree().Root.GetNode<Player>("Main/Player");
		StartWave();
	}
	
	public override void _Process(double delta)
	{
		difficultyTimer += (float)delta;
		bossTimer += (float)delta;
		
		if (difficultyTimer >= 5f)
		{
			difficultyTimer = 0f;
			
			if (spawnTimer.WaitTime > 0.5f)
			{
				spawnTimer.WaitTime -= 0.1f;
				GD.Print("Spawn Rate Increased: " + spawnTimer.WaitTime);
			}
		}
		
		/*if (bossTimer >= 7f)
		{
			bossTimer = 0f;
			SpawnBoss();
		}*/
	}
	
	private void SpawnEnemy()
{
	
	if(enemiesToSpawn <= 0)
	{
		spawnTimer.Stop();
		return;
	}
	Random random = new Random();
	
	Vector2 spawnPosition;
	do
	{
		float x = random.Next(50, 750);
		float y = random.Next(50, 550);
		spawnPosition = new Vector2 (x, y);
	}
	while (
		spawnPosition.DistanceTo(player.Position) < 150
	);
	
	int roll = (int)GD.RandRange(0, 99);

	Node2D enemy;

	if (roll < 50)
	{
		enemy = enemyScene.Instantiate<Node2D>();
	}
	else if (roll < 75)
	{
		enemy = fastEnemyScene.Instantiate<Node2D>();
	}
	else if (roll < 85)
	{
		enemy = tankEnemyScene.Instantiate<Node2D>();
	}
	else if (roll < 95)
	{
		enemy = rangedEnemyScene.Instantiate<Node2D>();
	}
	else
	{
		enemy = exploderEnemyScene.Instantiate<Node2D>();
	}

	enemy.Position = spawnPosition;
	enemiesToSpawn--;
	GetTree().CurrentScene.AddChild(enemy);
	}
	
	private void SpawnBoss()
	{
		bossAlive = true;
		Random random = new Random();
		
		float x = random.Next(50,750);
		float y = random.Next(50,550);
		
		Node2D boss=   
			bossScene.Instantiate<Node2D>();
		
		boss.Position = new Vector2(x,y);
		GetTree().CurrentScene.AddChild(boss);
		GD.Print("BOSS SPAWNED!");
	}
	
	private void StartWave()
	{
		waveLabel.Text = "WAVE " + currentWave;
		enemiesToSpawn = currentWave * 5;
		enemiesAlive = enemiesToSpawn;
		spawnTimer.Start();
		GD.Print("Starting Wave" + currentWave);
	}
	
	public void EnemyKilled()
	{
		enemiesAlive--;
		GD.Print("Enemies Remaining: " + enemiesAlive);
		
		if (!bossAlive && enemiesAlive <= 0)
		{
			WaveComplete();
		}
	}
	
	private void WaveComplete()
	{
		GD.Print("Wave Complete!");
		currentWave++;
		
		if (currentWave % 3 == 0)
		{
			spawnTimer.Stop();
			SpawnBoss();
		}
		else
		{
			StartWave();
		}
	}
	
	public void BossKilled()
	{
		bossAlive = false;
		currentWave++;
		StartWave();
	}
	
	public int GetCurrentWave()
	{
		return currentWave;
	}
}
