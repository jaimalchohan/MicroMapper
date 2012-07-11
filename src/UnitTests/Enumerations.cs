using System;
using AutoMicroMapper.MicroMapper.UnitTests;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.Tests
{
	[TestFixture]
	public class EnumMappingFixture
	{
		[SetUp]
		[TearDown]
		public void Cleanup()
		{
			MicroMapper.MicroMapper.Reset();
		}

		[Test]
		public void ShouldMapSharedEnum()
		{
			MicroMapper.MicroMapper.CreateMap<Order, OrderDto>();

			var order = new Order
				{
					Status = Status.InProgress
				};

			var dto = MicroMapper.MicroMapper.Map<Order, OrderDto>(order);

			dto.Status.ShouldEqual(Status.InProgress);
		}

		[Test]
		public void ShouldMapToUnderlyingType() {
			MicroMapper.MicroMapper.CreateMap<Order, OrderDtoInt>();

			var order = new Order {
				Status = Status.InProgress
			};

			var dto = MicroMapper.MicroMapper.Map<Order, OrderDtoInt>(order);

			dto.Status.ShouldEqual(1);
		}

		[Test]
		public void ShouldMapToStringType() {
			MicroMapper.MicroMapper.CreateMap<Order, OrderDtoString>();

			var order = new Order {
				Status = Status.InProgress
			};

			var dto = MicroMapper.MicroMapper.Map<Order, OrderDtoString>(order);

			dto.Status.ShouldEqual("InProgress");
		}

		[Test]
		public void ShouldMapFromUnderlyingType() {
			MicroMapper.MicroMapper.CreateMap<OrderDtoInt, Order>();

			var order = new OrderDtoInt {
				Status = 1
			};

			var dto = MicroMapper.MicroMapper.Map<OrderDtoInt, Order>(order);

			dto.Status.ShouldEqual(Status.InProgress);
		}

		[Test]
		public void ShouldMapFromStringType() {
			MicroMapper.MicroMapper.CreateMap<OrderDtoString, Order>();

			var order = new OrderDtoString {
				Status = "InProgress"
			};

			var dto = MicroMapper.MicroMapper.Map<OrderDtoString, Order>(order);

			dto.Status.ShouldEqual(Status.InProgress);
		}
		
		[Test]
		public void ShouldMapEnumByMatchingNames()
		{
			MicroMapper.MicroMapper.CreateMap<Order, OrderDtoWithOwnStatus>();

			var order = new Order
				{
					Status = Status.InProgress
				};

			var dto = MicroMapper.MicroMapper.Map<Order, OrderDtoWithOwnStatus>(order);

			dto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapEnumByMatchingValues()
		{
			MicroMapper.MicroMapper.CreateMap<Order, OrderDtoWithOwnStatus>();

			var order = new Order
				{
					Status = Status.InProgress
				};

			var dto = MicroMapper.MicroMapper.Map<Order, OrderDtoWithOwnStatus>(order);

			dto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapSharedNullableEnum() 
		{
			MicroMapper.MicroMapper.CreateMap<OrderWithNullableStatus, OrderDtoWithNullableStatus>();

			var order = new OrderWithNullableStatus {
				Status = Status.InProgress
			};

			var dto = MicroMapper.MicroMapper.Map<OrderWithNullableStatus, OrderDtoWithNullableStatus>(order);

			dto.Status.ShouldEqual(Status.InProgress);
		}

		[Test]
		public void ShouldMapNullableEnumByMatchingValues() 
		{
			MicroMapper.MicroMapper.CreateMap<OrderWithNullableStatus, OrderDtoWithOwnNullableStatus>();

			var order = new OrderWithNullableStatus {
				Status = Status.InProgress
			};

			var dto = MicroMapper.MicroMapper.Map<OrderWithNullableStatus, OrderDtoWithOwnNullableStatus>(order);

			dto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapNullableEnumToNullWhenSourceEnumIsNull() 
		{
			MicroMapper.MicroMapper.CreateMap<OrderWithNullableStatus, OrderDtoWithOwnNullableStatus>();

			var order = new OrderWithNullableStatus {
				Status = null
			};

			var dto = MicroMapper.MicroMapper.Map<OrderWithNullableStatus, OrderDtoWithOwnNullableStatus>(order);

			dto.Status.ShouldBeNull();
		}

		[Test]
		public void ShouldMapEnumUsingCustomResolver()
		{
			MicroMapper.MicroMapper.CreateMap<Order, OrderDtoWithOwnStatus>()
				.ForMember(dto => dto.Status, options => options
				                                         	.ResolveUsing<DtoStatusValueResolver>());

			var order = new Order
				{
					Status = Status.InProgress
				};

			var mappedDto = MicroMapper.MicroMapper.Map<Order, OrderDtoWithOwnStatus>(order);

			mappedDto.Status.ShouldEqual(StatusForDto.InProgress);
		}

		[Test]
		public void ShouldMapEnumUsingGenericEnumResolver()
		{
			MicroMapper.MicroMapper.CreateMap<Order, OrderDtoWithOwnStatus>()
				.ForMember(dto => dto.Status, options => options
				                                         	.ResolveUsing<EnumValueResolver<Status, StatusForDto>>()
				                                         	.FromMember(m => m.Status));

			var order = new Order
				{
					Status = Status.InProgress
				};

			var mappedDto = MicroMapper.MicroMapper.Map<Order, OrderDtoWithOwnStatus>(order);

			mappedDto.Status.ShouldEqual(StatusForDto.InProgress);
		}

        [Test]
        public void ShouldMapEnumWithInvalidValue()
        {
            MicroMapper.MicroMapper.CreateMap<Order, OrderDtoWithOwnStatus>();

            var order = new Order
            {
                Status = 0
            };

            var dto = MicroMapper.MicroMapper.Map<Order, OrderDtoWithOwnStatus>(order);

            var expected = (StatusForDto)0;

            dto.Status.ShouldEqual(expected);
        }

		public enum Status
		{
			InProgress = 1,
			Complete = 2
		}

		public enum StatusForDto
		{
			InProgress = 1,
			Complete = 2
		}

		public class Order
		{
			public Status Status { get; set; }
		}

		public class OrderDto
		{
			public Status Status { get; set; }
		}

		public class OrderDtoInt {
			public int Status { get; set; }
		}

		public class OrderDtoString {
			public string Status { get; set; }
		}

		public class OrderDtoWithOwnStatus
		{
			public StatusForDto Status { get; set; }
		}

		public class OrderWithNullableStatus 
		{
			public Status? Status { get; set; }
		}

		public class OrderDtoWithNullableStatus 
		{
			public Status? Status { get; set; }
		}

		public class OrderDtoWithOwnNullableStatus 
		{
			public StatusForDto? Status { get; set; }
		}

		public class DtoStatusValueResolver : IValueResolver
		{
			public ResolutionResult Resolve(ResolutionResult source)
			{
				return source.New(((Order)source.Value).Status);
			}
		}

		public class EnumValueResolver<TInputEnum, TOutputEnum> : IValueResolver
		{
			public ResolutionResult Resolve(ResolutionResult source)
			{
				return source.New(((TOutputEnum)Enum.Parse(typeof(TOutputEnum), Enum.GetName(typeof(TInputEnum), source.Value), false)));
			}
		}
	}

	[TestFixture]
	public class When_mapping_from_a_null_object_with_an_enum
	{
		[SetUp]
		public void SetUp()
		{
			MicroMapper.MicroMapper.AllowNullDestinationValues = false;
			MicroMapper.MicroMapper.CreateMap<SourceClass, DestinationClass>();
		}

		public enum EnumValues
		{
			One, Two, Three
		}

		public class DestinationClass
		{
			public EnumValues Values { get; set; }
		}

		public class SourceClass
		{
			public EnumValues Values { get; set; }
		}

		[Test]
		public void Should_set_the_target_enum_to_the_default_value()
		{
			SourceClass sourceClass = null;
			var dest = MicroMapper.MicroMapper.Map<SourceClass, DestinationClass>(sourceClass);
			dest.Values.ShouldEqual(default(EnumValues));
		}
	}

	[TestFixture]
	public class When_mapping_from_a_null_object_with_an_enum_on_a_nullable_enum
	{
		[SetUp]
		public void SetUp()
		{
			MicroMapper.MicroMapper.AllowNullDestinationValues = false;
			MicroMapper.MicroMapper.CreateMap<SourceClass, DestinationClass>();
		}

		public enum EnumValues
		{
			One, Two, Three
		}

		public class DestinationClass
		{
			public EnumValues? Values { get; set; }
		}

		public class SourceClass
		{
			public EnumValues Values { get; set; }
		}

		[Test]
		public void Should_set_the_target_enum_to_null()
		{
			SourceClass sourceClass = null;
			var dest = MicroMapper.MicroMapper.Map<SourceClass, DestinationClass>(sourceClass);
			dest.Values.ShouldEqual(null);
		}
	}

	[TestFixture]
	public class When_mapping_from_a_null_object_with_a_nullable_enum
	{
		[SetUp]
		public void SetUp()
		{
			MicroMapper.MicroMapper.AllowNullDestinationValues = false;
			MicroMapper.MicroMapper.CreateMap<SourceClass, DestinationClass>();
		}

		public enum EnumValues
		{
			One, Two, Three
		}

		public class DestinationClass
		{
			public EnumValues Values { get; set; }
		}

		public class SourceClass
		{
			public EnumValues? Values { get; set; }
		}

		[Test]
		public void Should_set_the_target_enum_to_the_default_value()
		{
			SourceClass sourceClass = null;
			var dest = MicroMapper.MicroMapper.Map<SourceClass, DestinationClass>(sourceClass);
			dest.Values.ShouldEqual(default(EnumValues));
		}
	}

	public class When_mapping_a_flags_enum : SpecBase
	{
		private DestinationFlags _result;

		[Flags]
		private enum SourceFlags
		{
			None = 0,
			One = 1,
			Two = 2,
			Four = 4,
			Eight = 8
		}

		[Flags]
		private enum DestinationFlags
		{
			None = 0,
			One = 1,
			Two = 2,
			Four = 4,
			Eight = 8
		}

		protected override void Establish_context()
		{
			// No type map needed
		}

		protected override void Because_of()
		{
			_result = MicroMapper.MicroMapper.Map<SourceFlags, DestinationFlags>(SourceFlags.One | SourceFlags.Four | SourceFlags.Eight);
		}

		[Test]
		public void Should_include_all_source_enum_values()
		{
			_result.ShouldEqual(DestinationFlags.One | DestinationFlags.Four | DestinationFlags.Eight);
		}
	}

}
