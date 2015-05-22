namespace Void.UI
{
    public class CellMetrics
    {
        public CellMetrics(int height, int width)
        {
            Height = height;
            Width = width;
        }

        public int Height { get; private set;  }
        public int Width { get; private set; }
    }
}