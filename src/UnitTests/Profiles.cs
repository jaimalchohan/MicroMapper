using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace Profiles
	{
		public class When_segregating_configuration_through_a_profile : SpecBase
		{
			private Dto _result;

			public class Model
			{
				public int Value { get; set; }
			}

			public class Dto
			{
				public string Value { get; set; }
			}

			public class Formatter : IValueFormatter
			{
				public string FormatValue(ResolutionContext context)
				{
					return context.SourceValue + " Custom";
				}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.AddFormatter<Formatter>();

				MicroMapper.MicroMapper.CreateProfile("Custom");

				MicroMapper.MicroMapper.CreateMap<Model, Dto>().WithProfile("Custom");
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Model, Dto>(new Model {Value = 5});
			}

			[Test]
			public void Should_not_include_default_profile_configuration_with_profiled_maps()
			{
				_result.Value.ShouldEqual("5");
			}
		}

		public class When_configuring_a_profile_through_a_profile_subclass : SpecBase
		{
			private Dto _result;
		    private CustomProfile1 _customProfile;

		    public class Model
			{
				public int Value { get; set; }
			}

			public class Dto
			{
				public string Value { get; set; }
			}

			public class Dto2
			{
				public string Value { get; set; }
			}

			public class Formatter : IValueFormatter
			{
				public string FormatValue(ResolutionContext context)
				{
					return context.SourceValue + " Custom";
				}
			}

			public class CustomProfile1 : Profile
			{
				protected internal override void Configure()
				{
					AddFormatter<Formatter>();

					CreateMap<Model, Dto>();
				}
			}

			public class CustomProfile2 : Profile
			{
				protected internal override void Configure()
				{
					AddFormatter<Formatter>();

					CreateMap<Model, Dto2>();
				}
			}

			protected override void Establish_context()
			{
			    _customProfile = new CustomProfile1();
			    MicroMapper.MicroMapper.AddProfile(_customProfile);
				MicroMapper.MicroMapper.AddProfile<CustomProfile2>();
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Model, Dto>(new Model { Value = 5 });
			}

		    [Test]
		    public void Should_default_the_custom_profile_name_to_the_type_name()
		    {
                _customProfile.ProfileName.ShouldEqual(typeof(CustomProfile1).FullName);
		    }

			[Test]
			public void Should_use_the_overridden_configuration_method_to_configure()
			{
				_result.Value.ShouldEqual("5 Custom");
			}
		}

	}
}