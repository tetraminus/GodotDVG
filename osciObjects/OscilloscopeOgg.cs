using System;
using System.IO;
using Godot;
using NVorbis;
using VectorRendering;
using AudioStream = Godot.AudioStream;
using FileAccess = Godot.FileAccess;

namespace VectorRendering.osciObjects;

public partial class OscilloscopeOgg : Node2D
{

    private VorbisReader _vorbisReader;
    private AudioStreamPlayer _audioStreamPlayer;
    [Export] 
    private string filePath;
    private int channels;
    private int sampleRate;
    private float time = 0.02f;
    private float[] samples;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (filePath == null)
        {
            GD.PrintErr("File path is not set");
            return;
        }
        if (System.IO.Path.GetExtension(filePath) != ".ogg")
        {
            GD.PrintErr("File is not an ogg file");
            return;
        }

        var fa = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        if (fa == null)
        {
            GD.PrintErr("File not found");
            return;
        }

        

        _vorbisReader = new VorbisReader(new GodotFileStream(fa));
        _audioStreamPlayer = GetNode("AudioStreamPlayer") as AudioStreamPlayer;
        _audioStreamPlayer.Stream = GD.Load(filePath) as AudioStream;
        _audioStreamPlayer.Play();
        
       
        channels = _vorbisReader.Channels;
        sampleRate = _vorbisReader.SampleRate;
        
        samples = new float[(int)(sampleRate * time) * channels];
        
        
    }

    public override void _Process(double delta)
    {
        if (!_audioStreamPlayer.Playing)
        {
            return;
        }
        
        GD.Print("begin ");
        var points = new Vector2[(int)(sampleRate * time)];
        if (_audioStreamPlayer == null) return;
        {

            
            long timeSamples = (long)(_audioStreamPlayer.GetPlaybackPosition() * sampleRate);
            // retime to end at current time
            timeSamples -= (long)(sampleRate * time);
            
            //clamp the time samples to the total samples
            timeSamples = Math.Clamp(timeSamples, 0, _vorbisReader.TotalSamples);
            
            
            
            
            _vorbisReader.SeekTo(timeSamples);
            _vorbisReader.ReadSamples(samples, 0, samples.Length);
          
            
			
            //x-y mode with left and right channels
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(getAudioSample(samples, i, 0), getAudioSample(samples, i, 1));
                points[i] *= GetViewportRect().Size.Y / 2; 
                points[i] *= new Vector2(1, -1);

                points[i] += GetViewportRect().GetCenter();
            }	
			
			
            OsciManager.Ins.AddPoints(points);
			
        }


        GD.Print("end ");
    }

    private float getAudioSample(float[] samples,int timeSamples, int channel)
    {
        
        if (channel >= channels)
        {
            return 0;
        }
        
        int sampleIndex = timeSamples * channels + channel;
        
        if (sampleIndex >= samples.Length)
        {
            return 0;
        }
        
        return samples[sampleIndex];
    }
}