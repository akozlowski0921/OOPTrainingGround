namespace SOLID.LSP.Good2
{
    public class Sparrow : Bird, IFlyable
    {
        public override void Eat()
        {
            System.Console.WriteLine("Sparrow eating");
        }

        public override void Move()
        {
            Fly();
        }

        public void Fly()
        {
            System.Console.WriteLine("Sparrow flying");
        }
    }
}
