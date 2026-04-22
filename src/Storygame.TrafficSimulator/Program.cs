var url = new Uri("https://localhost:7121");

var tasks = Enumerable.Range(0, 30)
    .Select(_ => Scenarios.SimpleScenario(url));
await Task.WhenAll(tasks);
