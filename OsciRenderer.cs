using Godot;
using Godot.Collections;

namespace VectorRendering;

[GlobalClass]
[Tool]
public partial class OsciRenderer : Node2D
{
    [Export]
    private Mesh mesh = new QuadMesh();
    private MultiMesh multiMesh;
    private Label fpsLabel;
    private ShaderMaterial shaderMaterial;
    [Export]
    private Texture2D texture;
    
    public OsciRenderer()
    {
        ProcessPriority = int.MaxValue;
        ZIndex = 999;
        
        shaderMaterial = Material as ShaderMaterial;
        multiMesh = new MultiMesh();
        
        
        
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        QueueRedraw();
        RenderingServer.CanvasItemSetCustomRect(GetCanvasItem(), true, new Rect2(-Vector2.Inf,Vector2.Inf));
        
        if (fpsLabel != null)
        {
            fpsLabel.Text = "FPS: " + Engine.GetFramesPerSecond();
        }
        else
        {
            
            fpsLabel = GetNode("Label") as Label;
        }
    }


    public override void _Draw()
    {
        if (OsciManager.Ins == null) return;
        Vector2[] points = OsciManager.Ins.GetPoints();
        if (points.Length < 2) return;
        multiMesh.InstanceCount = 0;
        multiMesh.UseCustomData = true;
        multiMesh.UseColors = true;
        multiMesh.CustomAabb = new Aabb(new Vector3(0, 0, 0), new Vector3(1000, 1000, 0));
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform2D;
        multiMesh.Mesh = mesh;
        multiMesh.InstanceCount = points.Length - 1;
     
        
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 p1 = points[i];
            Vector2 p2 = points[i + 1];
          
            multiMesh.SetInstanceCustomData(i, new Color(p1.X, p1.Y, p2.X, p2.Y));
            multiMesh.SetInstanceTransform2D(i, Transform2D.Identity);
            multiMesh.SetInstanceColor(i, new Color(1.60f,1.6f,0.2f));
        }
        
        DrawMultimesh(multiMesh, null);
        
        
    }
}