var url = new Uri("https://localhost:7121");


Console.WriteLine("Starting scenarios");
var tasks = new List<IEnumerable<Task>>()
{
    GenerateScenarios(30, Scenarios.SimpleScenario),
    GenerateScenarios(10, Scenarios.BrowsingAndAddScenario),
    GenerateScenarios(15, Scenarios.AudiobookListeningScenario),
    GenerateScenarios(5, Scenarios.BingeReaderScenario),
    GenerateScenarios(40, Scenarios.CasualSamplerScenario),
    GenerateScenarios(80, Scenarios.AbandonScenario)
}.SelectMany(x => x);
Console.WriteLine("All scenarios started");
await Task.WhenAll(tasks);
Console.WriteLine("All scenarios finished");

IEnumerable<Task> GenerateScenarios(int count, Func<Uri, Task> scenario)
    => Enumerable.Range(0, count).Select(_ => scenario(url));