using System;
using System.Collections.Generic;

class Evangelist
{
    private string _name;
    private int _yearsExperience;
    private string? _currentTask;   // fixed only to avoid warning

    public Evangelist(string name, int yearsExperience)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        _name = name.Trim();
        _yearsExperience = yearsExperience;
    }

    public string Name
    {
        get { return _name; }
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _name = value.Trim();
        }
    }

    public int YearsExperience
    {
        get { return _yearsExperience; }
        set { _yearsExperience = value; }
    }

    public string? CurrentTask
    {
        get { return _currentTask; }
        set { _currentTask = value; }
    }
}

class Program
{
    static void Main()
    {
        var chack = new Evangelist("Chack ", 20);

        var taskAssignments = new[]
        {
            "Chack", "Chack", "ChackGPT", "Chack", "ChackGPT"
        };

        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var handler in taskAssignments)
        {
            int current;
            if (counts.TryGetValue(handler, out current))
            {
                counts[handler] = current + 1;
            }
            else
            {
                counts[handler] = 1;
            }
        }

        Console.WriteLine("===== Evangelist Task Summary =====");
        Console.WriteLine();

        Console.WriteLine("Evangelist        Tasks");
        Console.WriteLine("------------------------");

        foreach (var kvp in counts)
        {
            Console.WriteLine($"{kvp.Key,-16} {kvp.Value}");
        }
    }
}