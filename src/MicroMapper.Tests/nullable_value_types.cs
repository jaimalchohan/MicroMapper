namespace MicroMapper.Tests
{
    using System;
    using NUnit.Framework;
    using NBehave.Spec.NUnit;

    public class nullable_value_types : SpecBase
    {
        private Destination _destination;

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

        protected override void Establish_context()
        {
            MicroMapper.CreateMap<string, int>().ConvertUsing(Convert.ToInt32);
            MicroMapper.CreateMap<Source, Destination>();
        }

        protected override void Because_of()
        {
            _destination = MicroMapper.Map<Source, Destination>(new Source { Value1 = "10", Value2 = "20" });
        }

        [Test]
        public void Should_use_map_registered_for_underlying_type()
        {
            _destination.Value2.ShouldEqual(20);
        }

        [Test]
        public void Should_still_map_value_type()
        {
            _destination.Value1.ShouldEqual(10);
        }


    }
}
