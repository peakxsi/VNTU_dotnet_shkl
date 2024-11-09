namespace MariiaShkliaruk.TaskPlanner.DataAccess.Abstractions;

using System;
using MariiaShkliaruk.TaskPlanner.Domain.Models;

public interface IWorkItemsRepository
{
  Guid Add(WorkItem workItem);
  WorkItem? Get(Guid id);
  WorkItem[] GetAll();
  bool Update(WorkItem workItem);
  bool Remove(Guid id);
  void SaveChanges();
}
