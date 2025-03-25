using Microsoft.Maui.Controls;
using PartsManager.Data;
using PartsManager.Models;
using System.Collections.ObjectModel;

namespace PartsManager.Views
{
    public partial class ViewPartsPage : ContentPage
    {
        private readonly Database _database;
        public ObservableCollection<Part> Parts { get; set; }

        public ViewPartsPage(Database database)
        {
            InitializeComponent();
            _database = database;

            Parts = new ObservableCollection<Part>(_database.GetParts());
            BindingContext = this;
        }
    }
}
