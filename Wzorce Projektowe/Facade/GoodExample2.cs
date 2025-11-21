using System;

namespace DesignPatterns.Facade.Good2
{
    // ✅ GOOD: Simple interface

    public class VideoConverter
    {
        private void Step1() { }
        private void Step2() { }
        private void Step3() { }
        private void Step4() { }
        private void Step5() { }

        public void Convert(string input, string output)
        {
            // ✅ Enkapsulacja complexity
            Step1();
            Step2();
            Step3();
            Step4();
            Step5();
            Console.WriteLine($"Converted {input} to {output}");
        }
    }

    public class Client
    {
        public void ConvertVideo()
        {
            var converter = new VideoConverter();
            // ✅ Simple API
            converter.Convert("input.avi", "output.mp4");
        }
    }
}
