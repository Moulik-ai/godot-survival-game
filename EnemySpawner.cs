using Godot;
using System;

public partial class EnemySpawner: Node
{
	private Timer spawnTimer;
	private float difficultyTimer = 0f;
	private PackedScene enemyScene;
	private PackedScene fastEnemyScene;
	private PackedScene tankEnemyScene;
	
	
	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
		spawnTimer = GetNode<Timer>("SpawnTimer");
		spawnTimer.Timeout += SpawnEnemy;
		fastEnemyScene = GD.Load<PackedScene>("res://FastEnemy.tscn");
		tankEnemyScene = GD.Load<PackedScene>("res://TankEnemy.tscn");
	}
	
	public override void _Process(double delta)
	{
		difficultyTimer += (float)delta;
		
		if (difficultyTimer >= 5f)
		{
			difficultyTimer = 0f;
			
			if (spawnTimer.WaitTime > 0.5f)
			{
				spawnTimer.WaitTime -= 0.1f;
				GD.Print("Spawn Rate Increased: " + spawnTimer.WaitTime);
			}
		}
	}
	
	private void SpawnEnemy()
{
	Random random = new Random();

	float x = random.Next(50, 750);
	float y = random.Next(50, 550);

	int roll = (int)GD.RandRange(0, 99);

	Node2D enemy;

	if (roll < 70)
	{
		enemy = enemyScene.Instantiate<Node2D>();
	}
	else if (roll < 90)
	{
		enemy = fastEnemyScene.Instantiate<Node2D>();
	}
	else
	{
		enemy = tankEnemyScene.Instantiate<Node2D>();
	}

	enemy.Position = new Vector2(x, y);

	GetTree().CurrentScene.AddChild(enemy);
	}
	
}
