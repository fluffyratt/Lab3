using System.Collections.ObjectModel;
using System.Reflection;

namespace Lab3
{
    public partial class MainPage : ContentPage
    {
        private string _filePath = "";
        private string _resultsFilePath = "";
        private bool _isError = false;
        private Dictionary<string, List<string>> _pickersData = new Dictionary<string, List<string>>
        {
            { "Name", new List<string>() },
            { "Starosta", new List<string>() },
            { "Department", new List<string>() },
            { "ClassMonitor", new List<string>() },
            { "CourseOrientation", new List<string>() },
            { "ManagerInfo", new List<string>() },
            { "TimeHandling", new List<string>() },
        };
        private ObservableCollection<StudentsClub> _deserializedData = new ObservableCollection<StudentsClub>();
        private ObservableCollection<StudentsClub> _dataToShow = new ObservableCollection<StudentsClub>();
        private LinqSearch _analizator = new LinqSearch();
        SearchCriteria criteria = new SearchCriteria();

        public MainPage()
        {
            InitializeComponent();
            ResultsListView.ItemsSource = _dataToShow;
        }

        private void AddPickerValue(StudentsClub club)
        {
            string[] selectedProperties = { "Name", "Starosta", "Department", "ClassMonitor", "CourseOrientation", "ManagerInfo", "TimeHandling", };

            foreach (string propertyName in selectedProperties)
            {
                PropertyInfo property = club.GetType().GetProperty(propertyName);
                var pickerList = _pickersData[propertyName];

                if (property != null)
                {
                    string propertyValue = property.GetValue(club) as string;
                    if (!string.IsNullOrEmpty(propertyValue) && !pickerList.Contains(propertyValue))
                    {
                        pickerList.Add(propertyValue);
                    }
                }
            }
        }

        private void ClearCriterias()
        {
            foreach (var list in _pickersData.Values)
            {
                list.Clear();
            }
        }

        private void ClearPickersValues()
        {
            namePicker.ItemsSource = null;
            departmentPicker.ItemsSource = null;
            starostaPicker.ItemsSource = null;
            timehandlingPicker.ItemsSource = null;
            classmonitorPicker.ItemsSource = null;
        }

        private void SortPickersValues()
        {
            foreach (var list in _pickersData.Values)
            {
                list.Sort();
            }
        }

        private void AddItemSourses()
        {
            SortPickersValues();
            namePicker.ItemsSource = _pickersData["Name"];
            departmentPicker.ItemsSource = _pickersData["Department"];
            starostaPicker.ItemsSource = _pickersData["Starosta"];
            timehandlingPicker.ItemsSource = _pickersData["TimeHandling"];
            classmonitorPicker.ItemsSource = _pickersData["ClassMonitor"];
        }

        private void FillPickers()
        {
            ClearCriterias();

            foreach (var club in _deserializedData)
            {
                AddPickerValue(club);
            }

            ClearPickersValues();
            AddItemSourses();
        }

        private void UpdateFilters()
        {
            namePicker.SelectedItem = null;
            departmentPicker.SelectedItem = null;
            starostaPicker.SelectedItem = null;
            timehandlingPicker.SelectedItem = null;
            classmonitorPicker.SelectedItem = null;
            _dataToShow.Clear();
            notFoundLabel.IsVisible = false;
        }

        private async Task<string> PickFile()
        {
            _isError = false;
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    string resultPath = result.FullPath;

                    if (File.Exists(resultPath))
                    {
                        string extension = Path.GetExtension(resultPath);
                        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            return resultPath;
                        }
                        else
                        {
                            _isError = true;
                            await DisplayAlert("Помилка", "Обраний файл не є JSON-файлом.", "ОК");
                        }
                    }
                    else
                    {
                        _isError = true;
                        await DisplayAlert("Помилка", "Обраного файлу не існує.", "ОК");
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _isError = true;
                await DisplayAlert("Помилка", $"{ex.Message}", "ОК");
                return string.Empty;
            }
        }

        private async void OnPickFileClicked(object sender, EventArgs e)
        {
            _filePath = await PickFile();

            if (!string.IsNullOrEmpty(_filePath) && !_isError)
            {
                FileInfo fileInfo = new FileInfo(_filePath);

                if (fileInfo.Length > 0)
                {
                    try
                    {
                        _deserializedData = JsonProcessor.Deserialize(_filePath);
                        UpdateFilters();
                        FillPickers();
                        filters.IsVisible = true;
                    }
                    catch (Exception ex)
                    {
                        _isError = true;
                        await DisplayAlert("Помилка", ex.Message, "ОК");
                    } 
                }
                else
                {
                    _isError = true;
                    await DisplayAlert("Помилка", "Файл пустий.", "ОК");
                }
            }
        }

        private async void SaveJsonBtnClicked(object sender, EventArgs e)
        {
            _resultsFilePath = await PickFile();

            if (!string.IsNullOrEmpty(_resultsFilePath) && !_isError)
            {
                try
                {
                    JsonProcessor.Serialize(_resultsFilePath, _dataToShow);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Помилка", ex.Message, "ОК");
                }

                await DisplayAlert("Інформація", "Результати збережені!", "ОК");
            }
        }

        private void OnCleanBtnClicked(object sender, EventArgs e)
        {
            UpdateFilters();
        }

        private SearchCriteria FormCriteria()
        {
            SearchCriteria newCriteria = new SearchCriteria();

            newCriteria.Name = namePicker.SelectedItem != null ? namePicker.SelectedItem as string : string.Empty;
            newCriteria.Department = departmentPicker.SelectedItem != null ? departmentPicker.SelectedItem as string : string.Empty;
            newCriteria.Starosta = starostaPicker.SelectedItem != null ? starostaPicker.SelectedItem as string : string.Empty;
            newCriteria.TimeHandling = timehandlingPicker.SelectedItem != null ? timehandlingPicker.SelectedItem as string : string.Empty;
            newCriteria.ClassMonitor = classmonitorPicker.SelectedItem != null ? classmonitorPicker.SelectedItem as string : string.Empty;

            return newCriteria;
        }

        private async void OnSearchBtnClicked(object sender, EventArgs e)
        {

            criteria = FormCriteria();

            try
            {
                _analizator.Search(criteria, _deserializedData, _dataToShow);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"{ex.Message}", "ОК");
            }

            if (_dataToShow.Count > 0 && !string.IsNullOrEmpty(_filePath))
            {
                ResultsContainer.IsVisible = true;
                notFoundLabel.IsVisible = false;
            }
            else
            {
                ResultsContainer.IsVisible = false;
                if (!string.IsNullOrEmpty(_filePath))
                {
                    notFoundLabel.IsVisible = true;
                }
            }

        }

        async private void OnHelpBtnClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Про програму",
                "Програма виконана: Хмельовою Діаною,\nКурс: 2,\nГрупа: К-27,\nДата виконання: 05.12.2023.\n\n" +
                "Програма розроблена для серіалізації/десеріалізації файлів JSON, відображення їх вмісту за допомогою .NET MAUI. " +
                "Наявна також можливість пошуку у списку, що складає вміст файлу, за певними критеріями, додавання елементів до списку, редагування елементів та їх видалення. " +
                "Можна зберегти результати пошуку у списку в JSON-форматі в окремий файл.",
                "ОК");
        }

        private void DeleteButtonClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            StudentsClub clubToDel = (StudentsClub)button.BindingContext;

            _dataToShow.Remove(clubToDel);
            _deserializedData.Remove(clubToDel);

            FillPickers();

            try
            {
                JsonProcessor.Serialize(_filePath, _deserializedData);
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", ex.Message, "ОК");
            }

        }

        private async void OnChangeBtnClicked(object sender, EventArgs e)
         {
             Button button = (Button)sender;
             StudentsClub selectedItem = (StudentsClub)button.BindingContext;
             if (selectedItem != null)
             {
                 var secondPage = new EditPage(selectedItem);
                 secondPage.DataModified += (s, modifiedData) =>
                 {
                     if (modifiedData != null)
                     {
                         ResultsListView.ItemsSource = null;

                         int idxInShowData = _dataToShow.IndexOf(selectedItem);
                         int idxInAllData = _dataToShow.IndexOf(selectedItem);

                         _dataToShow[idxInShowData] = modifiedData;
                         _deserializedData[idxInAllData] = modifiedData;

                         ResultsListView.ItemsSource = _dataToShow;
                         DisplayAlert("Інформація", "Зміни успішно внесені!", "ОК");
                         FillPickers();

                         try
                         {
                             JsonProcessor.Serialize(_filePath, _deserializedData);
                         }
                         catch (Exception ex)
                         {
                             DisplayAlert("Помилка", ex.Message, "ОК");
                         }
                     }
                     else
                     {
                         DisplayAlert("Помилка", "Внести зміни не вдалося.", "ОК");
                     }
                 };

                 await Navigation.PushModalAsync(secondPage);
             }
             else
             {
                 await DisplayAlert("Помилка", "Сталася помилка!.", "ОК");
             }
         }

        private async void OnAddElemBtnClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            StudentsClub newWork = new StudentsClub();

            var secondPage = new EditPage(newWork);
            secondPage.DataModified += (s, modifiedData) =>
            {
                if (modifiedData != null)
                {
                    _deserializedData.Insert(0, modifiedData);
                    _analizator.Search(criteria, _deserializedData, _dataToShow);

                    DisplayAlert("Інформація", "Новий студентський гурток додано!", "ОК");
                    FillPickers();

                    try
                    {
                        JsonProcessor.Serialize(_filePath, _deserializedData);
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Помилка", ex.Message, "ОК");
                    }
                }
                else
                {
                    DisplayAlert("Помилка", "Внести зміни не вдалося.", "ОК");
                }
            };

            await Navigation.PushModalAsync(secondPage);
        }
    }
}