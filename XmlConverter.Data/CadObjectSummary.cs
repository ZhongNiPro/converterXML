namespace XmlConverter.Data
{
    public class CadObjectSummary
    {
        public long ObjectId { get; set; }
        public required string CadastralNumber { get; set; }
        public  required string ObjectType { get; set; }
        public DateTime ExtractDate { get; set; }
    }
}