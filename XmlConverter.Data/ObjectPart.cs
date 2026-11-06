namespace XmlConverter.Data
{
    public class ObjectPart
    {
        public ObjectPart(
            int partNumber, 
            List<Contour> contours
            )
        {
            PartNumber = partNumber;
            PartsContours = contours ?? new List<Contour>();
        }

        public int PartNumber { get; private set; }
        public List<Contour>? PartsContours { get; private set; }
    }
}
