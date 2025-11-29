# Chack → ChackGPT Modernization Steps  
C# 14 + .NET 10 + Spectre.Console + System.CommandLine  
All instructions combined into ONE markdown file.

---

## 1) Add shebang + file-based app properties

```diff
+ #!/usr/bin/env -S dotnet run
+ #:property TargetFramework=net10.0
+ #:property LangVersion=preview
```

```csharp
#!/usr/bin/env -S dotnet run
#:property TargetFramework=net10.0
#:property LangVersion=preview
```

## 2) Convert Name → C# 14 field-backed property

```diff
- private string _name;
...
- public string Name
- {
-     get { return _name; }
-     set { _name = value.Trim(); }
- }

+ public string Name
+ {
+     get;
+     set => field = value?.Trim()
+         ?? throw new ArgumentNullException(nameof(value));
+ }
```

```csharp
public string Name
{
    get;
    set => field = value?.Trim()
        ?? throw new ArgumentNullException(nameof(value));
}
```

## 3) Add extension members

```csharp
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
```

## 4) Add null-conditional assignment handoff logic

```csharp
Evangelist? oldGuard = null;
var ai = Evangelist.Replacement;

oldGuard?.CurrentTask = "Transfer knowledge";
ai.CurrentTask = oldGuard?.CurrentTask ?? "Lead .NET 10 Launch";

Console.WriteLine($"{ai.Name}'s task: {ai.CurrentTask}");
```

## 5) Upgrade Dictionary → OrderedDictionary (.NET 10)

```diff
- var counts = new Dictionary<string,int>();
- if (counts.TryGetValue(h, out var c)) counts[h] = c + 1;
- else counts[h] = 1;

+ using System.Collections.Specialized;
+ var counts = new OrderedDictionary<string,int>(StringComparer.OrdinalIgnoreCase);
+ if (!counts.TryAdd(h, 1, out int index))
+ {
+     int v = counts.GetAt(index).Value;
+     counts.SetAt(index, v + 1);
+ }
```

## 6) Add Spectre.Console + System.CommandLine packages

```diff
+ #:package Spectre.Console
+ #:package System.CommandLine@2.0.0
```

## 7) Replace Main with modern CLI + UI

```csharp
using System.CommandLine;
using Spectre.Console;

var root = new RootCommand("ChackGPT — evangelist summary");
var summary = new Option<bool>("--summary");
root.AddOption(summary);

root.SetHandler((bool summary) =>
{
    if (!summary)
    {
        AnsiConsole.Markup("[yellow]Use --summary to view results.[/]\n");
        return;
    }

    var table = new Table().Border(TableBorder.Heavy)
        .AddColumn("Evangelist").AddColumn("Tasks");

    foreach (var kvp in counts)
        table.AddRow(kvp.Key, kvp.Value.ToString());

    AnsiConsole.Write(new FigletText("ChackGPT").Color(Color.Green));
    AnsiConsole.Write(table);

}, summary);

await root.InvokeAsync(args);
```

## Run final version

```
./ChackGPT.cs --summary
```

OR

```
dotnet run ChackGPT.cs -- --summary
```

---