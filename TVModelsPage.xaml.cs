using PartsManager.Data;
using PartsManager.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace PartsManager.Views
{
  public partial class TVModelsPage : ContentPage
  {
    private readonly Database _database;
    public ObservableCollection<TVModel> TVModels { get; set; }

    public TVModelsPage(Database database)
    {
      InitializeComponent();
      _database = database;

      TVModels = new ObservableCollection<TVModel>();
      BindingContext = this;

      LoadTVModels();
    }

    private void LoadTVModels()
    {
      TVModels.Clear();
      var models = _database.GetAllTVModels(); // Fetch data from the database
      foreach (var model in models)
      {
        TVModels.Add(model);
      }
    }

    private async void OnAddTVModelClicked(object sender, EventArgs e)
    {
      string name = await DisplayPromptAsync("New TV Model", "Enter TV Model Name:");
      if (!string.IsNullOrWhiteSpace(name))
      {
        _database.AddTVModel(name);
        LoadTVModels();
        await DisplayAlert("Success", "TV Model added!", "OK");
      }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
      if (sender is Button button && button.CommandParameter is int id)
      {
        bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this TV Model?", "Yes", "No");
        if (confirm)
        {
          _database.DeleteTVModel(id);
          LoadTVModels();
        }
      }
    }

    private async void OnManagePartsClicked(object sender, EventArgs e)
    {
      if (sender is Button button && button.CommandParameter is int tvModelId)
      {
        await Navigation.PushAsync(new ManagePartsPage(_database, tvModelId));
      }
    }
  }
}