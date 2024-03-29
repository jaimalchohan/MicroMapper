using System.Collections.Generic;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using System.Linq;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace FillingExistingDestination
	{

		public class When_the_destination_object_is_specified : SpecBase
		{
			private Source _source;
			private Destination _originalDest;
			private Destination _dest;

			public class Source
			{
				public int Value { get; set; }
			}

			public class Destination
			{
				public int Value { get; set; }
			}

			protected override void Establish_context()
			{
				base.Establish_context();

				MicroMapper.MicroMapper.CreateMap<Source, Destination>();

				_source = new Source
				{
					Value = 10,
				};
			}

			protected override void Because_of()
			{
				_originalDest = new Destination { Value = 1111 };
				_dest = MicroMapper.MicroMapper.Map<Source, Destination>(_source, _originalDest);
			}

			[Test]
			public void Should_do_the_translation()
			{
				_dest.Value.ShouldEqual(10);
			}

			[Test]
			public void Should_return_the_destination_object_that_was_passed_in()
			{
				_originalDest.ShouldBeTheSameAs(_dest);
			}
		}

		public class When_the_destination_object_is_specified_and_you_are_converting_an_enum : SpecBase
		{
			private string _result;

			public enum SomeEnum
			{
				One,
				Two,
				Three
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<SomeEnum, string>(SomeEnum.Two, "test");
			}

			[Test]
			public void Should_return_the_enum_as_a_string()
			{
				_result.ShouldEqual("Two");
			}
		}
	}
}