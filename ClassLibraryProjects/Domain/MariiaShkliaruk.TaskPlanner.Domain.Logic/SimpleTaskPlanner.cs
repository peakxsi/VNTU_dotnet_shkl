using MariiaShkliaruk.TaskPlanner.Domain.Models;
using MariiaShkliaruk.TaskPlanner.DataAccess.Abstractions;

namespace MariiaShkliaruk.TaskPlanner.Domain.Logic;

public class SimpleTaskPlanner
{
  public WorkItem[] CreatePlan(IWorkItemsRepository repository)
  {
    List<WorkItem> tasksList = repository.GetAll().ToList();

    tasksList.Sort((task1, task2) =>
    {
      int priorityComparison = task2.Priority.CompareTo(task1.Priority);
      if (priorityComparison != 0)
        return priorityComparison;

      int dateComparison = task1.DueDate.CompareTo(task2.DueDate);
      if (dateComparison != 0)
        return dateComparison;

      return string.Compare(task1.Title, task2.Title, StringComparison.OrdinalIgnoreCase);
    });

    return tasksList.Where(task => !task.IsCompleted).ToArray();
  }
}
