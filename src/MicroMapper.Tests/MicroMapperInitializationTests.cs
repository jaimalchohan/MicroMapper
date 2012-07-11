
namespace MicroMapper.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class MicroMapperInitializationTests : MicroMapperTestBase
    {
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void can_only_init_once()
        {
            MicroMapper.CreateMap<ObjectA, ObjectB>();

            MicroMapper.Init();
            MicroMapper.Init();
        }
    }
}
