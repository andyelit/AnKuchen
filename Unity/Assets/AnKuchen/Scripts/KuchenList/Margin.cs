namespace AnKuchen.KuchenList
{
    public interface IReadonlyMargin
    {
        float Top { get; }
        float Bottom { get; }
    }

    public class Margin : IReadonlyMargin
    {
        public float Top { get; set; }
        public float Bottom { get; set; }

        public float TopBottom
        {
            set
            {
                Top = value;
                Bottom = value;
            }
        }
    }

    public class Spacer
    {
        public Spacer(float size)
        {
            Size = size;
        }

        public float Size { get; }
    }
}