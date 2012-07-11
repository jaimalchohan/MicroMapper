using MicroMapper.Configuration;

namespace MicroMapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class MapperTests : MicroMapperTestBase
    {

        [Test]
        public void can_automap_properties_and_fields()
        {
            MicroMapper.CreateMap<ObjectA, ObjectB>();
            MicroMapper.Init();

            var objectA = new ObjectA() { Property = "jaimal", FieldA = "chohan" };

            var objectB = MicroMapper.Map<ObjectA, ObjectB>(objectA);

            Assert.AreEqual(objectA.Property, objectB.Property);
            Assert.AreEqual(objectA.FieldA, objectB.FieldA);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void cannot_create_same_map_twice()
        {
            MicroMapper.CreateMap<ObjectA, ObjectB>();
            MicroMapper.CreateMap<ObjectA, ObjectB>();
        }
    }
}
