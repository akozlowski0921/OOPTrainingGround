namespace SOLID.LSP.Bad2
{
    // ❌ BAD: Not all birds can fly - violates LSP
    public abstract class Bird
    {
        public abstract void Fly();
        public abstract void Eat();
    }

    public class Sparrow : Bird
    {
        public override void Fly()
        {
            System.Console.WriteLine("Sparrow flying");
        }

        public override void Eat()
        {
            System.Console.WriteLine("Sparrow eating");
        }
    }

    public class Penguin : Bird
    {
        public override void Fly()
        {
            // Problem: Penguin can't fly!
            throw new System.NotImplementedException("Penguins can't fly!");
        }

        public override void Eat()
        {
            System.Console.WriteLine("Penguin eating");
        }
    }

    public class Ostrich : Bird
    {
        public override void Fly()
        {
            // Problem: Ostrich can't fly either!
            throw new System.NotImplementedException("Ostriches can't fly!");
        }

        public override void Eat()
        {
            System.Console.WriteLine("Ostrich eating");
        }
    }
}
