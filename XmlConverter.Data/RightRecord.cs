namespace XmlConverter.Data
{
    public class RightRecord
    {
        public RightRecord(
            int recordNumber,
            string rightType,
            string rightHolder
            )
        {
            RecordNumber = recordNumber;
            RightType = rightType;
            RightHolder = rightHolder;
        }

        public int RecordNumber { get; private set; }
        public string RightType { get; private set; }
        public string RightHolder { get; private set; }
    }
}
