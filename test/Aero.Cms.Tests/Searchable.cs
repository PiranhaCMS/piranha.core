

using System.Text;
using Xunit;
using Aero.Cms.Extend;
using Aero.Cms.Extend.Blocks;
using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Tests;

[Collection("Integration tests")]
public class Searchable
{
    [Fact]
    public void SearchHtmlField()
    {
        var field = new HtmlField
        {
            Value = "<p>Lorem ipsum</p>"
        };
        var search = field.GetIndexedContent();

        Assert.Equal(field.Value, search);
    }

    [Fact]
    public void SearchMarkdownField()
    {
        var field = new MarkdownField
        {
            Value = "# Header"
        };
        var search = field.GetIndexedContent();

        Assert.Equal(field.ToHtml(), search);
    }

    [Fact]
    public void SearchStringField()
    {
        var field = new StringField
        {
            Value = "Lorem ipsum"
        };
        var search = field.GetIndexedContent();

        Assert.Equal(field.Value, search);
    }

    [Fact]
    public void SearchTextField()
    {
        var field = new TextField
        {
            Value = "Lorem ipsum"
        };
        var search = field.GetIndexedContent();

        Assert.Equal(field.Value, search);
    }

    [Fact]
    public void SearchColumnBlock()
    {
        var block = new ColumnBlock
        {
            Items = new List<Block>
            {
                new HtmlBlock
                {
                    Body = new HtmlField
                    {
                        Value = "<p>Lorem</p>"
                    }
                },
                new HtmlBlock
                {
                    Body = new HtmlField
                    {
                        Value = "<p>Ipsum</p>"
                    }
                }
            }
        };
        var sb = new StringBuilder();
        sb.AppendLine(((HtmlBlock)block.Items[0]).Body.Value);
        sb.AppendLine(((HtmlBlock)block.Items[1]).Body.Value);
        var value = sb.ToString();
        var search = block.GetIndexedContent();

        Assert.Equal(value, search);
    }

    [Fact]
    public void SearchHtmlBlock()
    {
        var block = new HtmlBlock
        {
            Body = new HtmlField
            {
                Value = "<p>Lorem ipsum</p>"
            }
        };
        var search = block.GetIndexedContent();

        Assert.Equal(block.Body.Value, search);
    }

    [Fact]
    public void SearchQuoteBlock()
    {
        var block = new QuoteBlock
        {
            Body = new TextField
            {
                Value = "Lorem ipsum"
            }
        };
        var search = block.GetIndexedContent();

        Assert.Equal(block.Body.Value, search);
    }

    [Fact]
    public void SearchTextBlock()
    {
        var block = new TextBlock
        {
            Body = new TextField
            {
                Value = "Lorem ipsum"
            }
        };
        var search = block.GetIndexedContent();

        Assert.Equal(block.Body.Value, search);
    }
}
