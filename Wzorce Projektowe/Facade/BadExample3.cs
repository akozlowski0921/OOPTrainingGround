using System;

namespace DesignPatterns.Facade.Bad3
{
    // ❌ BAD: Complex initialization

    public class ComplexSystem
    {
        private SubsystemA _a;
        private SubsystemB _b;
        private SubsystemC _c;

        public void UseIt()
        {
            // ❌ Client musi wiedzieć o wszystkich subsystemach
            _a = new SubsystemA();
            _a.Initialize();
            _b = new SubsystemB();
            _b.Configure(_a);
            _c = new SubsystemC();
            _c.Setup(_a, _b);
        }
    }

    class SubsystemA
    {
        public void Initialize() { }
    }

    class SubsystemB
    {
        public void Configure(SubsystemA a) { }
    }

    class SubsystemC
    {
        public void Setup(SubsystemA a, SubsystemB b) { }
    }
}
