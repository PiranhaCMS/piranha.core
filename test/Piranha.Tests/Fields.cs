/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Extend;
using Piranha.Runtime;
using Piranha.Services;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Piranha.Tests
{
    public class Fields : BaseTests
    {
        [FieldType(Name = "First", Shorthand = "1")]
        public class MyFirstField : Extend.Fields.SimpleField<string> { }

        [FieldType(Name = "Second")]
        public class MySecondField : Extend.Fields.SimpleField<string> { }

        [FieldType(Name = "Third")]
        public class MyThirdField : Extend.Fields.SimpleField<string> { }

        private AppFieldList fields = new AppFieldList();

        public enum MyEnum {
            Value1,
            Value2
        }

        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            Piranha.App.Init();
            Piranha.App.Fields.Register<MyFirstField>();

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
            Piranha.App.Fields.RegisterSelect<MyEnum>();
            fields.RegisterSelect<MyEnum>();
        }

        [Fact]
        public void GetByType() {
            var field = Piranha.App.Fields.GetByType(typeof(Extend.Fields.HtmlField));

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
            var field = Piranha.App.Fields.GetByType(typeof(Extend.Fields.HtmlField).FullName);

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
            var field = Piranha.App.Fields.GetByShorthand("Html");

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
            var count = Piranha.App.Fields.Count();

            Piranha.App.Fields.Register<MySecondField>();

            Assert.Equal(count + 1, Piranha.App.Fields.Count());
        }

        [Fact]
        public void RegisterLocal() {
            var count = fields.Count();

            fields.Register<MySecondField>();

            Assert.Equal(count + 1, fields.Count());
        }

        [Fact]
        public void UnRegister() {
            var count = Piranha.App.Fields.Count();

            Piranha.App.Fields.UnRegister<MyFirstField>();

            Assert.Equal(count - 1, Piranha.App.Fields.Count());
        }

        [Fact]
        public void UnRegisterLocal() {
            var count = fields.Count();

            fields.UnRegister<MyThirdField>();

            Assert.Equal(count - 1, fields.Count());
        }

        [Fact]
        public void Enumerate() {
            foreach (var field in Piranha.App.Fields) {
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

            Piranha.Extend.Fields.CheckBoxField field = value;
            Assert.Equal(value, field.Value);
        }

        [Fact]
        public void CheckBoxFieldEquals() {
            var field1 = new Piranha.Extend.Fields.CheckBoxField { 
                Value = true
            };
            var field2 = new Piranha.Extend.Fields.CheckBoxField { 
                Value = true
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void CheckBoxFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.CheckBoxField { 
                Value = true
            };
            var field2 = new Piranha.Extend.Fields.CheckBoxField { 
                Value = false
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }        

        [Fact]
        public void DateFieldConversions() {
            var now = DateTime.Now;

            Piranha.Extend.Fields.DateField field = now;
            Assert.Equal(now, field.Value);
        }

        [Fact]
        public void DateFieldEquals() {
            var field1 = new Piranha.Extend.Fields.DateField { 
                Value = new DateTime(2018, 1, 1)
            };
            var field2 = new Piranha.Extend.Fields.DateField { 
                Value = new DateTime(2018, 1, 1)
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void DateFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.DateField { 
                Value = new DateTime(2018, 1, 1)
            };
            var field2 = new Piranha.Extend.Fields.DateField { 
                Value = new DateTime(2017, 1, 1)
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }        

        [Fact]
        public void HtmlFieldConversions() {
            var str = "<p>This is a paragraph</p>";

            Piranha.Extend.Fields.HtmlField field = str;
            Assert.Equal(str, field.Value);

            string output = field;
            Assert.Equal(str, output);
        }

        [Fact]
        public void HtmlFieldEquals() {
            var field1 = new Piranha.Extend.Fields.HtmlField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.HtmlField { 
                Value = "Sollicitudin Justo Tristique"
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void HtmlFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.HtmlField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.HtmlField { 
                Value = "Sollicitudin Tristique"
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void ImageFieldConversions() {
            var media = new Data.Media() {
                Id = Guid.NewGuid()
            };

            Piranha.Extend.Fields.ImageField field = media;
            Assert.Equal(media.Id, field.Id.Value);
        }

        [Fact]
        public void ImageFieldInitMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var field = new Piranha.Extend.Fields.ImageField {
                    Id = Guid.NewGuid()
                };

                field.Init(api);

                Assert.Null(field.Id);
            }
        }

        [Fact]
        public void ImageFieldEquals() {
            var field1 = new Piranha.Extend.Fields.ImageField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.ImageField { 
                Id = field1.Id
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void ImageFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.ImageField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.ImageField { 
                Id = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void DocumentFieldConversions() {
            var media = new Data.Media() {
                Id = Guid.NewGuid()
            };

            Piranha.Extend.Fields.DocumentField field = media;
            Assert.Equal(media.Id, field.Id.Value);
        }

        [Fact]
        public void DocumentFieldInitMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var field = new Piranha.Extend.Fields.DocumentField {
                    Id = Guid.NewGuid()
                };

                field.Init(api);

                Assert.Null(field.Id);
            }
        }

        [Fact]
        public void DocumentFieldEquals() {
            var field1 = new Piranha.Extend.Fields.DocumentField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.DocumentField { 
                Id = field1.Id
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void DocumentFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.DocumentField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.DocumentField { 
                Id = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void VideoFieldConversions() {
            var media = new Data.Media() {
                Id = Guid.NewGuid()
            };

            Piranha.Extend.Fields.VideoField field = media;
            Assert.Equal(media.Id, field.Id.Value);
        }

        [Fact]
        public void VideoFieldInitMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var field = new Piranha.Extend.Fields.VideoField {
                    Id = Guid.NewGuid()
                };

                field.Init(api);

                Assert.Null(field.Id);
            }
        }

        [Fact]
        public void VideoFieldEquals() {
            var field1 = new Piranha.Extend.Fields.VideoField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.VideoField { 
                Id = field1.Id
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void VideoFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.VideoField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.VideoField { 
                Id = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void MediaFieldConversions() {
            var media = new Data.Media() {
                Id = Guid.NewGuid()
            };

            Piranha.Extend.Fields.MediaField field = media;
            Assert.Equal(media.Id, field.Id.Value);
        }

        [Fact]
        public void MediaFieldInitMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var field = new Piranha.Extend.Fields.MediaField {
                    Id = Guid.NewGuid()
                };

                field.Init(api);

                Assert.Null(field.Id);
            }
        }

        [Fact]
        public void MediaFieldEquals() {
            var field1 = new Piranha.Extend.Fields.MediaField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.MediaField { 
                Id = field1.Id
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void MediaFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.MediaField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.MediaField { 
                Id = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void ImageFieldConversionsNullImage() {
            var id = Guid.NewGuid();

            Piranha.Extend.Fields.ImageField field = id;
            Assert.Equal(id, field.Id.Value);

            string url = field;
            Assert.Equal("", url);
        }

        [Fact]
        public void DocumentFieldConversionsNullDocument() {
            var id = Guid.NewGuid();

            Piranha.Extend.Fields.DocumentField field = id;
            Assert.Equal(id, field.Id.Value);

            string url = field;
            Assert.Equal("", url);
        }

        [Fact]
        public void VideoFieldConversionsNullVideo() {
            var id = Guid.NewGuid();

            Piranha.Extend.Fields.VideoField field = id;
            Assert.Equal(id, field.Id.Value);

            string url = field;
            Assert.Equal("", url);
        }

        [Fact]
        public void MediaFieldConversionsNullMedia() {
            var id = Guid.NewGuid();

            Piranha.Extend.Fields.MediaField field = id;
            Assert.Equal(id, field.Id.Value);

            string url = field;
            Assert.Equal("", url);
        }

        [Fact]
        public void MarkdownFieldConversions() {
            var md = "This is a paragraph";
            var html = Piranha.App.Markdown.Transform(md);

            Piranha.Extend.Fields.MarkdownField field = md;
            Assert.Equal(md, field.Value);

            string outputHtml = field;
            Assert.Equal(html, outputHtml);
        }

        [Fact]
        public void MarkdownFieldEquals() {
            var field1 = new Piranha.Extend.Fields.MarkdownField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.MarkdownField { 
                Value = "Sollicitudin Justo Tristique"
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void MarkdownFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.MarkdownField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.MarkdownField { 
                Value = "Sollicitudin Tristique"
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void NumberFieldConversions() {
            var number = 25;

            Piranha.Extend.Fields.NumberField field = number;
            Assert.Equal(number, field.Value);
        }

        [Fact]
        public void NumberFieldEquals() {
            var field1 = new Piranha.Extend.Fields.NumberField { 
                Value = 23
            };
            var field2 = new Piranha.Extend.Fields.NumberField { 
                Value = 23
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void NumberFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.NumberField { 
                Value = 23
            };
            var field2 = new Piranha.Extend.Fields.NumberField { 
                Value = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void PageFieldEquals() {
            var field1 = new Piranha.Extend.Fields.PageField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.PageField { 
                Id = field1.Id
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void PageFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.PageField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.PageField { 
                Id = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }        

        [Fact]
        public void PostFieldEquals() {
            var field1 = new Piranha.Extend.Fields.PostField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.PostField { 
                Id = field1.Id
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void PostFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.PostField { 
                Id = Guid.NewGuid()
            };
            var field2 = new Piranha.Extend.Fields.PostField { 
                Id = null
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }        

        [Fact]
        public void SelectFieldEquals() {
            var field1 = new Piranha.Extend.Fields.SelectField<MyEnum> { 
                Value = MyEnum.Value1
            };
            var field2 = new Piranha.Extend.Fields.SelectField<MyEnum> { 
                Value = MyEnum.Value1
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void SelectFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.SelectField<MyEnum> { 
                Value = MyEnum.Value1
            };
            var field2 = new Piranha.Extend.Fields.SelectField<MyEnum> { 
                Value = MyEnum.Value2
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void StringFieldConversions() {
            var inStr = "This is a string";

            Piranha.Extend.Fields.StringField field = inStr;
            Assert.Equal(inStr, field.Value);

            string outStr = field;
            Assert.Equal(inStr, outStr); 
        }

        [Fact]
        public void StringFieldEquals() {
            var field1 = new Piranha.Extend.Fields.StringField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.StringField { 
                Value = "Sollicitudin Justo Tristique"
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void StringFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.StringField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.StringField { 
                Value = "Sollicitudin Tristique"
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void TextFieldConversions() {
            var inStr = "This is a string";

            Piranha.Extend.Fields.TextField field = inStr;
            Assert.Equal(inStr, field.Value);

            string outStr = field;
            Assert.Equal(inStr, outStr); 
        }

        [Fact]
        public void TextFieldEquals() {
            var field1 = new Piranha.Extend.Fields.TextField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.TextField { 
                Value = "Sollicitudin Justo Tristique"
            };

            Assert.True(field1 == field2);
            Assert.True(field1.Equals(field2));
            Assert.True(field1.Equals((object)field2));
        }

        [Fact]
        public void TextFieldNotEquals() {
            var field1 = new Piranha.Extend.Fields.TextField { 
                Value = "Sollicitudin Justo Tristique"
            };
            var field2 = new Piranha.Extend.Fields.TextField { 
                Value = "Sollicitudin Tristique"
            };

            Assert.True(field1 != field2);
            Assert.True(!field1.Equals(field2));
            Assert.True(!field1.Equals((object)field2));
        }

        [Fact]
        public void GetFieldTitleNull() {
            var field = new Piranha.Extend.Fields.TextField();

            Assert.Null(field.GetTitle());
        }

        [Fact]
        public void GetFieldTitle() {
            Piranha.Extend.Fields.TextField field = "String value";

            Assert.Equal("String value", field.GetTitle());
        }

        [Fact]
        public void GetFieldTitleMaxLength() {
            var sb = new StringBuilder();
            for (var n = 0; n < 10; n++) {
                sb.Append("NineChars");
            }

            Piranha.Extend.Fields.TextField field = sb.ToString();
            
            Assert.Equal(43, field.GetTitle().Length);
        }
    }
}
