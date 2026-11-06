using Npgsql;
using System.Configuration;
using XmlConverter.Business;
using XmlConverter.Database;

namespace XmlConverter.UI
{
    public partial class SettingsForm : Form
    {
        public const string ConnectionStringConfigName = "PostgresCadastreConnection";

        private readonly DatabaseManager _dbManager;
        private readonly CadObjectRepository _repository;
        private readonly XmlStreamDispatcher _dispatcher;

        private const string BackupFileFilter = "Файлы резервных копий (*.backup)|*.backup|Все файлы (*.*)|*.*";

        public SettingsForm(DatabaseManager dbManager, CadObjectRepository repository, XmlStreamDispatcher dispatcher)
        {
            InitializeComponent();

            _dbManager = dbManager;
            _repository = repository;
            _dispatcher = dispatcher;
        }

        private async void ButtonTestAndSave_Click(object sender, EventArgs e)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Host = TextHost.Text.Trim(),
                Port = int.TryParse(TextPort.Text, out int port) ? port : 5432,
                Username = TextUser.Text.Trim(),
                Password = TextPassword.Text,
                Database = TextDatabase.Text.Trim(),
                Pooling = true
            };

            string newConnectionString = builder.ConnectionString;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                ButtonTestAndSave.Enabled = false;

                await using (NpgsqlConnection testConnection = new NpgsqlConnection(newConnectionString))
                {
                    await testConnection.OpenAsync();

                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    ConnectionStringSettings? existingSettings = config.ConnectionStrings.ConnectionStrings[ConnectionStringConfigName];

                    if (existingSettings != null)
                    {
                        existingSettings.ConnectionString = newConnectionString;
                    }
                    else
                    {
                        ConnectionStringSettings newSettings = new ConnectionStringSettings(ConnectionStringConfigName, newConnectionString, "Npgsql");
                        config.ConnectionStrings.ConnectionStrings.Add(newSettings);
                    }

                    config.Save(ConfigurationSaveMode.Modified);

                    ConfigurationManager.RefreshSection("connectionStrings");
                }

                _repository.UpdateConnectionString(newConnectionString);
                _dbManager.UpdateConnectionString(newConnectionString);
                _dispatcher.UpdateConnectionString(newConnectionString);

                MessageBox.Show("Подключение успешно проверено и сохранено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось подключиться к СУБД: {ex.Message}", "Ошибка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ButtonTestAndSave.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private async void ButtonCreateBackup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = BackupFileFilter;
                saveFileDialog.Title = "Укажите файл для сохранения бэкапа базы данных";
                saveFileDialog.FileName = $"{TextDatabase.Text.Trim()}_{DateTime.Now:yyyyMMdd}.backup";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        ButtonCreateBackup.Enabled = false;

                        string backupPath = saveFileDialog.FileName;
                        string host = TextHost.Text.Trim();
                        string user = TextUser.Text.Trim();
                        string password = TextPassword.Text;
                        string dbName = TextDatabase.Text.Trim();

                        await _dbManager.BackupDatabaseAsync(backupPath, host, user, password, dbName);

                        MessageBox.Show("Резервная копия успешно создана!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при создании резервной копии: {ex.Message}", "Ошибка бэкапа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        ButtonCreateBackup.Enabled = true;
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private async void ButtonRestoreBackup_Click(object sender, EventArgs e)
        {
            DialogResult warningResult = MessageBox.Show(
                "Внимание! Восстановление базы данных приведет к перезаписи существующих таблиц. Вы уверены, что хотите продолжить?",
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (warningResult == DialogResult.No)
            {
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = BackupFileFilter;
                openFileDialog.Title = "Выберите файл резервной копии для восстановления СУБД";

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        ButtonRestoreBackup.Enabled = false;

                        string backupPath = openFileDialog.FileName;
                        string host = TextHost.Text.Trim();
                        string user = TextUser.Text.Trim();
                        string password = TextPassword.Text;
                        string dbName = TextDatabase.Text.Trim();

                        await _dbManager.RestoreDatabaseAsync(backupPath, host, user, password, dbName);

                        MessageBox.Show("База данных успешно восстановлена из резервной копии!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при восстановлении базы данных: {ex.Message}", "Ошибка восстановления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        ButtonRestoreBackup.Enabled = true;
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringConfigName];

                if (settings != null && string.IsNullOrWhiteSpace(settings.ConnectionString) == false)
                {
                    NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(settings.ConnectionString);

                    TextHost.Text = builder.Host;
                    TextPort.Text = builder.Port.ToString();
                    TextUser.Text = builder.Username;
                    TextPassword.Text = builder.Password;
                    TextDatabase.Text = builder.Database;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить текущие настройки из конфигурации: {ex.Message}", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}