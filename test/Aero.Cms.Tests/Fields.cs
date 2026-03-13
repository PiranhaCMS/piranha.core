using Aero.Cms.Extend;
using Aero.Cms.Models;
using Aero.Cms.Data.Repositories;
using Aero.Cms.Data.Services.Internal;
using Aero.Cms.Runtime;
using Aero.Cms.Services;

namespace Aero.Cms.Tests;

public class Fields(MartenFixture fixture) : AsyncTestBase(fixture)
{
    [FieldType(Name = "First", Shorthand = "1")]
    public class MyFirstField : Extend.Fields.SimpleField<string> { }

    [FieldType(Name = "Second")]
    public class MySecondField : Extend.Fields.SimpleField<string> { }

    [FieldType(Name = "Third")]
    public class MyThirdField : Extend.Fields.SimpleField<string> { }

    private readonly AppFieldList fields = new AppFieldList();

    public enum MyEnum {
        Value1,
        Value2
    }

    /// <summary>
    /// Sets up & initializes the tests.
    /// </summary>
    protected override void Init()
    {
        
        Aero.Cms.App.Init(api);
        Aero.Cms.App.Fields.Register<MyFirstField>();

        fields.Register<MyFirstField>();
        fields.Register<MyThirdField>();
    }

    /// <summary>
    /// Cleans up any possible data and resources
    /// created by the test.
    /// </summary>
    protected override void Cleanup() { }

    [Fact]
    public void RegisterSelect()
    {
        Aero.Cms.App.Fields.RegisterSelect<MyEnum>();
        fields.RegisterSelect<MyEnum>();
    }

    [Fact]
    public void GetByType() {
        var field = Aero.Cms.App.Fields.GetByType(typeof(Extend.Fields.HtmlField));

        Assert.NotNull(field);
        Assert.Equal("Html", field.Name);
    }

    [Fact]
    public void GetByTypeLocal() {
        var field = fields.GetByType(typeof(MyFirstField));

        Assert.NotNull(field);
        Assert.Equal("First", field.Name);
    }

    [Fact]
    public void GetByTypeName() {
        var field = Aero.Cms.App.Fields.GetByType(typeof(Extend.Fields.HtmlField).FullName);

        Assert.NotNull(field);
        Assert.Equal("Html", field.Name);
    }

    [Fact]
    public void GetByTypeNameLocal() {
        var field = fields.GetByType(typeof(MyFirstField).FullName);

        Assert.NotNull(field);
        Assert.Equal("First", field.Name);
    }

    [Fact]
    public void GetByShorthand() {
        var field = Aero.Cms.App.Fields.GetByShorthand("Html");

        Assert.NotNull(field);
        Assert.Equal("Html", field.Name);
    }

    [Fact]
    public void GetByShorthandLocal() {
        var field = fields.GetByShorthand("1");

        Assert.NotNull(field);
        Assert.Equal("First", field.Name);
    }

    [Fact]
    public void Register() {
        var count = Aero.Cms.App.Fields.Count();

        Aero.Cms.App.Fields.Register<MySecondField>();

        Assert.Equal(count + 1, Aero.Cms.App.Fields.Count());
    }

    [Fact]
    public void RegisterLocal() {
        var count = fields.Count();

        fields.Register<MySecondField>();

        Assert.Equal(count + 1, fields.Count());
    }

    [Fact]
    public void UnRegister() {
        var count = Aero.Cms.App.Fields.Count();

        Aero.Cms.App.Fields.UnRegister<MyFirstField>();

        Assert.Equal(count - 1, Aero.Cms.App.Fields.Count());
    }

    [Fact]
    public void UnRegisterLocal() {
        var count = fields.Count();

        fields.UnRegister<MyThirdField>();

        Assert.Equal(count - 1, fields.Count());
    }

    [Fact]
    public void Enumerate() {
        foreach (var field in Aero.Cms.App.Fields) {
            Assert.NotNull(field);
        }
    }

    [Fact]
    public void EnumerateLocal() {
        foreach (var field in fields) {
            Assert.NotNull(field);
        }
    }

    [Fact]
    public void CheckBoxFieldConversions()
    {
        var value = true;

        Aero.Cms.Extend.Fields.CheckBoxField field = value;

        Assert.Equal(value, field.Value);
    }

    [Fact]
    public void CheckBoxFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.CheckBoxField {
            Value = true
        };
        var field2 = new Aero.Cms.Extend.Fields.CheckBoxField {
            Value = true
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void CheckBoxFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.CheckBoxField {
            Value = true
        };
        var field2 = new Aero.Cms.Extend.Fields.CheckBoxField {
            Value = false
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void ColorFieldConversions()
    {
        var inStr = "#007eaa";

        Aero.Cms.Extend.Fields.ColorField field = inStr;
        Assert.Equal(inStr, field.Value);

        string outStr = field;
        Assert.Equal(inStr, outStr);
    }

    [Fact]
    public void ColorFieldEquals()
    {
        var field1 = new Aero.Cms.Extend.Fields.ColorField
        {
            Value = "#007eaa"
        };
        var field2 = new Aero.Cms.Extend.Fields.ColorField
        {
            Value = "#007eaa"
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void ColorFieldNotEquals()
    {
        var field1 = new Aero.Cms.Extend.Fields.ColorField
        {
            Value = "#007eaa"
        };
        var field2 = new Aero.Cms.Extend.Fields.ColorField
        {
            Value = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void DateFieldConversions() {
        var now = DateTime.Now;

        Aero.Cms.Extend.Fields.DateField field = now;
        Assert.Equal(now, field.Value);
    }

    [Fact]
    public void DateFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.DateField {
            Value = new DateTime(2018, 1, 1)
        };
        var field2 = new Aero.Cms.Extend.Fields.DateField {
            Value = new DateTime(2018, 1, 1)
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void DateFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.DateField {
            Value = new DateTime(2018, 1, 1)
        };
        var field2 = new Aero.Cms.Extend.Fields.DateField {
            Value = new DateTime(2017, 1, 1)
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void HtmlFieldConversions() {
        var str = "<p>This is a paragraph</p>";

        Aero.Cms.Extend.Fields.HtmlField field = str;
        Assert.Equal(str, field.Value);

        string output = field;
        Assert.Equal(str, output);
    }

    [Fact]
    public void HtmlFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.HtmlField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.HtmlField {
            Value = "Sollicitudin Justo Tristique"
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void HtmlFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.HtmlField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.HtmlField {
            Value = "Sollicitudin Tristique"
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void HtmlFieldTitle() {
        Aero.Cms.Extend.Fields.HtmlField field = "<p>Html value</p>";

        Assert.Equal("Html value", field.GetTitle());
    }

    [Fact]
    public void HtmlFieldTitleMaxLength() {
        var sb = new StringBuilder();
        for (var n = 0; n < 10; n++) {
            sb.Append("NineChars");
        }

        Aero.Cms.Extend.Fields.HtmlField field = sb.ToString();
        Assert.Equal(43, field.GetTitle().Length);
    }

    [Fact]
    public void ImageFieldConversions() {
        var media = new Media
        {
            Id = Snowflake.NewId()
        };

        Aero.Cms.Extend.Fields.ImageField field = media;
        Assert.Equal(media.Id, field.Id);
    }

    [Fact]
    public async Task ImageFieldInitMissing()
    {
        
        var field = new Aero.Cms.Extend.Fields.ImageField {
            Id = Snowflake.NewId()
        };

        await field.Init(api);

        Assert.Null(field.Id);
    }

    [Fact]
    public void ImageFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.ImageField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.ImageField {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void ImageFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.ImageField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.ImageField {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void DocumentFieldConversions() {
        var media = new Media
        {
            Id = Snowflake.NewId()
        };

        Aero.Cms.Extend.Fields.DocumentField field = media;
        Assert.Equal(media.Id, field.Id);
    }

    [Fact]
    public async Task DocumentFieldInitMissing()
    {
        
        var field = new Aero.Cms.Extend.Fields.DocumentField {
            Id = Snowflake.NewId()
        };

        await field.Init(api);

        Assert.Null(field.Id);
    }

    [Fact]
    public void DocumentFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.DocumentField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.DocumentField {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void DocumentFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.DocumentField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.DocumentField {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void VideoFieldConversions() {
        var media = new Media
        {
            Id = Snowflake.NewId()
        };

        Aero.Cms.Extend.Fields.VideoField field = media;
        Assert.Equal(media.Id, field.Id);
    }

    [Fact]
    public async Task VideoFieldInitMissing()
    {
        
        var field = new Aero.Cms.Extend.Fields.VideoField {
            Id = Snowflake.NewId()
        };

        await field.Init(api);

        Assert.Null(field.Id);
    }

    [Fact]
    public void VideoFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.VideoField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.VideoField {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void VideoFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.VideoField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.VideoField {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void AudioFieldConversions()
    {
        var media = new Media
        {
            Id = Snowflake.NewId()
        };

        Aero.Cms.Extend.Fields.AudioField field = media;
        Assert.Equal(media.Id, field.Id);
    }

    [Fact]
    public async Task AudioFieldInitMissing()
    {
        
        var field = new Aero.Cms.Extend.Fields.AudioField
        {
            Id = Snowflake.NewId()
        };

        await field.Init(api);

        Assert.Null(field.Id);
    }

    [Fact]
    public void AudioFieldEquals()
    {
        var field1 = new Aero.Cms.Extend.Fields.AudioField
        {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.AudioField
        {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void AudioFieldNotEquals()
    {
        var field1 = new Aero.Cms.Extend.Fields.AudioField
        {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.AudioField
        {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void MediaFieldConversions() {
        var media = new Media
        {
            Id = Snowflake.NewId()
        };

        Aero.Cms.Extend.Fields.MediaField field = media;
        Assert.Equal(media.Id, field.Id);
    }

    [Fact]
    public async Task MediaFieldInitMissing()
    {
        
        var field = new Aero.Cms.Extend.Fields.MediaField {
            Id = Snowflake.NewId()
        };

        await field.Init(api);

        Assert.Null(field.Id);
    }

    [Fact]
    public void MediaFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.MediaField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.MediaField {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void MediaFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.MediaField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.MediaField {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void ImageFieldConversionsNullImage() {
        var id = Snowflake.NewId();

        Aero.Cms.Extend.Fields.ImageField field = id;
        Assert.Equal(id, field.Id);

        string url = field;
        Assert.Equal("", url);
    }

    [Fact]
    public void DocumentFieldConversionsNullDocument() {
        var id = Snowflake.NewId();

        Aero.Cms.Extend.Fields.DocumentField field = id;
        Assert.Equal(id, field.Id);

        string url = field;
        Assert.Equal("", url);
    }

    [Fact]
    public void VideoFieldConversionsNullVideo() {
        var id = Snowflake.NewId();

        Aero.Cms.Extend.Fields.VideoField field = id;
        Assert.Equal(id, field.Id);

        string url = field;
        Assert.Equal("", url);
    }

    [Fact]
    public void AudioFieldConversionsNullAudio()
    {
        var id = Snowflake.NewId();

        Aero.Cms.Extend.Fields.AudioField field = id;
        Assert.Equal(id, field.Id);

        string url = field;
        Assert.Equal("", url);
    }

    [Fact]
    public void MediaFieldConversionsNullMedia() {
        var id = Snowflake.NewId();

        Aero.Cms.Extend.Fields.MediaField field = id;
        Assert.Equal(id, field.Id);

        string url = field;
        Assert.Equal("", url);
    }

    [Fact]
    public void MarkdownFieldConversions() {
        var md = "This is a paragraph";
        var html = Aero.Cms.App.Markdown.Transform(md);

        Aero.Cms.Extend.Fields.MarkdownField field = md;
        Assert.Equal(md, field.Value);

        string outputHtml = field;
        Assert.Equal(html, outputHtml);
    }

    [Fact]
    public void MarkdownFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.MarkdownField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.MarkdownField {
            Value = "Sollicitudin Justo Tristique"
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void MarkdownFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.MarkdownField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.MarkdownField {
            Value = "Sollicitudin Tristique"
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void NumberFieldConversions() {
        var number = 25;

        Aero.Cms.Extend.Fields.NumberField field = number;
        Assert.Equal(number, field.Value);

        int? converted = field;
        Assert.True(converted.HasValue);
        Assert.Equal(25, converted.Value);
    }

    [Fact]
    public void NumberFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.NumberField {
            Value = 23
        };
        var field2 = new Aero.Cms.Extend.Fields.NumberField {
            Value = 23
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void NumberFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.NumberField {
            Value = 23
        };
        var field2 = new Aero.Cms.Extend.Fields.NumberField {
            Value = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void PageFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.PageField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.PageField {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void PageFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.PageField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.PageField {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void PostFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.PostField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.PostField {
            Id = field1.Id
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void PostFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.PostField {
            Id = Snowflake.NewId()
        };
        var field2 = new Aero.Cms.Extend.Fields.PostField {
            Id = null
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void SelectFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.SelectField<MyEnum> {
            Value = MyEnum.Value1
        };
        var field2 = new Aero.Cms.Extend.Fields.SelectField<MyEnum> {
            Value = MyEnum.Value1
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void SelectFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.SelectField<MyEnum> {
            Value = MyEnum.Value1
        };
        var field2 = new Aero.Cms.Extend.Fields.SelectField<MyEnum> {
            Value = MyEnum.Value2
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void StringFieldConversions() {
        var inStr = "This is a string";

        Aero.Cms.Extend.Fields.StringField field = inStr;
        Assert.Equal(inStr, field.Value);

        string outStr = field;
        Assert.Equal(inStr, outStr);
    }

    [Fact]
    public void StringFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.StringField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.StringField {
            Value = "Sollicitudin Justo Tristique"
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void StringFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.StringField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.StringField {
            Value = "Sollicitudin Tristique"
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void TextFieldConversions() {
        var inStr = "This is a string";

        Aero.Cms.Extend.Fields.TextField field = inStr;
        Assert.Equal(inStr, field.Value);

        string outStr = field;
        Assert.Equal(inStr, outStr);
    }

    [Fact]
    public void TextFieldEquals() {
        var field1 = new Aero.Cms.Extend.Fields.TextField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.TextField {
            Value = "Sollicitudin Justo Tristique"
        };

        Assert.True(field1 == field2);
        Assert.True(field1.Equals(field2));
        Assert.True(field1.Equals((object)field2));
    }

    [Fact]
    public void TextFieldNotEquals() {
        var field1 = new Aero.Cms.Extend.Fields.TextField {
            Value = "Sollicitudin Justo Tristique"
        };
        var field2 = new Aero.Cms.Extend.Fields.TextField {
            Value = "Sollicitudin Tristique"
        };

        Assert.True(field1 != field2);
        Assert.True(!field1.Equals(field2));
        Assert.True(!field1.Equals((object)field2));
    }

    [Fact]
    public void GetFieldTitleNull() {
        var field = new Aero.Cms.Extend.Fields.TextField();

        Assert.Null(field.GetTitle());
    }

    [Fact]
    public void GetFieldTitle() {
        Aero.Cms.Extend.Fields.TextField field = "String value";

        Assert.Equal("String value", field.GetTitle());
    }

    [Fact]
    public void GetFieldTitleMaxLength() {
        var sb = new StringBuilder();
        for (var n = 0; n < 10; n++) {
            sb.Append("NineChars");
        }

        Aero.Cms.Extend.Fields.TextField field = sb.ToString();

        Assert.Equal(43, field.GetTitle().Length);
    }

    [Fact]
    public void ReadonlyFieldConversions()
    {
        var value = "Value";

        Aero.Cms.Extend.Fields.ReadonlyField field = value;
        Assert.Equal(value, field.Value);

        string value2 = field;
        Assert.Equal(value, value2);
    }

    private IApi CreateApi()
    {
        var factory = new ContentFactory(services);
        var serviceFactory = new ContentServiceFactory(factory);

        var db = GetDb();

        return new Api(
            factory,
            new AliasRepository(db),
            new ArchiveRepository(db),
            new ContentRepository(db, serviceFactory),
            new ContentGroupRepository(db),
            new ContentTypeRepository(db),
            new LanguageRepository(db),
            new MediaRepository(db),
            new PageRepository(db, serviceFactory),
            new PageTypeRepository(db),
            new ParamRepository(db),
            new PostRepository(db, serviceFactory),
            new PostTypeRepository(db),
            new SiteRepository(db, serviceFactory),
            new SiteTypeRepository(db)
        );
    }
}
