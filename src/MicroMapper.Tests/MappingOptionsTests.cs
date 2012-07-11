using MicroMapper.Configuration;

namespace MicroMapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class MappingOptionsTests : MicroMapperTestBase
    {
        [Test]
        public void can_ignore_properties()
        {
            MicroMapper.CreateMap<ObjectA, ObjectB>()
                .ForMember(o => o.Ignore(), d => d.Property)
                .ForMember(o => o.MapFrom(s => s.Property), d => d.Property2)
                .ForMember(o => o.MapFrom(s => s.Property), d => d.Property4);

            MicroMapper.Init();

            var objectA = new ObjectA() { Property = "jaimal" };

            var objectB = MicroMapper.Map<ObjectA, ObjectB>(objectA);

            Assert.IsNullOrEmpty(objectB.Property);
            Assert.AreEqual(objectA.Property, objectB.Property2);
            Assert.AreEqual(objectA.Property, objectB.Property4);
        }

        [Test]
        public void can_map_1_source_property_to_2_destination_properties()
        {
            MicroMapper.CreateMap<ObjectA, ObjectB>()
                .ForMember(o => o.MapFrom(s => s.Property), d => d.Property2)
                .ForMember(o => o.MapFrom(s => s.Property), d => d.Property4);

            MicroMapper.Init();

            var objectA = new ObjectA() { Property = "jaimal" };

            var objectB = MicroMapper.Map<ObjectA, ObjectB>(objectA);

            Assert.AreEqual(objectA.Property, objectB.Property);
            Assert.AreEqual(objectA.Property, objectB.Property2);
            Assert.AreEqual(objectA.Property, objectB.Property4);
        }

        [Test]
        public void can_map_properties_with_different_names()
        {
            MicroMapper.CreateMap<ObjectA, ObjectB>()
                       .ForMember(o => o.MapFrom(s => s.Property1), o => o.Property2);

            MicroMapper.Init();

            var objectA = new ObjectA() { Property1 = "jaimal" };

            var objectB = MicroMapper.Map<ObjectA, ObjectB>(objectA);

            Assert.AreEqual(objectA.Property1, objectB.Property2);
        }
    }
}
