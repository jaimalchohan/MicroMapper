using System;
using System.Collections.Generic;
using System.ComponentModel;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
    namespace InterfaceMapping
    {
        public class When_mapping_an_interface_to_an_abstract_type : SpecBase
        {
            private DtoObject _result;

            public class ModelObject
            {
                public IChildModelObject Child { get; set; }
            }

            public interface IChildModelObject
            {
                string ChildProperty { get; set; }
            }

            public class SubChildModelObject : IChildModelObject
            {
                public string ChildProperty { get; set; }
            }

            public class DtoObject
            {
                public DtoChildObject Child { get; set; }
            }

            public abstract class DtoChildObject
            {
                public virtual string ChildProperty { get; set; }
            }

            public class SubDtoChildObject : DtoChildObject
            {
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.Reset();

                var model = new ModelObject
                {
                    Child = new SubChildModelObject {ChildProperty = "child property value"}
                };

                MicroMapper.MicroMapper.CreateMap<ModelObject, DtoObject>();

                MicroMapper.MicroMapper.CreateMap<IChildModelObject, DtoChildObject>()
                    .Include<SubChildModelObject, SubDtoChildObject>();

                MicroMapper.MicroMapper.CreateMap<SubChildModelObject, SubDtoChildObject>();

                MicroMapper.MicroMapper.AssertConfigurationIsValid();

                _result = MicroMapper.MicroMapper.Map<ModelObject, DtoObject>(model);
            }

            [Test]
            public void Should_map_Child_to_SubDtoChildObject_type()
            {
                _result.Child.ShouldBeInstanceOfType(typeof (SubDtoChildObject));
            }

            [Test]
            public void Should_map_ChildProperty_to_child_property_value()
            {
                _result.Child.ChildProperty.ShouldEqual("child property value");
            }
        }

        public class When_mapping_a_concrete_type_to_an_interface_type : SpecBase
        {
            private IDestination _result;

            public class Source
            {
                public int Value { get; set; }
            }

            public interface IDestination
            {
                int Value { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.CreateMap<Source, IDestination>();
            }

            protected override void Because_of()
            {
                _result = MicroMapper.MicroMapper.Map<Source, IDestination>(new Source {Value = 5});
            }

            [Test]
            public void Should_create_an_implementation_of_the_interface()
            {
                _result.Value.ShouldEqual(5);
            }

            [Test]
            public void Should_not_derive_from_INotifyPropertyChanged()
            {
                _result.ShouldNotBeInstanceOf<INotifyPropertyChanged>();    
            }
        }

        public class When_mapping_a_concrete_type_to_an_interface_type_that_derives_from_INotifyPropertyChanged : SpecBase
        {
            private IDestination _result;

            private int _count;

            public class Source
            {
                public int Value { get; set; }
            }

            public interface IDestination : INotifyPropertyChanged
            {
                int Value { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.CreateMap<Source, IDestination>();
            }

            protected override void Because_of()
            {
                _result = MicroMapper.MicroMapper.Map<Source, IDestination>(new Source {Value = 5});
            }

            [Test]
            public void Should_create_an_implementation_of_the_interface()
            {
                _result.Value.ShouldEqual(5);
            }

            [Test]
            public void Should_derive_from_INotifyPropertyChanged()
            {
                _result.ShouldBeInstanceOf<INotifyPropertyChanged>();    
            }

            [Test]
            public void Should_notify_property_changes()
            {
                var count = 0;
                _result.PropertyChanged += (o, e) => {
                    count++;
                    o.ShouldBeTheSameAs(_result); 
                    e.PropertyName.ShouldEqual("Value");
                };

                _result.Value = 42;
                count.ShouldEqual(1);
                _result.Value.ShouldEqual(42);
            }

            [Test]
            public void Should_detach_event_handler()
            {
                _result.PropertyChanged += MyHandler;
                _count.ShouldEqual(0);

                _result.Value = 56;
                _count.ShouldEqual(1);

                _result.PropertyChanged -= MyHandler;
                _count.ShouldEqual(1);

                _result.Value = 75;
                _count.ShouldEqual(1);
            }

            private void MyHandler(object sender, PropertyChangedEventArgs e) {
                _count++;
            }
        }

        public class When_mapping_a_derived_interface_to_an_derived_concrete_type : SpecBase
        {
            private Destination _result = null;

            public interface ISourceBase
            {
                int Id { get; }
            }

            public interface ISource : ISourceBase
            {
                int SecondId { get; }
            }

            public class Source : ISource
            {
                public int Id { get; set; }
                public int SecondId { get; set; }
            }

            public abstract class DestinationBase
            {
                public int Id { get; set; }
            }

            public class Destination : DestinationBase
            {
                public int SecondId { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.CreateMap<ISource, Destination>();
            }

            protected override void Because_of()
            {
                _result = MicroMapper.MicroMapper.Map<ISource, Destination>(new Source {Id = 7, SecondId = 42});
            }

            [Test]
            public void Should_map_base_interface_property()
            {
                _result.Id.ShouldEqual(7);
            }

            [Test]
            public void Should_map_derived_interface_property()
            {
                _result.SecondId.ShouldEqual(42);
            }

            [Test]
            public void Should_pass_configuration_testing()
            {
                MicroMapper.MicroMapper.AssertConfigurationIsValid();
            }
        }

        public class When_mapping_a_derived_interface_to_an_derived_concrete_type_with_readonly_interface_members :
            SpecBase
        {
            private Destination _result = null;

            public interface ISourceBase
            {
                int Id { get; }
            }

            public interface ISource : ISourceBase
            {
                int SecondId { get; }
            }

            public class Source : ISource
            {
                public int Id { get; set; }
                public int SecondId { get; set; }
            }

            public interface IDestinationBase
            {
                int Id { get; }
            }

            public interface IDestination : IDestinationBase
            {
                int SecondId { get; }
            }

            public abstract class DestinationBase : IDestinationBase
            {
                public int Id { get; set; }
            }

            public class Destination : DestinationBase, IDestination
            {
                public int SecondId { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.CreateMap<ISource, Destination>();
            }

            protected override void Because_of()
            {
                _result = MicroMapper.MicroMapper.Map<ISource, Destination>(new Source {Id = 7, SecondId = 42});
            }

            [Test]
            public void Should_map_base_interface_property()
            {
                _result.Id.ShouldEqual(7);
            }

            [Test]
            public void Should_map_derived_interface_property()
            {
                _result.SecondId.ShouldEqual(42);
            }

            [Test]
            public void Should_pass_configuration_testing()
            {
                MicroMapper.MicroMapper.AssertConfigurationIsValid();
            }
        }

        public class When_mapping_to_a_type_with_explicitly_implemented_interface_members : SpecBase
        {
            private Destination _destination;

            public class Source
            {
                public int Value { get; set; }
            }

            public interface IOtherDestination
            {
                int OtherValue { get; set; }
            }

            public class Destination : IOtherDestination
            {
                public int Value { get; set; }
                int IOtherDestination.OtherValue { get; set; }
            }

            protected override void Establish_context()
            {
                MicroMapper.MicroMapper.CreateMap<Source, Destination>();
            }

            protected override void Because_of()
            {
                _destination = MicroMapper.MicroMapper.Map<Source, Destination>(new Source {Value = 10});
            }

            [Test]
            public void Should_ignore_interface_members_for_mapping()
            {
                _destination.Value.ShouldEqual(10);
            }

            [Test]
            public void Should_ignore_interface_members_for_validation()
            {
                MicroMapper.MicroMapper.AssertConfigurationIsValid();
            }
        }

        [TestFixture, Explicit]
        public class MappingToInterfacesPolymorphic
        {
            [SetUp]
            public void SetUp()
            {
                MicroMapper.MicroMapper.Reset();
            }

            public interface DomainInterface
            {
                Guid Id { get; set; }
                NestedType Nested { get; set; }
            }

            public class NestedType
            {
                public virtual string Name { get; set; }
                public virtual decimal DecimalValue { get; set; }
            }

            public class DomainImplA : DomainInterface
            {
                public virtual Guid Id { get; set; }
                private NestedType nested;

                public virtual NestedType Nested
                {
                    get
                    {
                        if (nested == null) nested = new NestedType();
                        return nested;
                    }
                    set { nested = value; }
                }
            }

            public class DomainImplB : DomainInterface
            {
                public virtual Guid Id { get; set; }
                private NestedType nested;

                public virtual NestedType Nested
                {
                    get
                    {
                        if (nested == null) nested = new NestedType();
                        return nested;
                    }
                    set { nested = value; }
                }
            }

            public class Dto
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
                public decimal DecimalValue { get; set; }
            }

            [Test]
            public void CanMapToDomainInterface()
            {
                MicroMapper.MicroMapper.CreateMap<DomainInterface, Dto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nested.Name))
                    .ForMember(dest => dest.DecimalValue, opt => opt.MapFrom(src => src.Nested.DecimalValue));
                MicroMapper.MicroMapper.CreateMap<Dto, DomainInterface>()
                    .ForMember(dest => dest.Nested.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Nested.DecimalValue, opt => opt.MapFrom(src => src.DecimalValue));

                var domainInstance1 = new DomainImplA();
                var domainInstance2 = new DomainImplB();
                var domainInstance3 = new DomainImplA();

                var dtoCollection = new List<Dto>
                {
                    MicroMapper.MicroMapper.Map<DomainInterface, Dto>(domainInstance1),
                    MicroMapper.MicroMapper.Map<DomainInterface, Dto>(domainInstance2),
                    MicroMapper.MicroMapper.Map<DomainInterface, Dto>(domainInstance3)
                };

                dtoCollection[0].Id = Guid.NewGuid();
                dtoCollection[0].DecimalValue = 1M;
                dtoCollection[0].Name = "Bob";
                dtoCollection[1].Id = Guid.NewGuid();
                dtoCollection[1].DecimalValue = 0.1M;
                dtoCollection[1].Name = "Frank";
                dtoCollection[2].Id = Guid.NewGuid();
                dtoCollection[2].DecimalValue = 2.1M;
                dtoCollection[2].Name = "Sam";

                MicroMapper.MicroMapper.Map<Dto, DomainInterface>(dtoCollection[0], domainInstance1);
                MicroMapper.MicroMapper.Map<Dto, DomainInterface>(dtoCollection[1], domainInstance2);
                MicroMapper.MicroMapper.Map<Dto, DomainInterface>(dtoCollection[2], domainInstance3);

                Assert.AreEqual(dtoCollection[0].Id, domainInstance1.Id);
                Assert.AreEqual(dtoCollection[1].Id, domainInstance2.Id);
                Assert.AreEqual(dtoCollection[2].Id, domainInstance3.Id);

                Assert.AreEqual(dtoCollection[0].DecimalValue, domainInstance1.Nested.DecimalValue);
                Assert.AreEqual(dtoCollection[1].DecimalValue, domainInstance2.Nested.DecimalValue);
                Assert.AreEqual(dtoCollection[2].DecimalValue, domainInstance3.Nested.DecimalValue);

                Assert.AreEqual(dtoCollection[0].DecimalValue, domainInstance1.Nested.Name);
                Assert.AreEqual(dtoCollection[1].DecimalValue, domainInstance2.Nested.Name);
                Assert.AreEqual(dtoCollection[2].DecimalValue, domainInstance3.Nested.Name);
            }
        }
    }
}