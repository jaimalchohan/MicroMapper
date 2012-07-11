using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	[TestFixture]
	public class Tester
	{
		[Test]
		public void Should_be_able_to_handle_derived_proxy_types()
		{
			MicroMapper.MicroMapper.CreateMap<ModelType, DtoType>();
			var source = new[] { new DerivedModelType { TheProperty = "Foo" }, new DerivedModelType { TheProperty = "Bar" } };

			var destination = (DtoType[])MicroMapper.MicroMapper.Map(source, typeof(ModelType[]), typeof(DtoType[]));

			destination[0].TheProperty.ShouldEqual("Foo");
			destination[1].TheProperty.ShouldEqual("Bar");
		}

		[TearDown]
		public void Teardown()
		{
			MicroMapper.MicroMapper.Reset();
		}

		public class ModelType
		{
			public string TheProperty { get; set; }
		}

		public class DerivedModelType : ModelType
		{
		}

		public class DtoType
		{
			public string TheProperty { get; set; }
		}
	}
}