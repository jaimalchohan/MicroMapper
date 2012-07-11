using System;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace Indexers
	{
		public class When_mapping_to_a_destination_with_an_indexer_property : SpecBase
		{
			private Destination _result;

			public class Source
			{
				public string Value { get; set; }
			}

			public class Destination
			{
				public string Value { get; set; }
				public string this[string key] { get { return null; }}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<Source, Destination>();
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Source, Destination>(new Source {Value = "Bob"});
			}

			[Test]
			public void Should_ignore_indexers_and_map_successfully()
			{
				_result.Value.ShouldEqual("Bob");
			}

			[Test]
			public void Should_pass_configuration_check()
			{
				Exception thrown = null;
				try
				{
					MicroMapper.MicroMapper.AssertConfigurationIsValid();
				}
				catch (Exception ex)
				{
					thrown = ex;
				}

				thrown.ShouldBeNull();
			}
		}

	}
}