namespace XmlConverter.Data
{
    public class CadObjectLand : ICadastralObject
    {
        public CadObjectLand(
            DateOnly dateFormation,
            string cadNumber,
            string quarterCadNumber,
            string objectType,
            string subType,
            string category,
            string permittedUse,
            double area,
            double inaccuracy,
            string address,
            decimal cost,
            string spesialNotes
            )
        {
            DateFormation = dateFormation;
            CadNumber = cadNumber;
            QuarterCadNumber = quarterCadNumber;
            ObjectType = objectType;
            SubType = subType;
            Category = category;
            PermittedUse = permittedUse;
            Area = area;
            Inaccuracy = inaccuracy;
            Address = address;
            Cost = cost;
            SpesialNotes = spesialNotes;
        }

        public DateOnly DateFormation { get; private set; }
        public string CadNumber { get; private set; }
        public string QuarterCadNumber { get; private set; }
        public string ObjectType { get; private set; }
        public string SubType { get; private set; }
        public List<string>? AscendantCadNumbers { get; private set; } = new List<string>();
        public List<string>? DescendantCadNumbers { get; private set; } = new List<string>();
        public List<string>? IncludedCadNumbers { get; private set; } = new List<string>();
        public string Category { get; private set; }
        public string PermittedUse { get; private set; }
        public double Area { get; private set; }
        public double? Inaccuracy { get; private set; }
        public string Address { get; private set; }
        public decimal? Cost { get; private set; }
        public List<ObjectPart>? ObjectParts { get; private set; } = new List<ObjectPart>();
        public List<Contour>? Contours { get; private set; } = new List<Contour>();
        public string? SpesialNotes { get; private set; }
        public List<RightRecord>? RightRecords { get; private set; } = new List<RightRecord>();
    }
}
