
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal class LinqSearch
    {
        private bool IsValidPickerValue(string workValue, string criteriaValue) // параметри фільтрації
        {
            return string.IsNullOrEmpty(criteriaValue) || workValue.Equals(criteriaValue);
        }

        public void Search(SearchCriteria criteria, ObservableCollection <StudentsClub> data, ObservableCollection <StudentsClub> results)
        {
            var clubs = (from club in data
                         where (
                          IsValidPickerValue(club.Name, criteria.Name) &&
                          IsValidPickerValue(club.TimeHandling, criteria.TimeHandling) &&
                          IsValidPickerValue(club.Faculty, criteria.Faculty) &&
                          IsValidPickerValue(club.Department, criteria.Department) &&
                          IsValidPickerValue(club.ClassMonitor, criteria.ClassMonitor) &&
                          IsValidPickerValue(club.Starosta, criteria.Starosta)
                         )
                         select club).ToList();

            results.Clear();
            foreach (StudentsClub work in clubs)
            {
                results.Add(work);
            }
        }
    }
}
