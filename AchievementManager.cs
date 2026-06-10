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
	await ToSignal(GetTree().CreateTimer(3f), "timeout");
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
