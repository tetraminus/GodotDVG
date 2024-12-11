using Godot;
using System;
using VectorRendering.osciObjects;

[GlobalClass]
[Tool]
public partial class OsciCircle : OsciObject
{
	[Export]
	private CircleShape2D _circleShape = new();
	
	const int resolution = 32;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		for (int i = 0; i < resolution + 1; i++)
		{
			float angle = Mathf.Pi * 2 / resolution * i;
			Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _circleShape.Radius;

			point += GlobalPosition;
			OsciManager.Ins.AddPoint(point);
		}
	}
}
