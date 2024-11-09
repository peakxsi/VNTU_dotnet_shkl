namespace MariiaShkliaruk.TaskPlanner.DataAccess;

using System;
using System.Collections.Generic;
using System.IO;
using MariiaShkliaruk.TaskPlanner.DataAccess.Abstractions;
using MariiaShkliaruk.TaskPlanner.Domain.Models;
using Newtonsoft.Json;

public class FileWorkItemsRepository : IWorkItemsRepository
{
  private const string FilePath = "work-items.json";
  private readonly Dictionary<Guid, WorkItem> _itemsCollection = new Dictionary<Guid, WorkItem>();

  public FileWorkItemsRepository()
  {
    if (File.Exists(FilePath) && new FileInfo(FilePath).Length > 0)
    {
      string fileContent = File.ReadAllText(FilePath);
      var itemsArray = JsonConvert.DeserializeObject<WorkItem[]>(fileContent);

      if (itemsArray == null)
      {
        return;
      }

      foreach (var item in itemsArray)
      {
        _itemsCollection.Add(item.Id, item);
      }
    }
    else
    {
      _itemsCollection = new Dictionary<Guid, WorkItem>();
    }
  }

  public Guid Add(WorkItem item)
  {
    var clonedItem = (WorkItem)item.Clone();
    clonedItem.Id = Guid.NewGuid();
    clonedItem.CreationDate = DateTime.Now;

    _itemsCollection.Add(clonedItem.Id, clonedItem);

    return clonedItem.Id;
  }

  public WorkItem? Get(Guid itemId)
  {
    return _itemsCollection.TryGetValue(itemId, out var item) ? item : null;
  }

  public WorkItem[] GetAll()
  {
    return new List<WorkItem>(_itemsCollection.Values).ToArray();
  }

  public bool Update(WorkItem item)
  {
    if (_itemsCollection.ContainsKey(item.Id))
    {
      _itemsCollection[item.Id] = item;
      return true;
    }
    return false;
  }

  public bool Remove(Guid itemId)
  {
    return _itemsCollection.Remove(itemId);
  }

  public void SaveChanges()
  {
    var itemsArray = new List<WorkItem>(_itemsCollection.Values);
    string fileContent = JsonConvert.SerializeObject(itemsArray, Formatting.Indented);
    File.WriteAllText(FilePath, fileContent);
  }
}
