using System;

namespace DesignPatterns.Facade.Good3
{
    // ✅ GOOD: Simple facade

    public class SystemFacade
    {
        private readonly SubsystemA _a;
        private readonly SubsystemB _b;
        private readonly SubsystemC _c;

        public SystemFacade()
        {
            _a = new SubsystemA();
            _a.Initialize();
            _b = new SubsystemB();
            _b.Configure(_a);
            _c = new SubsystemC();
            _c.Setup(_a, _b);
        }

        public void PerformOperation()
        {
            _a.DoWork();
            _b.Process();
            _c.Finalize();
        }
    }

    class SubsystemA
    {
        public void Initialize() { }
        public void DoWork() => Console.WriteLine("A working");
    }

    class SubsystemB
    {
        public void Configure(SubsystemA a) { }
        public void Process() => Console.WriteLine("B processing");
    }

    class SubsystemC
    {
        public void Setup(SubsystemA a, SubsystemB b) { }
        public void Finalize() => Console.WriteLine("C finalizing");
    }
}
