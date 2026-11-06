namespace XmlConverter.Data
{
    public class CadObjectBlock : ICadastralObject
    {
        public CadObjectBlock(
            string organRegistrRight,
            DateOnly dateFormation,
            string cadastralNumber
            )
        {
            OrganRegistrRight = organRegistrRight;
            DateFormation = dateFormation;
            CadastralNumber = cadastralNumber;
        }

        public string OrganRegistrRight { get; private set; }
        public DateOnly DateFormation { get; private set; }
        public string CadastralNumber { get; private set; }
        public List<Contour> Contours { get; private set; } = new List<Contour>();
    }
}
