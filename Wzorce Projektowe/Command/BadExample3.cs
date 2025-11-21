using System;

namespace DesignPatterns.Command.Bad3
{
    // ❌ BAD: No macro support

    public class SimpleEditor
    {
        public void Cut() { Console.WriteLine("Cut"); }
        public void Copy() { Console.WriteLine("Copy"); }
        public void Paste() { Console.WriteLine("Paste"); }
        
        // ❌ Nie można wykonać sekwencji operacji jako jednej
    }
}
