using Storygame.Client;

var url = new Uri("https://localhost:7121");
var client = new StorygameClient(url);

await client.Login();

await Task.Delay(-1);