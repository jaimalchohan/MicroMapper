using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests.Bug
{

    [TestFixture]
    public class When_configuring_all_members_and_some_do_not_match
    {
        public class ModelObjectNotMatching
        {
            public string Foo_notfound { get; set; }
            public string Bar_notfound;
        }

        public class ModelDto
        {
            public string Foo { get; set; }
            public string Bar;
        }

        [SetUp]
        public void SetUp()
        {
            MicroMapper.MicroMapper.Reset();
        }

        [Test]
        public void Should_still_apply_configuration_to_missing_members()
        {
            MicroMapper.MicroMapper.CreateMap<ModelObjectNotMatching, ModelDto>()
                .ForAllMembers(opt => opt.Ignore());
            MicroMapper.MicroMapper.AssertConfigurationIsValid();
        }
    }
}