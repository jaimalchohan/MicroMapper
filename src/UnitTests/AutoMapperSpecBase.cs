using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
    public class SpecBase : NonValidatingSpecBase
	{
        [Test]
        public void Should_have_valid_configuration()
        {
            MicroMapper.MicroMapper.AssertConfigurationIsValid();
        }
    }

    public class NonValidatingSpecBase : SpecBase
    {
        protected override void Cleanup()
        {
            MicroMapper.MicroMapper.Reset();
        }

    }
}