using NUnit.Framework;
using NBehave.Spec.NUnit;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace Internationalization
	{
		public class When_mapping_a_source_with_non_english_property_names : SpecBase
		{
			private OrderDto _result;

			public class Order
			{
				public Customer Customer { get; set; }
			}

			public class Customer
			{
				public string ��� { get; set; }
			}

			public class OrderDto
			{
				public string Customer��� { get; set; }
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<Order, OrderDto>();
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Order, OrderDto>(new Order {Customer = new Customer {��� = "Bob"}});
			}

			[Test]
			public void Should_match_to_identical_property_name_on_destination()
			{
				_result.Customer���.ShouldEqual("Bob");
			}
		}

	}
}