using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace EventSchemaRegistry;

public static class SchemaRegistry
{
    static SchemaRegistry()
    {
        var path = Assembly.GetAssembly(typeof(SchemaRegistry))!.Location;
        var dir = Path.GetDirectoryName(path);
    }

    public static bool ValidateEvent(object eventObj, string schemaPath, int schemaVersion)
    {
        var dirPath = Path.Combine(@"C:\projects\TaskManager\TaskManager\EventSchemaRegistry\Schemas", schemaPath);
        Directory.CreateDirectory(dirPath);
        var fullPath = Path.Combine(dirPath, $"{schemaVersion}.json");
        if (File.Exists(fullPath))
        {
            var schema = JSchema.Parse(File.ReadAllText(fullPath));
            return JObject.FromObject(eventObj).IsValid(schema);
        }

        var generator = new JSchemaGenerator();
        var schemaNew = generator.Generate(eventObj.GetType());
        File.WriteAllText(fullPath, schemaNew.ToString());
        return true;
    }
}