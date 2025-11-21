namespace SOLID.LSP.Good3
{
    // ✅ Square is independent, not inheriting from Rectangle
    public class Square : Shape
    {
        public int Side { get; set; }

        public Square(int side)
        {
            Side = side;
        }

        public override int CalculateArea()
        {
            return Side * Side;
        }
    }
}
