using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using PartsManager.Data;

namespace PartsManager
{
  public partial class MainPage : ContentPage
  {
    private readonly Database _database;

    public MainPage()
    {
      InitializeComponent();
      string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "inventory.db");
      _database = new Database(dbPath);
    }

    private async void OnAddTVModelClicked(object sender, EventArgs e)
    {
      string name = await DisplayPromptAsync("New TV Model", "Enter TV Model Name:");
      if (!string.IsNullOrWhiteSpace(name))
      {
        _database.AddTVModel(name);
        await DisplayAlert("Success", "TV Model added!", "OK");
      }
    }

    private async void OnViewTVModelsClicked(object sender, EventArgs e)
    {
      var tvModels = _database.GetTVModels();
      await DisplayAlert("TV Models", string.Join("\n", tvModels), "OK");
    }

    private async void OnAddPartClicked(object sender, EventArgs e)
    {
      string partNumber = await DisplayPromptAsync("New Part", "Enter Part Number:");
      string name = await DisplayPromptAsync("New Part", "Enter Part Name:");
      string quantityStr = await DisplayPromptAsync("New Part", "Enter Quantity:");
      string location = await DisplayPromptAsync("New Part", "Enter Location:");

      if (int.TryParse(quantityStr, out int quantity) &&
          !string.IsNullOrWhiteSpace(partNumber) &&
          !string.IsNullOrWhiteSpace(name) &&
          !string.IsNullOrWhiteSpace(location))
      {
        _database.AddPart(partNumber, name, quantity, location);
        await DisplayAlert("Success", "Part added!", "OK");
      }
    }

    private async void OnViewPartsClicked(object sender, EventArgs e)
    {
      var parts = _database.GetParts();
      await DisplayAlert("Parts", string.Join("\n", parts), "OK");
    }
  }
}

