namespace Telegami.Demo.Console;

class Person
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, LastName: {LastName}, Age: {Age}";
    }
}