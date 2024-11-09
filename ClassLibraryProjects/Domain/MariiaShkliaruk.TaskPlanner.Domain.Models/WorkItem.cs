namespace MariiaShkliaruk.TaskPlanner.Domain.Models;

using System;
using MariiaShkliaruk.TaskPlanner.Domain.Models.Enums;

public class WorkItem : ICloneable
{
  public Guid Id { get; set; }
  public DateTime CreationDate { get; set; }
  public DateTime DueDate { get; set; }
  public Priority Priority { get; set; }
  public Complexity Complexity { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public bool IsCompleted { get; set; }

  public override string ToString()
  {
    return $"Id: {Id}, {Title}: due {DueDate:dd.MM.yyyy}, {Priority.ToString().ToLower()} priority";
  }

  public object Clone()
  {
    return this.MemberwiseClone();
  }
}

