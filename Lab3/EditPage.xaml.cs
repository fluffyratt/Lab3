using System.Collections.ObjectModel;

namespace Lab3
{
    public partial class EditPage : ContentPage
    {
       private StudentsClub _selectedItem;
        public event EventHandler <StudentsClub> DataModified;

        private void FillInputs()
        {
            nameInput.Text = _selectedItem.Name;
            timehandlingInput.Text = _selectedItem.TimeHandling;
            facultyInput.Text = _selectedItem.Faculty;
            departInput.Text = _selectedItem.Department;
            classmonInput.Text = _selectedItem.ClassMonitor;
            starostaInput.Text = _selectedItem.Starosta;
        }
        public EditPage(StudentsClub selected)
        {
            InitializeComponent();

            _selectedItem = selected;
            FillInputs();
        }

        
        private bool IsEmpty(string value) 
        {
            return value == string.Empty;
        }

        private bool ValidateAll()
        {
            return
                (
                        !IsEmpty(nameInput.Text) &&
                        !IsEmpty(timehandlingInput.Text) &&
                        !IsEmpty(facultyInput.Text)&&
                        !IsEmpty(departInput.Text)&&
                        !IsEmpty(classmonInput.Text)&&
                        !IsEmpty(starostaInput.Text)
                );
        }

        private void UpdateSelected()
        {
            _selectedItem.Name = nameInput.Text;
            _selectedItem.TimeHandling = timehandlingInput.Text;
            _selectedItem.Faculty = facultyInput.Text;
            _selectedItem.Department = departInput.Text;
            _selectedItem.ClassMonitor = classmonInput.Text;
            _selectedItem.Starosta = starostaInput.Text;
           
        }
        private void SaveButtonClicked(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                UpdateSelected();
                DataModified?.Invoke(this, _selectedItem);
                Navigation.PopModalAsync();
            }
            else
            {
                DisplayAlert("Помилка", "Деякі введення не валідні.", "ОК");
            }
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
                Navigation.PopModalAsync();
        }
    }
}
