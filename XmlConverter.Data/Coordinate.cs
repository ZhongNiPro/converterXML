namespace XmlConverter.Data
{
    public class Coordinate
    {
        public Coordinate(
             int ordNmb,
             double x,
             double y
           )
        {
            OrdNmb = ordNmb;
            X = x;
            Y = y;
        }

        public int OrdNmb { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
    }
}
