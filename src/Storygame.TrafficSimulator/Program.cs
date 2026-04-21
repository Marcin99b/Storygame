using Storygame.Client;

var url = new Uri("https://localhost:7121");
var client = new StorygameClient(url);

await client.Login();
var me = await client.Me();
var catalog = await client.GetCatalog();

await Task.Delay(-1);