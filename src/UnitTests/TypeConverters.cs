using System;
using System.ComponentModel;
using System.Reflection;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace CustomMapping
	{
		public class When_specifying_type_converters : SpecBase
		{
			private Destination _result;

			public class Source
			{
				public string Value1 { get; set; }
				public string Value2 { get; set; }
				public string Value3 { get; set; }
			}

			public class Destination
			{
				public int Value1 { get; set; }
				public DateTime Value2 { get; set; }
				public Type Value3 { get; set; }
			}

			public class DateTimeTypeConverter : TypeConverter<string, DateTime>
			{
				protected override DateTime ConvertCore(string source)
				{
					return System.Convert.ToDateTime(source);
				}
			}

			public class TypeTypeConverter : TypeConverter<string, Type>
			{
				protected override Type ConvertCore(string source)
				{
					Type type = Assembly.GetExecutingAssembly().GetType(source);
					return type;
				}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<string, int>().ConvertUsing(arg => Convert.ToInt32(arg));
				MicroMapper.MicroMapper.CreateMap<string, DateTime>().ConvertUsing(new DateTimeTypeConverter());
				MicroMapper.MicroMapper.CreateMap<string, Type>().ConvertUsing<TypeTypeConverter>();

				MicroMapper.MicroMapper.CreateMap<Source, Destination>();

				var source = new Source
				{
					Value1 = "5",
					Value2 = "01/01/2000",
					Value3 = "AutoMicroMapper.MicroMapper.UnitTests.CustomMapping.When_specifying_type_converters+Destination"
				};

				_result = MicroMapper.MicroMapper.Map<Source, Destination>(source);
			}

			[Test]
			public void Should_convert_type_using_expression()
			{
				_result.Value1.ShouldEqual(5);
			}

			[Test]
			public void Should_convert_type_using_instance()
			{
				_result.Value2.ShouldEqual(new DateTime(2000, 1, 1));
			}

			[Test]
			public void Should_convert_type_using_Func_that_returns_instance()
			{
				_result.Value3.ShouldEqual(typeof(Destination));
			}
		}

		public class When_specifying_type_converters_on_types_with_incompatible_members : SpecBase
		{
            private ParentDestination _result;

			public class Source
			{
				public string Foo { get; set; }
			}

			public class Destination
			{
                public int Type { get; set; }
			}

            public class ParentSource
            {
                public Source Value { get; set; }
            }

            public class ParentDestination
            {
                public Destination Value { get; set; }
            }

			protected override void Establish_context()
			{
			    MicroMapper.MicroMapper.CreateMap<Source, Destination>().ConvertUsing(arg => new Destination {Type = Convert.ToInt32(arg.Foo)});
			    MicroMapper.MicroMapper.CreateMap<ParentSource, ParentDestination>();

                var source = new ParentSource
				{
					Value = new Source { Foo = "5",}
				};

                _result = MicroMapper.MicroMapper.Map<ParentSource, ParentDestination>(source);
			}

			[Test]
			public void Should_convert_type_using_expression()
			{
                _result.Value.Type.ShouldEqual(5);
			}
		}

		public class When_specifying_mapping_with_the_BCL_type_converter_class : SpecBase
		{
			[TypeConverter(typeof(CustomTypeConverter))]
			public class Source
			{
				public int Value { get; set; }
			}

			public class Destination
			{
				public int OtherValue { get; set; }
			}

			public class CustomTypeConverter : TypeConverter
			{
				public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
				{
					return destinationType == typeof (Destination);
				}

				public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
				{
					return new Destination
						{
							OtherValue = ((Source) value).Value + 10
						};
				}
			}

			[Test]
			public void Should_convert_type_using_the_custom_type_converter()
			{
				var source = new Source
					{
						Value = 5
					};
				var destination = MicroMapper.MicroMapper.Map<Source, Destination>(source);

				destination.OtherValue.ShouldEqual(15);
			}
		}

		public class When_specifying_a_type_converter_for_a_non_generic_configuration : SpecBase
		{
			private Destination _result;

			public class Source
			{
				public int Value { get; set; }
			}

			public class Destination
			{
				public int OtherValue { get; set; }
			}

			public class CustomConverter : TypeConverter<Source, Destination>
			{
				protected override Destination ConvertCore(Source source)
				{
					return new Destination
						{
							OtherValue = source.Value + 10
						};
				}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap(typeof(Source), typeof(Destination)).ConvertUsing<CustomConverter>();
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Source, Destination>(new Source {Value = 5});
			}

			[Test]
			public void Should_use_converter_specified()
			{
				_result.OtherValue.ShouldEqual(15);
			}

			[Test]
			public void Should_pass_configuration_validation()
			{
				MicroMapper.MicroMapper.AssertConfigurationIsValid();
			}
		}

		public class When_specifying_a_non_generic_type_converter_for_a_non_generic_configuration : SpecBase
		{
			private Destination _result;

			public class Source
			{
				public int Value { get; set; }
			}

			public class Destination
			{
				public int OtherValue { get; set; }
			}

			public class CustomConverter : TypeConverter<Source, Destination>
			{
				protected override Destination ConvertCore(Source source)
				{
					return new Destination
						{
							OtherValue = source.Value + 10
						};
				}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap(typeof(Source), typeof(Destination)).ConvertUsing(typeof(CustomConverter));
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Source, Destination>(new Source {Value = 5});
			}

			[Test]
			public void Should_use_converter_specified()
			{
				_result.OtherValue.ShouldEqual(15);
			}

			[Test]
			public void Should_pass_configuration_validation()
			{
				MicroMapper.MicroMapper.AssertConfigurationIsValid();
			}
		}

	}
}