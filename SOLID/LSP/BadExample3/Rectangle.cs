namespace SOLID.LSP.Bad3
{
    // ❌ BAD: Square violates LSP when substituted for Rectangle
    public class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int CalculateArea()
        {
            return Width * Height;
        }
    }

    public class Square : Rectangle
    {
        public override int Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value; // Problem: Changing both dimensions
            }
        }

        public override int Height
        {
            get => base.Height;
            set
            {
                base.Width = value; // Problem: Changing both dimensions
                base.Height = value;
            }
        }
    }
}
