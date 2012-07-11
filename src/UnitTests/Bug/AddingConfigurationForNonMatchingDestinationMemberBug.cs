using System;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests.Bug
{
    namespace AddingConfigurationForNonMatchingDestinationMember
    {
        public class AddingConfigurationForNonMatchingDestinationMemberBug : NonValidatingSpecBase
        {
            public class Source
            {
                
            }

            public class Destination
            {
                public string Value { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Source, Destination>()
                        .ForMember(dest => dest.Value, opt => opt.NullSubstitute("Foo"));
                });
            }

            [Test]
            public void Should_show_configuration_error()
            {
                typeof (ConfigurationException).ShouldBeThrownBy(MicroMapper.MicroMapper.AssertConfigurationIsValid);
            }
        }
    }
}