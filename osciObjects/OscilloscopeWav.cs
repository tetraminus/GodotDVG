using Godot;


public partial class OscilloscopeWav : Node2D
{
	
	private AudioStreamPlayer _audioStreamPlayer;
	[Export]
	private AudioStream _audioStream;

	
	private int sampleRate;
	private int channels;
	public AudioStreamWav.FormatEnum format;
	private float time = 0.1f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_audioStream == null)
		{
			GD.PrintErr("AudioStream is not set");
			return;
		}
		if (_audioStream.CanBeSampled() == false)
		{
			GD.PrintErr("AudioStream cannot be sampled");
			return;
		}
		_audioStreamPlayer = GetNode("AudioStreamPlayer") as AudioStreamPlayer;
		
		_audioStreamPlayer.Stream = _audioStream;
		_audioStreamPlayer.Play();



		if (_audioStream is AudioStreamWav wav)
		{
			sampleRate = wav.MixRate;
			channels = wav.Stereo ? 2 : 1;
			format = wav.Format;
		}

		else
		{
			GD.PrintErr("AudioStream is not a wav file");
			
		}
	}
	
	private float getWavAudioSample(byte[] samples,int timeSamples, int channel)
	{
		if (_audioStream is AudioStreamWav wav)
		{
			//byte[] 
			// use format to get the correct sample size
			
			
			int sampleSize = format switch
			{
				AudioStreamWav.FormatEnum.Format8Bits => 1,
				AudioStreamWav.FormatEnum.Format16Bits => 2,
				_ => 0
			};
			
			int sampleIndex = timeSamples * channels * sampleSize + channel * sampleSize;
			
			if (sampleIndex + sampleSize > samples.Length)
			{
				return 0;
			}
			
			
			
				//# Combine low bits and high bits to obtain 16-bit value
				// var u = b0 | (b1 << 8)
				// # Emulate signed to unsigned 16-bit conversion
				// u = (u + 32768) & 0xffff
				// # Convert to -1..1 range
				// var s = float(u - 32768) / 32768.0
				
				float sample = 0;
				if (sampleSize == 1)
				{
					sample = samples[sampleIndex] / 255.0f;
				}
				else if (sampleSize == 2)
				{
					sample = (short)(samples[sampleIndex] | (samples[sampleIndex + 1] << 8)) / 32768.0f;
				}
				
			return sample;
			
		}
		else
		{
			GD.PrintErr("AudioStream is not a wav file");
			return 0;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var points = new Vector2[(int)(sampleRate * time)];
		if (_audioStream is AudioStreamWav wav)
		{

			var samples = wav.GetData();
			int timeSamples = (int)(_audioStreamPlayer.GetPlaybackPosition() * sampleRate);
			
			//x-y mode with left and right channels
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = new Vector2(getWavAudioSample(samples,timeSamples+i, 0), getWavAudioSample(samples,timeSamples+i, 1));
				points[i] *= 10;
				points[i] += GetViewportRect().GetCenter();
			}	
			
			
			OsciManager.Ins.AddPoints(points);
			
		}
		else
		{
			GD.PrintErr("AudioStream is not a wav file");
		}
		
		
	}
}
