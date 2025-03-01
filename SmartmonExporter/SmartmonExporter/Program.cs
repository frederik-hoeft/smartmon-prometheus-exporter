using ConsoleAppFramework;
using SmartmonExporter;

ConsoleApp.ConsoleAppBuilder app = ConsoleApp.Create();
app.Add<Commands>();
await app.RunAsync(args);