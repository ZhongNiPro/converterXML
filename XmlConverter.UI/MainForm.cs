using XmlConverter.Business;
using XmlConverter.Data;
using XmlConverter.Database;

namespace XmlConverter.UI
{
    public partial class MainForm : Form
    {
        private const string FileDialogFilter = "Кадастровые файлы (*.xml, *.zip)|*.xml;*.zip|XML файлы (*.xml)|*.xml|ZIP архивы (*.zip)|*.zip"; 
        private readonly XmlStreamDispatcher _xmlStreamDispatcher;
        private readonly CadObjectRepository _repository;
        private readonly DatabaseManager _dbManager;
        private CancellationTokenSource? _parsingCancellationTokenSource;
        private readonly List<string> _selectedFilesPaths = new List<string>();

        public MainForm(string connectionString)
        {
            InitializeComponent();

            _xmlStreamDispatcher = new XmlStreamDispatcher(connectionString);
            _repository = new CadObjectRepository(connectionString);
            _dbManager = new DatabaseManager(connectionString);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                await _dbManager.CheckAndInitializeSchemaAsync();

                List<CadObjectSummary> summaryList = await _repository.GetObjectsSummaryAsync();

                GridCadastralObjects.DataSource = summaryList;

                if (GridCadastralObjects.Columns.Contains(DatabaseConstants.Columns.ObjectId))
                {
                    GridCadastralObjects.Columns[DatabaseConstants.Columns.ObjectId].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}", "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void ButtonSearchObjects_Click(object sender, EventArgs e)
        {
            string searchPattern = TextSearchCadastralNumber.Text;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                List<CadObjectSummary> searchResults = await _repository.SearchObjectsAsync(searchPattern);
                GridCadastralObjects.DataSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}", "Ошибка поиска", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void GridCadastralObjects_SelectionChanged(object sender, EventArgs e)
        {
            if (GridCadastralObjects.CurrentRow == null)
                return;

            CadObjectSummary? selectedSummary = GridCadastralObjects.CurrentRow.DataBoundItem as CadObjectSummary;

            if (selectedSummary == null)
                return;

            try
            {
                ICadastralObject? fullObject = await _repository.GetFullObjectAsync(selectedSummary.ObjectId);

                if (fullObject != null)
                {
                    DisplayObjectDetails(fullObject);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки карточки: {ex.Message}");
            }
        }

        private void DisplayObjectDetails(ICadastralObject cadObject)
        {
            TextMainProperties.Clear();
            TextPropertyRights.Clear();

            switch (cadObject)
            {
                case CadObjectLand land:
                    TextMainProperties.AppendText($"ТИП ОБЪЕКТА: ЗЕМЕЛЬНЫЙ УЧАСТОК\n");
                    TextMainProperties.AppendText($"Кадастровый номер: {land.CadNumber}\n");
                    TextMainProperties.AppendText($"Кадастровый квартал: {land.QuarterCadNumber}\n");
                    TextMainProperties.AppendText($"Адрес: {land.Address}\n");
                    TextMainProperties.AppendText($"Площадь: {land.Area} кв.м. (Погрешность: {land.Inaccuracy} кв.м.)\n");
                    TextMainProperties.AppendText($"Категория земель: {land.Category}\n");
                    TextMainProperties.AppendText($"Разрешенное использование: {land.PermittedUse}\n");
                    TextMainProperties.AppendText($"Кадастровая стоимость: {land.Cost} руб.\n");
                    TextMainProperties.AppendText($"Дата формирования выписки: {land.DateFormation}\n");
                    TextMainProperties.AppendText($"Особые отметки: {land.SpesialNotes}\n\n");

                    if (land.AscendantCadNumbers != null && land.AscendantCadNumbers.Count > 0)
                    {
                        TextMainProperties.AppendText($"Предыдущие кадастровые номера:\n");
                        foreach (string ascendant in land.AscendantCadNumbers)
                        {
                            TextMainProperties.AppendText($" - {ascendant}\n");
                        }
                    }

                    if (land.DescendantCadNumbers != null && land.DescendantCadNumbers.Count > 0)
                    {
                        TextMainProperties.AppendText($"Последующие кадастровые номера:\n");
                        foreach (string descendant in land.DescendantCadNumbers)
                        {
                            TextMainProperties.AppendText($" - {descendant}\n");
                        }
                    }

                    if (land.IncludedCadNumbers != null && land.IncludedCadNumbers.Count > 0)
                    {
                        TextMainProperties.AppendText($"Входящие объекты:\n");
                        foreach (string included in land.IncludedCadNumbers)
                        {
                            TextMainProperties.AppendText($" - {included}\n");
                        }
                    }

                    if (land.RightRecords != null && land.RightRecords.Count > 0)
                    {
                        foreach (RightRecord record in land.RightRecords)
                        {
                            TextPropertyRights.AppendText($"Запись №{record.RecordNumber}\n");
                            TextPropertyRights.AppendText($"Вид права: {record.RightType}\n");
                            TextPropertyRights.AppendText($"Правообладатель: {record.RightHolder}\n");
                            TextPropertyRights.AppendText(new string('-', 30) + "\n");
                        }
                    }
                    else
                    {
                        TextPropertyRights.AppendText("Сведения о зарегистрированных правах отсутствуют.");
                    }
                    break;

                case CadObjectBuilt built:
                    TextMainProperties.AppendText($"ТИП ОБЪЕКТА: ЗДАНИЕ / ОКС\n");
                    TextMainProperties.AppendText($"Кадастровый номер: {built.CadNumber}\n");
                    TextMainProperties.AppendText($"Кадастровый квартал: {built.QuarterCadNumber}\n");
                    TextMainProperties.AppendText($"Назначение здания: {built.Purpose}\n");
                    TextMainProperties.AppendText($"Адрес: {built.Address}\n");
                    TextMainProperties.AppendText($"Площадь: {built.Area} кв.м.\n");
                    TextMainProperties.AppendText($"Высота: {built.Height} м. | Объем: {built.Volume} куб.м.\n");
                    TextMainProperties.AppendText($"Глубина: {built.Depth} м. | Глубина залегания: {built.OccurenceDepth} м.\n");
                    TextMainProperties.AppendText($"Протяженность: {built.Extension} м. | Площадь застройки: {built.BuiltUpArea} кв.м.\n");
                    TextMainProperties.AppendText($"Кадастровая стоимость: {built.Cost} руб.\n");
                    TextMainProperties.AppendText($"Дата формирования выписки: {built.DateFormation}\n");
                    TextMainProperties.AppendText($"Особые отметки: {built.SpesialNotes}\n");

                    if (built.RightRecords != null && built.RightRecords.Count > 0)
                    {
                        foreach (RightRecord record in built.RightRecords)
                        {
                            TextPropertyRights.AppendText($"Запись №{record.RecordNumber}\n");
                            TextPropertyRights.AppendText($"Вид права: {record.RightType}\n");
                            TextPropertyRights.AppendText($"Правообладатель: {record.RightHolder}\n");
                            TextPropertyRights.AppendText(new string('-', 30) + "\n");
                        }
                    }
                    else
                    {
                        TextPropertyRights.AppendText("Сведения о зарегистрированных правах отсутствуют.");
                    }
                    break;

                case CadObjectBoundary boundary:
                    TextMainProperties.AppendText($"ТИП ОБЪЕКТА: ТЕРРИТОРИАЛЬНАЯ ГРАНИЦА / ЗОНА\n");
                    TextMainProperties.AppendText($"Реестровый номер границы: {boundary.RegNumbBorder}\n");
                    TextMainProperties.AppendText($"Вид границы: {boundary.TypeBoundary}\n");
                    TextMainProperties.AppendText($"Тип зоны: {boundary.TypeZone}\n");
                    TextMainProperties.AppendText($"Наименование по документу: {boundary.NameByDoc}\n");
                    TextMainProperties.AppendText($"Описание: {boundary.Description}\n");
                    TextMainProperties.AppendText($"Дата формирования выписки: {boundary.DateFormation}\n\n");

                    if (boundary.QuarterCadNumbers != null && boundary.QuarterCadNumbers.Count > 0)
                    {
                        TextMainProperties.AppendText($"Входит в кадастровые кварталы:\n");
                        foreach (string quarterNum in boundary.QuarterCadNumbers)
                        {
                            TextMainProperties.AppendText($" - {quarterNum}\n");
                        }
                    }

                    TextPropertyRights.AppendText("Для данного типа кадастрового объекта сведения о правах не применимы.");
                    break;
                case CadObjectBlock block:
                    TextMainProperties.AppendText($"ТИП ОБЪЕКТА: КАДАСТРОВЫЙ КВАРТАЛ\n");
                    TextMainProperties.AppendText($"Номер квартала: {block.CadastralNumber}\n");
                    TextMainProperties.AppendText($"Орган регистрации прав: {block.OrganRegistrRight}\n");
                    TextMainProperties.AppendText($"Дата формирования выписки: {block.DateFormation}\n");
                    TextPropertyRights.AppendText("Для данного типа кадастрового объекта сведения о правах не применимы.");
                    break;
            }
        }

        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            if (_selectedFilesPaths.Count == 0)
            {
                MessageBox.Show("Пожалуйста, сначала выберите файлы для импорта.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _parsingCancellationTokenSource?.Dispose();
            _parsingCancellationTokenSource = new CancellationTokenSource();

            IProgress<double> progressReporter = new Progress<double>(percentage =>
            {
                myProgressBar.Value = (int)percentage;
            });

            try
            {
                this.Cursor = Cursors.WaitCursor;

                await _xmlStreamDispatcher.RunParsingPipelineAsync(_selectedFilesPaths, progressReporter, _parsingCancellationTokenSource.Token);

                MessageBox.Show("Импорт всего пакета файлов успешно завершен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                List<CadObjectSummary> summaryList = await _repository.GetObjectsSummaryAsync();
                GridCadastralObjects.DataSource = summaryList;

                _selectedFilesPaths.Clear();
                LabelFilesCount.Text = "Файлы не выбраны";
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Импорт был отменен пользователем.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                myProgressBar.Value = 0;
                this.Cursor = Cursors.Default;
            }
        }


        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            _parsingCancellationTokenSource?.Cancel();
        }

        private void ButtonSelectFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = FileDialogFilter;
                openFileDialog.Multiselect = true; 
                openFileDialog.Title = "Выберите файлы XML или ZIP-архивы для импорта";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedFilesPaths.Clear();
                    _selectedFilesPaths.AddRange(openFileDialog.FileNames);

                    LabelFilesCount.Text = $"Выбрано файлов для обработки: {_selectedFilesPaths.Count}";
                }
            }
        }

        private async void ButtonOpenSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm(_dbManager, _repository, _xmlStreamDispatcher))
            {
                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        await _dbManager.CheckAndInitializeSchemaAsync();

                        List<CadObjectSummary> summaryList = await _repository.GetObjectsSummaryAsync();
                        GridCadastralObjects.DataSource = summaryList;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка обновления данных после смены СУБД: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
}