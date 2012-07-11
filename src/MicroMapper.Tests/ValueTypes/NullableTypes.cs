using System;
using NUnit.Framework;

namespace MicroMapper.Tests.ValueTypes
{
    public class NullableTypesAndGlobalConvertors : MicroMapperTestBase
    {
        private Destination _destination;
        private Source _source;

        public class Source
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }

        public struct Destination
        {
            public int Value1 { get; set; }
            public int? Value2 { get; set; }
        }

        /*protected override void create_map()
        {
            MicroMapper.CreateMap<string, int>().ConvertUsing(Convert.ToInt32);
            MicroMapper.CreateMap<Source, Destination>();

            _source = new Source() { Value1 = "10", Value2 = "20" };

            _destination = MicroMapper.Map<Source, Destination>(_source);
        }


        [Test]
        public void Should_use_map_registered_for_underlying_type()
        {
            Assert _destination.Value2.ShouldEqual(20);
        }

        [Test]
        public void Should_still_map_value_type()
        {
            _destination.Value1.ShouldEqual(10);
        }*/
    }
}
