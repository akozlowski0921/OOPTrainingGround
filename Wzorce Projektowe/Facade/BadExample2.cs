using System;

namespace DesignPatterns.Facade.Bad2
{
    // ❌ BAD: Complex API exposure

    public class VideoConverter
    {
        public void Step1() { }
        public void Step2() { }
        public void Step3() { }
        public void Step4() { }
        public void Step5() { }
    }

    public class Client
    {
        public void ConvertVideo()
        {
            var converter = new VideoConverter();
            // ❌ Client musi znać wszystkie kroki
            converter.Step1();
            converter.Step2();
            converter.Step3();
            converter.Step4();
            converter.Step5();
        }
    }
}
