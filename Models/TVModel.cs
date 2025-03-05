using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace PartsManager.Models
{
  public class TVModel
  {
    public int Id { get; set; }
    public string Name { get; set; } // TV model name

  }

  public class Part
  {
    public int Id { get; set; }
    public string PartNumber { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; }
    public string Name { get; internal set; }
  }

  // Relationship table for many-to-many mapping
  public class TVModelPart
  {
    public int Id { get; set; }
    public int TVModelId { get; set; }
    public int PartId { get; set; }
  }
}
