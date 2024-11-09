using MariiaShkliaruk.TaskPlanner.Domain.Models;
using MariiaShkliaruk.TaskPlanner.Domain.Models.Enums;
using MariiaShkliaruk.TaskPlanner.Domain.Logic;
using MariiaShkliaruk.TaskPlanner.DataAccess;
using MariiaShkliaruk.TaskPlanner.DataAccess.Abstractions;

namespace MariiaShkliaruk.TaskPlanner.ConsoleRunner;

internal static class Program
{
  public static void Main(string[] args)
  {
    IWorkItemsRepository repository = new FileWorkItemsRepository();
    SimpleTaskPlanner planner = new SimpleTaskPlanner();
    string? choice;

    while (true)
    {
      Console.WriteLine("[A]dd work item");
      Console.WriteLine("[B]uild a plan");
      Console.WriteLine("[M]ark work item as completed");
      Console.WriteLine("[R]emove a work item");
      Console.WriteLine("[Q]uit the app");

      choice = Console.ReadLine()?.ToLower();

      if (choice == null)
      {
        continue;
      }

      switch (choice)
      {
        case "a":
          AddNewWorkItem(repository);
          break;

        case "b":
          GeneratePlan(repository, planner);
          break;

        case "m":
          CompleteWorkItem(repository);
          break;

        case "r":
          DeleteWorkItem(repository);
          break;

        case "q":
          return;

        default:
          Console.WriteLine("Неправильна команда.");
          break;
      }
    }
  }

  private static void CommitChanges(IWorkItemsRepository repository)
  {
    repository.SaveChanges();
  }

  private static void AddNewWorkItem(IWorkItemsRepository repository)
  {
    string? taskTitle;
    while (true)
    {
      Console.WriteLine("Назва завдання: ");
      taskTitle = Console.ReadLine();
      if (taskTitle != null) break;
      Console.WriteLine("Спробуйте ще раз.");
    }

    string? taskDescription;
    while (true)
    {
      Console.WriteLine("Опис завдання: ");
      taskDescription = Console.ReadLine();
      if (taskDescription != null) break;
      Console.WriteLine("Спробуйте ще раз.");
    }

    DateTime deadline;
    while (true)
    {
      Console.WriteLine("Дата завершення (формат: dd.MM.yyyy): ");
      if (DateTime.TryParse(Console.ReadLine(), out deadline)) break;
      Console.WriteLine("Неправильний формат дати.");
    }

    Priority taskPriority;
    while (true)
    {
      Console.WriteLine("Пріоритет (None, Low, Medium, High, Urgent): ");
      if (Enum.TryParse<Priority>(Console.ReadLine(), true, out taskPriority) && Enum.IsDefined(typeof(Priority), taskPriority)) break;
      Console.WriteLine("Неправильний пріоритет.");
    }

    Complexity taskComplexity;
    while (true)
    {
      Console.WriteLine("Складність (None, Minutes, Hours, Days, Weeks): ");
      if (Enum.TryParse<Complexity>(Console.ReadLine(), true, out taskComplexity) && Enum.IsDefined(typeof(Complexity), taskComplexity)) break;
      Console.WriteLine("Неправильна складність.");
    }

    WorkItem task = new WorkItem
    {
      Title = taskTitle,
      Description = taskDescription,
      CreationDate = DateTime.Now,
      DueDate = deadline,
      Priority = taskPriority,
      Complexity = taskComplexity,
      IsCompleted = false
    };

    repository.Add(task);
    CommitChanges(repository);
    Console.WriteLine("Завдання додано успішно.");
  }

  private static void GeneratePlan(IWorkItemsRepository repository, SimpleTaskPlanner planner)
  {
    WorkItem[] orderedTasks = planner.CreatePlan(repository);

    if (orderedTasks.Length == 0)
    {
      Console.WriteLine("Немає завдань для побудови плану.");
      return;
    }

    Console.WriteLine("\nСписок завдань:");
    foreach (WorkItem task in orderedTasks)
    {
      Console.WriteLine(task.ToString());
    }
  }

  private static void CompleteWorkItem(IWorkItemsRepository repository)
  {
    Console.WriteLine("ID завдання для позначення як виконаного:");
    Guid taskId;
    if (Guid.TryParse(Console.ReadLine(), out taskId))
    {
      WorkItem? task = repository.Get(taskId);
      if (task != null)
      {
        task.IsCompleted = true;
        repository.Update(task);
        CommitChanges(repository);
        Console.WriteLine("Завдання відзначено як виконане.");
      }
      else
      {
        Console.WriteLine("Завдання з таким ID не знайдено.");
      }
    }
    else
    {
      Console.WriteLine("Неправильний формат ID.");
    }
  }

  private static void DeleteWorkItem(IWorkItemsRepository repository)
  {
    Console.WriteLine("ID завдання для видалення:");
    Guid taskId;
    if (Guid.TryParse(Console.ReadLine(), out taskId))
    {
      bool isDeleted = repository.Remove(taskId);
      if (isDeleted)
      {
        CommitChanges(repository);
        Console.WriteLine("Завдання успішно видалено.");
      }
      else
      {
        Console.WriteLine("Завдання з таким ID не знайдено.");
      }
    }
    else
    {
      Console.WriteLine("Неправильний формат ID.");
    }
  }
}
