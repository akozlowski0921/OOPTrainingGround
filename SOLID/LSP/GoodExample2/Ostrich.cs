namespace SOLID.LSP.Good2
{
    // ✅ Ostrich doesn't implement IFlyable
    public class Ostrich : Bird
    {
        public override void Eat()
        {
            System.Console.WriteLine("Ostrich eating");
        }

        public override void Move()
        {
            Run();
        }

        public void Run()
        {
            System.Console.WriteLine("Ostrich running");
        }
    }
}
