using System.Reflection;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace SelfConfiguration
	{
		public class When_a_destination_configures_itself : SpecBase
		{
			private Destination _destination;

			protected override void Because_of()
			{
				Assembly assembly = typeof(Destination).Assembly;

				MicroMapper.MicroMapper.Initialize(x => x.SelfConfigure(assembly));

				var source = new Source { Property = "Test" };

				_destination = MicroMapper.MicroMapper.Map<Source, Destination>(source);
			}

			[Test]
			public void Should_map_with_regular_convention()
			{
				_destination.Property.ShouldEqual("Test");
			}

			[Test]
			public void Should_apply_mapping_expressions()
			{
				_destination.ConfiguredProperty.ShouldEqual("Test");
			}

			[Test]
			public void Should_configure_properly()
			{
				MicroMapper.MicroMapper.AssertConfigurationIsValid();
			}

			public class Source
			{
				public string Property { get; set; }
			}

			public class Destination : SelfProfiler<Source, Destination>
			{
				public string Property { get; set; }
				public string ConfiguredProperty { get; set; }

				protected override void DescribeConfiguration(IMappingExpression<Source, Destination> map)
				{
					map.ForMember(x => x.ConfiguredProperty, o => o.MapFrom(x => x.Property));
					map.ForMember(x => x.AllowNullDestinationValues, o => o.Ignore());
					map.ForMember(x => x.DestinationMemberNamingConvention, o => o.Ignore());
					map.ForMember(x => x.SourceMemberNamingConvention, o => o.Ignore());
					map.ForMember(x => x.SourceMemberNameTransformer, o => o.Ignore());
                    map.ForMember(x => x.DestinationMemberNameTransformer, o => o.Ignore());
                }
			}
		}
	}
}
