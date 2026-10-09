using System;
using System.Runtime.CompilerServices;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        
        Console.WriteLine("=== 1. Використання using ===");
        using (var stream1 = new CryptoStream("AES"))
        {
            stream1.Encrypt("Hello, using!");
        } 

        
        Console.WriteLine("\n=== 2. Явний виклик Dispose() ===");
        var stream2 = new CryptoStream("RSA");
        stream2.Encrypt("Hello, Dispose!");
        stream2.Dispose();
        stream2.Encrypt("Спроба після Dispose");

       
        Console.WriteLine("\n=== 3. Деструктор через GC.Collect() ===");
        CreateForgottenStream();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine("\nКінець програми");
    }

    
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void CreateForgottenStream()
    {
        var stream3 = new CryptoStream("DES");
        stream3.Encrypt("Забув викликати Dispose");
    }
}