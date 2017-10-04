/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Serializers;
using Xunit;

namespace Piranha.Tests
{
    public class Serializers
    {
        [Fact]
        public SerializerManager Register() {
            var manager = new SerializerManager();

            manager.Register<TextField>(new StringFieldSerializer<TextField>());

            return manager;
        }

        [Fact]
        public void GetSerializer() {
            var manager = Register();

            var serializer = manager[typeof(TextField)];

            Assert.NotNull(serializer);
        }

        [Fact]
        public void UnRegister() {
            var manager = Register();

            manager.UnRegister<TextField>();

            Assert.Null(manager[typeof(TextField)]);
        }

        [Fact]
        public void SerializeDateField() {
            var serializer = new DateFieldSerializer();

            var str = serializer.Serialize(new DateField() {
               Value = new DateTime(2001, 1, 5, 16, 0, 0) 
            });

            Assert.Equal("2001-01-05 16:00:00", str);
        }

        [Fact]
        public void SerializeEmptyDateField() {
            var serializer = new DateFieldSerializer();

            var str = serializer.Serialize(new DateField());

            Assert.Null(str);
        }

        [Fact]
        public void DeserializeDateField() {
            var serializer = new DateFieldSerializer();
            var date = new DateTime(2001, 1, 5, 16, 0, 0);
            var str = "2001-01-05 16:00:00";

            var field = (DateField)serializer.Deserialize(str);

            Assert.NotNull(field);
            Assert.Equal(date, field.Value.Value);
        }

        [Fact]
        public void DeserializeEmptyDateField() {
            var serializer = new DateFieldSerializer();

            var field = (DateField)serializer.Deserialize(null);

            Assert.False(field.Value.HasValue);            
        }

        [Fact]
        public void WrongInputToDateField() {
            var serializer = new DateFieldSerializer();

            Assert.Throws<ArgumentException>(() => serializer.Serialize(new StringField() {
                Value = "Exception"
            }));
        }

        [Fact]
        public void SerializeImageField() {
            var serializer = new ImageFieldSerializer();
            var id = Guid.NewGuid().ToString();

            var str = serializer.Serialize(new ImageField() {
                Id = id
            });

            Assert.Equal(id, str);
        }

        [Fact]
        public void DeserializeImageField() {
            var serializer = new ImageFieldSerializer();
            var id = Guid.NewGuid().ToString();

            var field = (ImageField)serializer.Deserialize(id);

            Assert.Equal(id, field.Id);
        }

        [Fact]
        public void WrongInputToImageField() {
            var serializer = new ImageFieldSerializer();

            Assert.Throws<ArgumentException>(() => serializer.Serialize(new StringField() {
                Value = "Exception"
            }));
        }
        

        [Fact]
        public void SerializeStringField() {
            var serializer = new StringFieldSerializer<StringField>();
            var ipsum = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";

            var str = serializer.Serialize(new StringField() {
                Value = ipsum
            });

            Assert.Equal(ipsum, str);
        }

        [Fact]
        public void DeserializeStringField() {
            var serializer = new StringFieldSerializer<StringField>();
            var ipsum = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";

            var field = (StringField)serializer.Deserialize(ipsum);

            Assert.Equal(ipsum, field.Value);
        }

        [Fact]
        public void WrongInputStringField() {
            var serializer = new StringFieldSerializer<StringField>();

            Assert.Throws<ArgumentException>(() => serializer.Serialize(new DateField() {
                Value = DateTime.Now
            }));
        }        
    }
}