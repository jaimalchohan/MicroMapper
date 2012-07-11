using System;
using NUnit.Framework;

namespace MicroMapper.Tests.ValueTypes
{
    public class FieldsPropertiesAndStructs : MicroMapperTestBase
    {
        private Destination _destination;
        private Source _source;

        public class Source
        {
            public int Value1 { get; set; }
            public string Value2 { get; set; }
        }

        public struct Destination
        {
            public int Value1 { get; set; }
            public string Value2;
        }


        protected override void create_map()
        {
            MicroMapper.CreateMap<Source, Destination>();
            MicroMapper.Init();

            _source = new Source() { Value1 = 4, Value2 = "hello" };
            _destination = MicroMapper.Map<Source, Destination>(_source);
        }

        [Test]
        public void can_map_property_value()
        {
            Assert.AreEqual(_source.Value1, _destination.Value1);
        }

        [Test]
        public void can_map_field_value()
        {
            Assert.AreEqual(_source.Value2, _destination.Value2);
        }
    }
}
