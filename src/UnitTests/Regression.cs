using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using System.Linq;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	namespace Regression
	{
		public class TestDomainItem : ITestDomainItem
		{
			public Guid ItemId { get; set; }
		}

		public interface ITestDomainItem
		{
			Guid ItemId { get; }
		}

		public class TestDtoItem
		{
			public Guid Id { get; set; }
		}

		[TestFixture]
		public class _fails_to_map_custom_mappings_when_mapping_collections_for_an_interface
		{
			[SetUp]
			public void Setup()
			{
				MicroMapper.MicroMapper.Reset();
			}

			[Test]
			public void should_map_the_id_property()
			{
				var domainItems = new List<ITestDomainItem>
                {
                    new TestDomainItem {ItemId = Guid.NewGuid()},
                    new TestDomainItem {ItemId = Guid.NewGuid()}
                };
				MicroMapper.MicroMapper.CreateMap<ITestDomainItem, TestDtoItem>()
					.ForMember(d => d.Id, s => s.MapFrom(x => x.ItemId));

				MicroMapper.MicroMapper.AssertConfigurationIsValid();

				var dtos = MicroMapper.MicroMapper.Map<IEnumerable<ITestDomainItem>, TestDtoItem[]>(domainItems);

				Assert.AreEqual(domainItems[0].ItemId, dtos[0].Id);
			}
		}

		public class Chris_bennages_nullable_datetime_issue : SpecBase
		{
			private Destination _result;

			public class Source
			{
				public DateTime? SomeDate { get; set; }
			}

            public class Destination
			{
				public MyCustomDate SomeDate { get; set; }
			}

            public class MyCustomDate
			{
				public int Day { get; set; }
				public int Month { get; set; }
				public int Year { get; set; }

				public MyCustomDate(int day, int month, int year)
				{
					Day = day;
					Month = month;
					Year = year;
				}
			}

			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<Source, Destination>();
				MicroMapper.MicroMapper.CreateMap<DateTime?, MyCustomDate>()
					.ConvertUsing(src => src.HasValue ? new MyCustomDate(src.Value.Day, src.Value.Month, src.Value.Year) : null);
			}

			protected override void Because_of()
			{
				_result = MicroMapper.MicroMapper.Map<Source, Destination>(new Source { SomeDate = new DateTime(2005, 12, 1) });
			}

			[Test]
			public void Should_map_a_date_with_a_value()
			{
				_result.SomeDate.Day.ShouldEqual(1);
			}

			[Test]
			public void Should_map_null_to_null()
			{
				var destination = MicroMapper.MicroMapper.Map<Source, Destination>(new Source());
				destination.SomeDate.ShouldBeNull();
			}
		}

		public class When_mappings_are_created_on_the_fly : NonValidatingSpecBase
		{
			private Order _order;

			public class Order
			{
				public string Name { get; set; }
				public Product Product { get; set; }
			}

			public class Product
			{
				public string ProductName { get; set; }
			}

			[Test]
			public void Should_not_use_AssignableMapper_when_mappings_are_specified_on_the_fly()
			{
				MicroMapper.MicroMapper.CreateMap<Order, Order>();

				var sourceOrder = new Order()
				{
					Name = "order",
					Product = new Product() { ProductName = "product" }
				};
				var destinationOrder = new Order();
				destinationOrder = MicroMapper.MicroMapper.Map(sourceOrder, destinationOrder);

				// Defining this mapping on the fly, but since the previous call to MicroMapper.MicroMapper.Map()
				// had to deal with mapping Product to Product, it created an AssignableMapper
				// which will get used in place of this one.  I would expect that if I call
				// MicroMapper.MicroMapper.CreateMap(), that should replace any cached value in
				// MappingEngine._objectMapperCache.
				MicroMapper.MicroMapper.CreateMap<Product, Product>();

				var sourceProduct = new Product() { ProductName = "name" };
				var destinationProduct = new Product();
				destinationProduct = MicroMapper.MicroMapper.Map(sourceProduct, destinationProduct);

				sourceProduct.ProductName.ShouldEqual(destinationProduct.ProductName);
				sourceProduct.ShouldNotEqual(destinationProduct);
			}
		}

		public class TestEnumerable : SpecBase
		{
			protected override void Establish_context()
			{
				MicroMapper.MicroMapper.CreateMap<Person, PersonModel>();
			}

			[Test]
			public void MapsEnumerableTypes()
			{
				Person[] personArr = new[] {new Person() {Name = "Name"}};
				People people = new People(personArr);
				
				var pmc = MicroMapper.MicroMapper.Map<People, List<PersonModel>>(people);
				
				Assert.IsNotNull(pmc);
				Assert.IsTrue(pmc.Count == 1);
			}

			public class People : IEnumerable
			{
				private readonly Person[] people;
				public People(Person[] people)
				{
					this.people = people;
				}
				public IEnumerator GetEnumerator()
				{
					foreach (var person in people)
					{
						yield return person;
					}
				}
			}

			public class Person
			{
				public string Name { get; set; }
			}

			public class PersonModel
			{
				public string Name { get; set; }
			}
		}

	}
}