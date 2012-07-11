namespace MicroMapper.Tests
{
    using System;
    using NUnit.Framework;

    public class MicroMapperTestBase
    {
        [SetUp]
        public void setup()
        {
            create_map();
        }

        [TearDown]
        public void teardown()
        {
            MicroMapper.Clear();
        }

        protected virtual void create_map()
        {
            
        }
    }
}
