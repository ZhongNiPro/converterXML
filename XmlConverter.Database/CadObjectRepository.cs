namespace XmlConverter.Database
{
    public partial class CadObjectRepository
    {
        private  string _connectionString;

        public CadObjectRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void UpdateConnectionString(string newConnectionString)
        {
            if (string.IsNullOrWhiteSpace(newConnectionString))
            {
                throw new ArgumentException("Строка подключения не может быть пустой.");
            }
            _connectionString = newConnectionString;
        }
    }
}