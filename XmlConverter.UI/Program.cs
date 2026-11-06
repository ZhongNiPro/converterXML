using System.Configuration; 

namespace XmlConverter.UI
{
    internal static class Program
    {
        public const string ConnectionStringConfigName = "PostgresCadastreConnection";

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringConfigName];

            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=cadastre_db";

            if (settings != null && string.IsNullOrWhiteSpace(settings.ConnectionString) == false)
            {
                connectionString = settings.ConnectionString;
            }

            MainForm mainForm = new MainForm(connectionString);

            Application.Run(mainForm);
        }
    }
}