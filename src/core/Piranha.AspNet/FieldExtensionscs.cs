using Microsoft.AspNetCore.Html;

public static class FieldExtensionscs
{
    public static HtmlString Html(this Piranha.Extend.Fields.HtmlField field) {
        return new HtmlString(field.Value);
    }
}
