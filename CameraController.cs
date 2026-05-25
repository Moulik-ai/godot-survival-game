using Godot;
using System;

public partial class CameraController : Camera2D
{
	private float shakeStrength = 1f;

	public override void _Ready()
	{
		MakeCurrent();

		PositionSmoothingEnabled = false;
	}

	public override void _Process(double delta)
	{
		if (shakeStrength > 0)
		{
			Offset = new Vector2(
				(float)GD.RandRange(-shakeStrength, shakeStrength),
				(float)GD.RandRange(-shakeStrength, shakeStrength)
			);

			shakeStrength = Mathf.Lerp(shakeStrength, 0, 10f * (float)delta);
		}
		else
		{
			Offset = Vector2.Zero;
		}
	}

	public void Shake(float strength)
	{
		shakeStrength = strength;
	}
}
