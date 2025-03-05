using PartsManager.Data;
using PartsManager.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace PartsManager.Views
{
  public partial class ManagePartsPage : ContentPage
  {
    private readonly Database _database;
    private readonly int _tvModelId;

    public ObservableCollection<Part> AvailableParts { get; set; }
    public ObservableCollection<Part> LinkedParts { get; set; }

    public ManagePartsPage(Database database, int tvModelId)
    {
      InitializeComponent();
      _database = database;
      _tvModelId = tvModelId;

      AvailableParts = new ObservableCollection<Part>((IEnumerable<Part>)_database.GetParts());
      LinkedParts = new ObservableCollection<Part>(_database.GetPartsForTVModel(tvModelId));

      BindingContext = this;
    }

    private void OnAddPartClicked(object sender, EventArgs e)
    {
      if (sender is Button button && button.CommandParameter is int partId)
      {
        _database.LinkPartToTVModel(_tvModelId, partId);
        RefreshParts();
      }
    }

    private void OnRemovePartClicked(object sender, EventArgs e)
    {
      if (sender is Button button && button.CommandParameter is int partId)
      {
        _database.UnlinkPartFromTVModel(_tvModelId, partId);
        RefreshParts();
      }
    }

    private void RefreshParts()
    {
      LinkedParts.Clear();
      foreach (var part in _database.GetPartsForTVModel(_tvModelId))
      {
        LinkedParts.Add(part);
      }
    }
  }
}