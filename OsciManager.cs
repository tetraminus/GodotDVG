using System.Linq;
using Godot;
using Godot.Collections;


[Tool]
public partial class OsciManager : Node
{
	public static OsciManager Ins { get; private set; }
	
	private Array<Vector2> _points = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Ins = this;
		ProcessPriority = -1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Ins == null) Ins = this;
		_points.Clear();
	}
	
	public void AddPoint(Vector2 point)
	{
		_points.Add(point);
	}

	public Vector2[] GetPoints()
	{
		return (Vector2[]) _points.ToArray();
	}
}
