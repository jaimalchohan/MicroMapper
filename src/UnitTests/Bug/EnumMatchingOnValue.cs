using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests.Bug
{
    [TestFixture]
    public class EnumMatchingOnValue : SpecBase
    {
        private SecondClass _result;

        public class FirstClass
        {
            public FirstEnum EnumValue { get; set; }
        }

        public enum FirstEnum
        {
            NamedEnum = 1,
            SecondNameEnum = 2
        }

        public class SecondClass
        {
            public SecondEnum EnumValue { get; set; }
        }

        public enum SecondEnum
        {
            DifferentNamedEnum = 1,
            SecondNameEnum = 2
        }
        protected override void Establish_context()
        {
            MicroMapper.MicroMapper.Initialize(cfg =>
            {
                cfg.CreateMap<FirstClass, SecondClass>();
            });
        }

        protected override void Because_of()
        {
            var source = new FirstClass
            {
                EnumValue = FirstEnum.NamedEnum
            };
            _result = MicroMapper.MicroMapper.Map<FirstClass, SecondClass>(source);
        }

        [Test]
        public void Should_match_on_the_name_even_if_values_match()
        {
            _result.EnumValue.ShouldEqual(SecondEnum.DifferentNamedEnum);
        }
    }
}