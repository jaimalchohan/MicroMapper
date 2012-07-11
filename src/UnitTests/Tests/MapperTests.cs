using NUnit.Framework;
using NBehave.Spec.NUnit;

namespace AutoMicroMapper.MicroMapper.UnitTests.Tests
{
	[TestFixture]
	public class MapperTests : NonValidatingSpecBase
	{
		public class Source
		{
			
		}
		
		public class Destination
		{
			
		}
			
		[Test]
		public void Should_find_configured_type_map_when_two_types_are_configured()
		{
			MicroMapper.MicroMapper.CreateMap<Source, Destination>();

			MicroMapper.MicroMapper.FindTypeMapFor<Source, Destination>().ShouldNotBeNull();
		}
	}
}