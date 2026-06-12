using Godot;
using System;

public partial class MainMenu : Control
{
	private Button playButton;
	private Button settingsButton;
	private Button quitButton;
	private Panel settingsPanel;
	private HSlider musicSlider;
	private HSlider sfxSlider;
	private Button backButton;
	private Panel creditsPanel;
	private Button creditsButton;
	private Button backsButton;
	
	public override void _Ready()
	{
		playButton = GetNode<Button>("CenterContainer/VBoxContainer/PlayButton");
		settingsButton = GetNode<Button>("CenterContainer/VBoxContainer/SettingsButton");
		quitButton = GetNode<Button>("CenterContainer/VBoxContainer/QuitButton");
		settingsPanel = GetNode<Panel>("SettingsPanel");
		musicSlider = GetNode<HSlider>("SettingsPanel/CenterContainer/VBoxContainer/MusicSlider");
		sfxSlider = GetNode<HSlider>("SettingsPanel/CenterContainer/VBoxContainer/SFXSlider");
		backButton = GetNode<Button>("SettingsPanel/CenterContainer/VBoxContainer/BackButton");
		creditsPanel = GetNode<Panel>("CreditsPanel");
		creditsButton = GetNode<Button>("CenterContainer/VBoxContainer/CreditsButton");
		backsButton = GetNode<Button>("CreditsPanel/TextureRect/BacksButton");
		
		playButton.Pressed += PlayGame;
		settingsButton.Pressed += OpenSettings;
		quitButton.Pressed += QuitGame;
		backButton.Pressed += CloseSettings;
		musicSlider.ValueChanged += UpdateMusicVolume;
		sfxSlider.ValueChanged += UpdateSFXVolume;
		creditsButton.Pressed += ShowCredits;
		backsButton.Pressed += HideCredits;
	}
	
	private void PlayGame()
	{
		GetTree().ChangeSceneToFile("res://Main.tscn");
	}
	
	private void OpenSettings()
	{
		settingsPanel.Visible = true;
	}
	
	private void CloseSettings()
	{
		settingsPanel.Visible = false;
	}
	
	private void QuitGame()
	{
		GetTree().Quit();
	}
	
	private void UpdateMusicVolume (double Value)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb((float)Value / 100f));
	}
	
	private void UpdateSFXVolume(double Value)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb((float) Value/100f));
	}
	
	private void ShowCredits()
	{
		creditsPanel.Visible = true;
	}
	
	private void HideCredits()
	{
		creditsPanel.Visible = false;
	}
}
