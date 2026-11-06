namespace XmlConverter.Data
{
    public class CadObjectBoundary : ICadastralObject
    {
        public CadObjectBoundary(
            DateOnly dateFormation,
            string quarterCadNumber,
            string regNumbBorder,
            string typeBoundary,
            string typeZone,
            string description,
            string nameByDoc
            )
        {
            DateFormation = dateFormation;
            RegNumbBorder = regNumbBorder;
            TypeBoundary = typeBoundary;
            TypeZone = typeZone;
            Description = description;
            NameByDoc = nameByDoc;

            AddQuarterNumber(quarterCadNumber);
        }

        public DateOnly DateFormation { get; private set; }
        public List<string> QuarterCadNumbers { get; private set; } = new List<string> ();
        public string RegNumbBorder { get; private set; }
        public string TypeBoundary { get; private set; }
        public string? TypeZone { get; private set; }
        public string? Description { get; private set; }
        public string? NameByDoc { get; private set; }
        public List<Contour>? Contours { get; private set; } = new List<Contour>();

        public void AddQuarterNumber(string number)
        {
            if (QuarterCadNumbers.Contains (number)==false)
                QuarterCadNumbers.Add (number);
        }
    }
}
