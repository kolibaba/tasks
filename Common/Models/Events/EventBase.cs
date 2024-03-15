namespace Common.Models.Events;

public interface IEventBase
{
    Guid Id { get; set; }
    int Version { get; set; }
    DateTime Time { get; set; }
    string Name { get; set; }
}

public class EventBase<TData> : IEventBase
{
    public EventBase(string name, int version, TData data)
    {
        Id = Guid.NewGuid();
        Time = DateTime.Now;
        Version = version;
        Name = name;
        Data = data;
    }

    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTime Time { get; set; }
    public string Name { get; set; }
    public TData Data { get; set; }
}