using analyticsService.Models;

namespace analyticsService.Repositories;

public class AnalyticsTasksRepository
{
    public static readonly AnalyticsTasksRepository Instance = new();

    private readonly List<AnalyticsTask> list = new();

    public void Add(AnalyticsTask analyticsTask)
    {
        list.Add(analyticsTask);
    }

    public void UpdatePrices(Guid taskId, decimal assignPrice, decimal completePrice)
    {
        var task = Find(taskId);
        if (task != null)
        {
            task.AssignPrice = assignPrice;
            task.CompletePrice = completePrice;
        }
        else
        {
            list.Add(new AnalyticsTask
            {
                TaskId = taskId,
                AssignPrice = assignPrice,
                CompletePrice = completePrice
            });
        }
    }

    public AnalyticsTask? Find(Guid taskId)
    {
        return list.FirstOrDefault(x => x.TaskId == taskId);
    }
}