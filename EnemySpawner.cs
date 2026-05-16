using Godot;
using System;

public partial class EnemySpawner: Node
{
	private PackedScene enemyScene;
	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
		Timer timer = GetNode<Timer>("SpawnTimer");
		timer.Timeout += SpawnEnemy;
	}
	
	private void SpawnEnemy()
	{
		Enemy enemy = enemyScene.Instantiate<Enemy>();
		Random random = new Random();
		float x = random.Next (50,750);
		float y = random.Next (50, 550);
		
		enemy.Position = new Vector2(x,y);
		GetTree().CurrentScene.AddChild(enemy);
	}
}
