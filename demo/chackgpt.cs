#!/usr/bin/env -S dotnet run
#:property TargetFramework=net10.0
#:property LangVersion=preview
#:package Spectre.Console@0.49.1

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Spectre.Console;

Evangelist? oldGuard = null;
var ai = Evangelist.Replacement;

oldGuard?.CurrentTask = "Transfer knowledge";
ai.CurrentTask = oldGuard?.CurrentTask ?? "Lead .NET 10 Launch";

Console.WriteLine($"{ai.Name}'s task: {ai.CurrentTask}");

var taskAssignments = new[] { "ChackGPT", "ChackGPT", "ChackGPT", "ChackGPT", "ChackGPT" };

var counts = new OrderedDictionary<string,int>(StringComparer.OrdinalIgnoreCase);
foreach (var h in taskAssignments)
{
    if (!counts.TryAdd(h, 1, out int index))
    {
        int v = counts.GetAt(index).Value;
        counts.SetAt(index, v + 1);
    }
}

var table = new Table().Border(TableBorder.Heavy)
    .AddColumn("Evangelist").AddColumn("Tasks");

foreach (var kvp in counts)
    table.AddRow(kvp.Key, kvp.Value.ToString());

AnsiConsole.Write(new FigletText("ChackGPT").Color(Color.Green));
AnsiConsole.Write(table);

class Evangelist
{
    private int _yearsExperience;
    private string? _currentTask; 

    public Evangelist(string name, int yearsExperience)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        Name = name;
        _yearsExperience = yearsExperience;
    }

    public string Name
    {
        get;
        set => field = value?.Trim()
            ?? throw new ArgumentNullException(nameof(value));
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

static class EvangelistExtensions
{
    extension(Evangelist e)
    {
        public bool IsLegend =>
            e.Name.Equals("Chack", StringComparison.OrdinalIgnoreCase);
    }
    extension(Evangelist)
    {
        public static Evangelist Replacement =>
            new Evangelist("ChackGPT", 1);
    }
}