using System;
using System.Text;

public class CryptoStream : IDisposable
{
    private bool _disposed = false;
    private string _algorithm;
    private bool _isStreamOpen;

    
    public string Algorithm => _algorithm;
    public bool IsStreamOpen => _isStreamOpen;

    
    public CryptoStream(string algorithm)
    {
        _algorithm = algorithm;
        _isStreamOpen = true;
        Console.WriteLine($"[{_algorithm}] Криптопотік відкрито");
    }

    
    public string Encrypt(string data)
    {
        if (!_isStreamOpen)
        {
            Console.WriteLine($"[{_algorithm}] Потік закрито, шифрування неможливе");
            return string.Empty;
        }

        string encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        Console.WriteLine($"[{_algorithm}] Зашифровано: \"{data}\" -> {encrypted}");
        return encrypted;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                
                Console.WriteLine($"[{_algorithm}] Disposing managed resources");
            }

            
            if (_isStreamOpen)
            {
                Console.WriteLine($"[{_algorithm}] Releasing unmanaged resource: криптопотік закрито");
                _isStreamOpen = false;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CryptoStream()
    {
        Console.WriteLine($"[{_algorithm}] Викликано деструктор");
        Dispose(false);
    }
}