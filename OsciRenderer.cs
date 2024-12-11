using Godot;
using Godot.Collections;

namespace VectorRendering;

[GlobalClass]
[Tool]
public partial class OsciRenderer : Node2D
{
    
    private ArrayMesh mesh;
    private Label fpsLabel;
    [Export]
    private Texture2D texture;
    
    public OsciRenderer()
    {
        mesh = new ArrayMesh();
        
        
        
        ProcessPriority = int.MaxValue;
        ZIndex = 999;
        
        RenderingServer.CanvasItemSetCustomRect(GetCanvasItem(), true, new Rect2(0, 0, 1000, 1000));
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        QueueRedraw();
        
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
        
        //mesh.Clear();
        
        mesh.ClearSurfaces();
        
        var pointarr = new Vector2[6];
        var uvarr = new Vector2[6];
            
        uvarr[0] = new Vector2(0, 0);
        uvarr[1] = new Vector2(1, 0);
        uvarr[2] = new Vector2(1, 1);
        uvarr[3] = new Vector2(0, 1);
        uvarr[4] = new Vector2(0, 0);
        uvarr[5] = new Vector2(1, 1);
        
        

        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        
        for (int i = 0; i < points.Length - 1; i++)
        {
            var dir = (points[i + 1] - points[i]).Normalized().Rotated(Mathf.Pi / 2);
            var width = 4;
            
            // pointarr[0] = points[i] + dir * width;
            // pointarr[1] = points[i + 1] + dir * width;
            // pointarr[2] = points[i + 1] - dir * width;
            // pointarr[3] = points[i] - dir * width;
            
            // two triangles, six points
            pointarr[0] = points[i] + dir * width;
            pointarr[1] = points[i + 1] + dir * width;
            pointarr[2] = points[i + 1] - dir * width;
            pointarr[3] = points[i] - dir * width;
            pointarr[4] = points[i] + dir * width;
            pointarr[5] = points[i + 1] - dir * width;
            for (int j = 0; j < 6; j++)
            {
                st.SetUV(uvarr[j]);
                st.AddVertex(new Vector3(pointarr[j].X, pointarr[j].Y, 0));
            }
        }
        
        mesh = st.Commit();
        
        DrawMesh(mesh, new Texture2D());
    }
}