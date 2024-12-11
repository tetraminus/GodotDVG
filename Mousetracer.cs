using Godot;
using System;
using Godot.Collections;

public partial class Mousetracer : Node2D
{
    
    Array<Vector2> _points = new();
    
    public override void _Process(double delta)
    {
        _points.Add(GetGlobalMousePosition());
        
        if (_points.Count > 100)
        {
            _points.RemoveAt(0);
        }
        
        OsciManager.Ins.AddPoints(_points);
    }
}
