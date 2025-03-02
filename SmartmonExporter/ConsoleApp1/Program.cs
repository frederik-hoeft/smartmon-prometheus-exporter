using System.Text.Json;
using System.Text.Json.Serialization;

string json =
    """
    {
        "FirstName": "Foo",
        "LastName": "Bar"
    }
    """;

Person person = (Person)JsonSerializer.Deserialize(json, typeof(Person), new MyJsonSerializerContext())!;
Console.WriteLine(person);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(Person))]
internal partial class MyJsonSerializerContext : JsonSerializerContext;

internal record Person(string FirstName, string LastName);