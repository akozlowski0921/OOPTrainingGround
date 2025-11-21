namespace SOLID.LSP.Good3
{
    public class Rectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override int CalculateArea()
        {
            return Width * Height;
        }
    }
}
