using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests.Bug
{
    namespace NamingConventions
    {
        public class Neda
        {
            public string cmok { get; set; }

            public string moje_ime { get; set; }

            public string moje_prezime { get; set; }

            public string ja_se_zovem_imenom { get; set; }

        }

        public class Dario
        {
            public string cmok { get; set; }

            public string mojeIme { get; set; }

            public string MojePrezime { get; set; }

            public string JaSeZovemImenom { get; set; }
        }

        public class When_mapping_with_lowercae_naming_conventions_two_ways_in_profiles : SpecBase
        {
            private Dario _dario;
            private Neda _neda;

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.Initialize(cfg =>
                {
                    cfg.CreateProfile("MyMapperProfile", prf =>
                    {
                        prf.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                        prf.CreateMap<Neda, Dario>();
                    });                   
                    cfg.CreateProfile("MyMapperProfile2", prf =>
                    {
                        prf.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
                        prf.CreateMap<Dario, Neda>();
                    });
                });
            }

            protected override void Because_of()
            {
                _dario = MicroMapper.MicroMapper.Map<Neda, Dario>(new Neda {ja_se_zovem_imenom = "foo"});
                _neda = MicroMapper.MicroMapper.Map<Dario, Neda>(_dario);
            }

            [Test]
            public void Should_map_from_lower_to_pascal()
            {
                _neda.ja_se_zovem_imenom.ShouldEqual("foo");
            }

            [Test]
            public void Should_map_from_pascal_to_lower()
            {
                _dario.JaSeZovemImenom.ShouldEqual("foo");
            }
        }

    }
}