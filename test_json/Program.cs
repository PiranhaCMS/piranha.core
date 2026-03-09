using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Piranha.Extend.Fields;
using Piranha.Models;

public class MyPage : Page<MyPage>
{
    [Piranha.Extend.Region]
    public MarkdownField Body { get; set; }
}

public class Program
{
    public static void Main()
    {
        var page = new MyPage { Body = new MarkdownField { Value = "test body" } };
        page.Blocks.Add(new Piranha.Extend.Blocks.TextBlock { Body = "my text block" });
        
        var typeInfoResolver = new DefaultJsonTypeInfoResolver();
        typeInfoResolver.Modifiers.Add(info =>
        {
            if (typeof(PageBase).IsAssignableFrom(info.Type))
            {
                info.PolymorphismOptions = null;
            }
        });

        var settings = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = typeInfoResolver
        };
        
        // Serialize as object
        var json = JsonSerializer.Serialize((object)page, settings);
        Console.WriteLine("JSON: " + json);
        
        // Deserialize as MyPage
        var clone = JsonSerializer.Deserialize<MyPage>(json, settings);
        Console.WriteLine($"Clone body value: {clone.Body?.Value ?? "null"}");
        Console.WriteLine($"Clone blocks count: {clone.Blocks?.Count ?? 0}");
        if (clone.Blocks?.Count > 0) {
            Console.WriteLine($"Clone block type: {clone.Blocks[0].GetType()}");
        }
    }
}