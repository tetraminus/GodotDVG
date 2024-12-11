
using System;
using System.IO;
using FileAccess = Godot.FileAccess;


namespace VectorRendering;

public class GodotFileStream : Stream
{
    
    private FileAccess _fileAccess;
    
    public GodotFileStream(FileAccess fileAccess)
    {
        _fileAccess = fileAccess;
    }

    
    public override void Flush()
    {
        _fileAccess.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // copy the data from the file access to the buffer and return the number of bytes read
        byte[] data = _fileAccess.GetBuffer((uint)count);
        Array.Copy(data, 0, buffer, offset, data.Length);
            
        return data.Length;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                _fileAccess.Seek((ulong)offset);
                break;
            case SeekOrigin.Current:
                _fileAccess.Seek((ulong)_fileAccess.GetPosition() + (ulong) offset);
                break;
            case SeekOrigin.End:
                _fileAccess.Seek((ulong) _fileAccess.GetLength() + (ulong)offset);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }
        
        return (long)_fileAccess.GetPosition();
    }

    public override void SetLength(long value)
    {
        throw new System.NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanRead => _fileAccess.IsOpen() && !_fileAccess.EofReached();
    public override bool CanSeek => _fileAccess.IsOpen();
    public override bool CanWrite => false;
    public override long Length => (long)_fileAccess.GetLength();
    public override long Position { get => (long)_fileAccess.GetPosition(); set => Seek(value, SeekOrigin.Begin); }
}