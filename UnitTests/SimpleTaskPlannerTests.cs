using Moq;

using MariiaShkliaruk.TaskPlanner.Domain.Models;
using MariiaShkliaruk.TaskPlanner.Domain.Logic;
using MariiaShkliaruk.TaskPlanner.Domain.Models.Enums;
using MariiaShkliaruk.TaskPlanner.DataAccess.Abstractions;

namespace UnitTests
{
  public abstract class TestsBase : IDisposable
  {
    protected Mock<IWorkItemsRepository> mockIWorkItemsRepository;

    protected TestsBase()
    {
      WorkItem[] mockData = {
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Task Low Priority, Earlier Due Date",
                    Description = "Low priority task",
                    CreationDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(1),
                    Priority = Priority.Low,
                    Complexity = Complexity.Hours,
                    IsCompleted = false
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Task High Priority, Later Due Date",
                    Description = "High priority task",
                    CreationDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = Priority.High,
                    Complexity = Complexity.Days,
                    IsCompleted = false
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Task High Priority, Earlier Due Date",
                    Description = "High priority, but earlier due date",
                    CreationDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = Priority.High,
                    Complexity = Complexity.Days,
                    IsCompleted = false
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Task Low Priority, Completed",
                    Description = "Low priority, completed",
                    CreationDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = Priority.High,
                    Complexity = Complexity.Days,
                    IsCompleted = true
                }
            };

      mockIWorkItemsRepository = new Mock<IWorkItemsRepository>();
      mockIWorkItemsRepository.Setup(m => m.GetAll()).Returns(mockData);
    }

    public void Dispose() { }
  }

  public class SimpleTaskPlannerTests : TestsBase
  {
    [Fact]
    public void TestSimpleTaskPlannerSorting()
    {
      var taskPlanner = new SimpleTaskPlanner();
      var result = taskPlanner.CreatePlan(mockIWorkItemsRepository.Object);
      Console.WriteLine(result[0].ToString());
      Console.WriteLine(result[1].ToString());
      Console.WriteLine(result[2].ToString());

      Assert.NotNull(result);
      Assert.Equal(3, result.Length);

      Assert.Equal(Priority.High, result[0].Priority);
      Assert.Equal(Priority.High, result[1].Priority);
      Assert.Equal(Priority.Low, result[2].Priority);

      Assert.True(result[0].DueDate <= result[1].DueDate);
    }

    [Fact]
    public void TestSimpleTaskPlannerBuildsPlanWithoutCompletedTasks()
    {
      var taskPlanner = new SimpleTaskPlanner();
      var result = taskPlanner.CreatePlan(mockIWorkItemsRepository.Object);
      Console.WriteLine(result[0].ToString());
      Console.WriteLine(result[1].ToString());
      Console.WriteLine(result[2].ToString());

      Assert.NotNull(result);
      Assert.Equal(3, result.Length);
      Assert.All(result, task => Assert.False(task.IsCompleted));
    }
  }
}
