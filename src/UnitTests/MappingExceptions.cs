using System;
using NUnit.Framework;
using NBehave.Spec.NUnit;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace MappingExceptions
	{
		public class When_encountering_a_member_mapping_problem_during_mapping : NonValidatingSpecBase
		{
			public class Source
			{
				public string Value { get; set; }
			}

			public class Dest
			{
				public int Value { get; set;}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<Source, Dest>();
			}

			[Test]
			public void Should_provide_a_contextual_exception()
			{
				var source = new Source {Value = "adsf"};
				typeof(MappingException).ShouldBeThrownBy(() => MicroMapper.MicroMapper.Map<Source, Dest>(source));
			}

			[Test]
			public void Should_have_contextual_mapping_information()
			{
				var source = new Source { Value = "adsf" };
				MappingException thrown = null;
				try
				{
					MicroMapper.MicroMapper.Map<Source, Dest>(source);
				}
				catch (MappingException ex)
				{
					thrown = ex;
				}
				thrown.ShouldNotBeNull();
				thrown.InnerException.ShouldNotBeNull();
				thrown.InnerException.ShouldBeInstanceOf<MappingException>();
				((MappingException) thrown.InnerException).Context.PropertyMap.ShouldNotBeNull();
			}
		}
	}
}