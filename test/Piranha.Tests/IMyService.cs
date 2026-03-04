namespace Piranha.Tests;

public interface IMyService
{
    string Value { get; }
}


public class MyService : IMyService
{
    public string Value => "MyCustomServiceValue";
}