using Godot;
using System;

public partial class EnemySpawner: Node
{
	private Timer spawnTimer;
	private float difficultyTimer = 0f;
	private PackedScene enemyScene;
	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
		spawnTimer = GetNode<Timer>("SpawnTimer");
		spawnTimer.Timeout += SpawnEnemy;
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
		Enemy enemy = enemyScene.Instantiate<Enemy>();
		Random random = new Random();
		float x = random.Next (50,750);
		float y = random.Next (50, 550);
		
		enemy.Position = new Vector2(x,y);
		enemy.Speed = (float)GD.RandRange(100,250);
		GetTree().CurrentScene.AddChild(enemy);
	}
}
