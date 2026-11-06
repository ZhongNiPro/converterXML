namespace XmlConverter.Data
{
    public class Contour
    {
        public Contour(
            int numberPP,
            string cadNumber,
            string skId,
            List<Coordinate> coordinates
            )
        {
            NumberPP = numberPP;
            CadNumber = cadNumber;
            SkId = skId;
            Coordinates = coordinates ?? new List<Coordinate>();
        }

        public int NumberPP { get; private set; }
        public string CadNumber { get; private set; }
        public string SkId { get; private set; }
        public List<Coordinate> Coordinates { get; private set; }
    }
}
