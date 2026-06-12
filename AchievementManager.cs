using Godot;
using System.Collections.Generic;

public partial class AchievementManager: Node
{
	private Label achievementLabel;
	private HashSet <string> unlocked = new HashSet<string>();
	private int enemiesKilled = 0;
	public int critHits = 0;
	
	
	public override void _Ready()
	{
		achievementLabel = GetTree().Root.GetNode<Label>("Main/UI/AchievementLabel");
	}

public async void UnlockAchievement(string title)
{
	if (unlocked.Contains(title))
	{
		return;
	}
	unlocked.Add(title);
	achievementLabel.Text = "🏆 " + title;
	achievementLabel.Visible = true;
	
	achievementLabel.Position = new Vector2(achievementLabel.Position.X, -100);
	Tween slideIn = CreateTween();
	
	slideIn.TweenProperty(
		achievementLabel,
		"position:y",
		50,
		0.4f);
	await ToSignal(GetTree().CreateTimer(3f), "timeout");
	
	Tween slideOut = CreateTween();
	
	slideOut.TweenProperty(
		achievementLabel,
		"position:y",
		-100,
		0.4f);
		
		await ToSignal(slideOut, "finished");
	achievementLabel.Visible = false;
}

public void EnemyKilled()
{
	enemiesKilled++;
	
	if (enemiesKilled == 1)
	{
		UnlockAchievement("First Blood");
	}
	
	if (enemiesKilled == 100)
	{
		UnlockAchievement("Centurion");
	}
	
	if (enemiesKilled == 200)
	{
		UnlockAchievement("Double Centurion");
	}
	
	if (enemiesKilled == 300)
	{
		UnlockAchievement("Triple Centurion");
	}
}

public void BossKilled()
{
	UnlockAchievement("BOSS SLAYER");
}

public void CheckWave(int Wave)
{
	if (Wave >= 5)
	{
		UnlockAchievement("Wave Rider");
	}
	
	if (Wave >= 10)
	{
		UnlockAchievement("Survivor");
	}
	
	if (Wave >= 20)
	{
		UnlockAchievement("Legend");
	}
}

public void RegisterCrit()
{
	critHits++;
	
	if (critHits >= 10)
	{
		UnlockAchievement("Lucky Shot");
	}
}
}
