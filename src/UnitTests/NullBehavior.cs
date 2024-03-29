using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace NullBehavior
	{
		public class When_mapping_a_model_with_null_items : SpecBase
		{
			private ModelDto _result;

			public class ModelDto
			{
				public ModelSubDto Sub { get; set; }
				public int SubSomething { get; set; }
				public string NullString { get; set; }
			}

			public class ModelSubDto
			{
				public int[] Items { get; set; }
			}

			public class ModelObject
			{
				public ModelSubObject Sub { get; set; }
				public string NullString { get; set; }
			}

			public class ModelSubObject
			{
				public int[] GetItems()
				{
					return new[] { 0, 1, 2, 3 };
				}

				public int Something { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject();
				model.Sub = null;

				MicroMapper.MicroMapper.AllowNullDestinationValues = false;
				MicroMapper.MicroMapper.CreateMap<ModelObject, ModelDto>();
				MicroMapper.MicroMapper.CreateMap<ModelSubObject, ModelSubDto>();

				_result = MicroMapper.MicroMapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_populate_dto_items_with_a_value()
			{
				_result.Sub.ShouldNotBeNull();
			}

			[Test]
			public void Should_provide_empty_array_for_array_type_values()
			{
				_result.Sub.Items.ShouldNotBeNull();
			}

			[Test]
			public void Should_return_default_value_of_property_in_the_chain()
			{
				_result.SubSomething.ShouldEqual(0);
			}
	
            [Test]
			public void Default_value_for_string_should_be_empty()
			{
				_result.NullString.ShouldEqual(string.Empty);
			}
        }

		public class When_overriding_null_behavior_with_null_source_items : SpecBase
		{
			private ModelDto _result;

			public class ModelDto
			{
				public ModelSubDto Sub { get; set; }
				public int SubSomething { get; set; }
				public string NullString { get; set; }
			}

			public class ModelSubDto
			{
				public int[] Items { get; set; }
			}

			public class ModelObject
			{
				public ModelSubObject Sub { get; set; }
				public string NullString { get; set; }
			}

			public class ModelSubObject
			{
				public int[] GetItems()
				{
					return new[] { 0, 1, 2, 3 };
				}

				public int Something { get; set; }
			}

			protected override void Establish_context()
			{
				var model = new ModelObject();
				model.Sub = null;
				model.NullString = null;

				MicroMapper.MicroMapper.Initialize(c => c.AllowNullDestinationValues = true);
				MicroMapper.MicroMapper.CreateMap<ModelObject, ModelDto>();
				MicroMapper.MicroMapper.CreateMap<ModelSubObject, ModelSubDto>();

				_result = MicroMapper.MicroMapper.Map<ModelObject, ModelDto>(model);
			}

			[Test]
			public void Should_map_first_level_items_as_null()
			{
				_result.NullString.ShouldBeNull();
			}

			[Test]
			public void Should_map_primitive_items_as_default()
			{
				_result.SubSomething.ShouldEqual(0);
			}

			[Test]
			public void Should_map_any_sub_mapped_items_as_null()
			{
				_result.Sub.ShouldBeNull();
			}
		}

		public class When_overriding_null_behavior_in_a_profile : SpecBase
		{
			private DefaultDestination _defaultResult;
			private NullDestination _nullResult;

			public class DefaultSource
			{
				public object Value { get; set; }
			}

			public class DefaultDestination
			{
				public object Value { get; set; }
			}

			public class NullSource
			{
				public object Value { get; set; }
			}

			public class NullDestination
			{
				public object Value { get; set; }
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateProfile("MapsNulls", p =>
					{
						p.AllowNullDestinationValues = false;
						p.CreateMap<NullSource, NullDestination>();
					});
				MicroMapper.MicroMapper.CreateMap<DefaultSource, DefaultDestination>();
			}

			protected override void Because_of()
			{
				_defaultResult = MicroMapper.MicroMapper.Map<DefaultSource, DefaultDestination>(new DefaultSource());
				_nullResult = MicroMapper.MicroMapper.Map<NullSource, NullDestination>(new NullSource());
			}

			[Test]
			public void Should_use_default_behavior_in_default_profile()
			{
				_defaultResult.Value.ShouldBeNull();
			}

			[Test]
			public void Should_use_overridden_null_behavior_in_profile()
			{
				_nullResult.Value.ShouldNotBeNull();
			}
		}

		public class When_using_a_custom_resolver_and_the_source_value_is_null : NonValidatingSpecBase
		{
			public class NullResolver : ValueResolver<Source, string>
			{
				protected override string ResolveCore(Source source)
				{
					if (source == null)
						return "jon";
					return "fail";
				}
			}

			private Source _source;
			private Destination _dest;

			public class Source
			{
				public string MyName { get; set; }
			}

			public class Destination
			{
				public string Name { get; set; }
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<Source, Destination>()
					.ForMember(dest => dest.Name, opt => opt.ResolveUsing<NullResolver>().FromMember(src => src.MyName));
				_source = new Source();
			}

			protected override void Because_of()
			{
				_dest = MicroMapper.MicroMapper.Map<Source, Destination>(_source);
			}

			[Test]
			public void Should_perform_the_translation()
			{
				_dest.Name.ShouldEqual("jon");
			}
		}

	    public class When_mapping_using_a_custom_member_mapping_and_source_is_null : SpecBase
	    {
	        private Dest _dest;

	        public class Source
            {
                public SubSource Sub { get; set; }
            }

            public class SubSource
            {
                public int Value { get; set; }
            }

            public class Dest
            {
                public int OtherValue { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.AllowNullDestinationValues = false;
                MicroMapper.MicroMapper.CreateMap<Source, Dest>()
                    .ForMember(dest => dest.OtherValue, opt => opt.MapFrom(src => src.Sub.Value));
            }

            protected override void Because_of()
            {
                _dest = MicroMapper.MicroMapper.Map<Source, Dest>(new Source());
            }

	        [Test]
	        public void Should_map_to_null_on_destination_values()
	        {
	            _dest.OtherValue.ShouldEqual(0);
	        }
	    }

	    public class When_specifying_a_resolver_for_a_nullable_type : SpecBase
	    {
	        private FooViewModel _result;

	        public class NullableBoolToLabel : TypeConverter<bool?, string>
            {
                protected override string ConvertCore(bool? source)
                {
                    if (source.HasValue)
                    {
                        if (source.Value)
                            return "Yes";
                        else
                            return "No";
                    }
                    else
                        return "(n/a)";
                }
            }

            public class Foo
            {
                public bool? IsFooBarred { get; set; }
            }

            public class FooViewModel
            {
                public string IsFooBarred { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.Initialize(cfg =>
                {
                    cfg.CreateMap<bool?, string>().ConvertUsing<NullableBoolToLabel>();
                    cfg.CreateMap<Foo, FooViewModel>();
                });
            }

            protected override void Because_of()
            {
                var foo3 = new Foo { IsFooBarred = null };
                _result = MicroMapper.MicroMapper.Map<Foo, FooViewModel>(foo3);
            }

	        [Test]
	        public void Should_allow_the_resolver_to_handle_null_values()
	        {
                _result.IsFooBarred.ShouldEqual("(n/a)");
            }
	    }

	}
}