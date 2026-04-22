var url = new Uri("https://localhost:7121");


var tasks = new List<Task>();
Console.WriteLine("Starting scenarios");
tasks.AddRange(GenerateScenarios(30, Scenarios.SimpleScenario));
tasks.AddRange(GenerateScenarios(10, Scenarios.BrowsingAndAddScenario));
tasks.AddRange(GenerateScenarios(15, Scenarios.AudiobookListeningScenario));
tasks.AddRange(GenerateScenarios(5, Scenarios.BingeReaderScenario));
tasks.AddRange(GenerateScenarios(40, Scenarios.CasualSamplerScenario));
tasks.AddRange(GenerateScenarios(80, Scenarios.AbandonScenario));
Console.WriteLine("All scenarios started");
await Task.WhenAll(tasks);
Console.WriteLine("All scenarios finished");

IEnumerable<Task> GenerateScenarios(int count, Func<Uri, Task> scenario)
    => Enumerable.Range(0, count).Select(_ => scenario(url));