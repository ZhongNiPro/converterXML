namespace XmlConverter.Data
{
    public class CadObjectBuilt : ICadastralObject
    {
        public CadObjectBuilt(
            DateOnly dateFormation,
            string cadNumber,
            string quarterCadNumber,
            string objectType,
            string purpose,
            double area,
            string address,
            decimal cost,
            string spesialNotes,
            double extension,
            double builtUpArea,
            double depth,
            double occurenceDepth,
            double volume,
            double height
            )
        {
            DateFormation = dateFormation;
            CadNumber = cadNumber;
            QuarterCadNumber = quarterCadNumber;
            ObjectType = objectType;
            Purpose = purpose;
            Area = area;
            Address = address;
            Cost = cost;
            SpesialNotes = spesialNotes;
            Extension = extension;
            BuiltUpArea = builtUpArea;
            Depth = depth;
            OccurenceDepth = occurenceDepth;
            Volume = volume;
            Height = height;
        }

        public DateOnly DateFormation { get; private set; }
        public string CadNumber { get; private set; }
        public string QuarterCadNumber { get; private set; }
        public string ObjectType { get; private set; }
        public string Purpose { get; private set; }
        public double? Area { get; private set; }
        public string Address { get; private set; }
        public decimal? Cost { get; private set; }
        public List<Contour>? Contours { get; private set; } = new List<Contour>();
        public string? SpesialNotes { get; private set; }
        public List<RightRecord>? RightRecords { get; private set; } = new List<RightRecord>();
        public double? Extension { get; private set; }
        public double? BuiltUpArea { get; private set; }
        public double? Depth { get; private set; }
        public double? OccurenceDepth { get; private set; }
        public double? Volume { get; private set; }
        public double? Height { get; private set; }
    }
}
