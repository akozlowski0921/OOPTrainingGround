namespace SOLID.LSP.Good2
{
    // ✅ Penguin doesn't implement IFlyable
    public class Penguin : Bird
    {
        public override void Eat()
        {
            System.Console.WriteLine("Penguin eating");
        }

        public override void Move()
        {
            Swim();
        }

        public void Swim()
        {
            System.Console.WriteLine("Penguin swimming");
        }
    }
}
