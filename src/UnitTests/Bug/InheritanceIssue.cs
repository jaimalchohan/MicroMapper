using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ;
using NBehave.Spec.NUnit;

namespace AutoMicroMapper.MicroMapper.UnitTests.Bug
{

    namespace EntityTest1
    {
        #region Dto objects
        public abstract class DynamicPropertyDTO<T>
        {
            public abstract event PropertyChangedEventHandler PropertyChanged;
        }

        public class ComplexPropertyDTO<T> : DynamicPropertyDTO<T>
        {
            public Dictionary<string, object> Properties { get; set; }

            public override event PropertyChangedEventHandler PropertyChanged;

            public ComplexPropertyDTO()
            {
                this.Properties = new Dictionary<string, object>();
            }
        }

        public class ComponentContainerDTO
        {
            public Dictionary<string, ComponentDTO> Components
            {
                get;
                set;
            }

            public ComponentContainerDTO()
            {
                this.Components = new Dictionary<string, ComponentDTO>();
            }
        }

        public class EntityDTO : ComponentContainerDTO
        {
            public int Id { get; set; }

        }

        public class ComponentDTO : ComplexPropertyDTO<object>
        {
            public EntityDTO Owner { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }

        }

        public class HealthDTO : ComponentDTO
        {
            public decimal CurrentHealth { get; set; }

        }

        public class PhysicalLocationDTO : ComponentDTO
        {
            public Point2D Location { get; set; }
        }
        #endregion


        #region Domain objects
        public abstract class DynamicProperty<T> : INotifyPropertyChanged
        {
            public abstract event PropertyChangedEventHandler PropertyChanged;
        }

        public class ComplexProperty<T> : DynamicProperty<T>
        {
            public Dictionary<string, object> Properties { get; set; }

            public override event PropertyChangedEventHandler PropertyChanged;

            public ComplexProperty()
            {
                this.Properties = new Dictionary<string, object>();
            }
        }

        public class SimpleProperty<T> : DynamicProperty<T>
        {
            public override event PropertyChangedEventHandler PropertyChanged;
        }

        public class ComponentContainer
        {
            public Dictionary<string, Component> Components { get; set; }

            public ComponentContainer()
            {
                this.Components = new Dictionary<string, Component>();
            }
        }

        public class Entity : ComponentContainer
        {
            public int Id { get; set; }

        }

        public class Component : ComplexProperty<object>
        {
            public Entity Owner { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }

        }

        public class Health : Component
        {
            public decimal CurrentHealth { get; set; }
        }

        public struct Point2D
        {
            public decimal X;
            public decimal Y;

            public Point2D(decimal x, decimal y)
            {
                X = x;
                Y = y;
            }
        }

        public class PhysicalLocation : Component
        {
            public Point2D Location { get; set; }
        }
        #endregion

        class Program
        {
            static void Main(string[] args)
            {
                var entity = new Entity() { Id = 1 };
                var healthComponent = new Health()
                {
                    CurrentHealth = 100,
                    Owner = entity,
                    Name = "Health",
                    Id = 2
                };
                entity.Components.Add("1", healthComponent);
                var locationComponent = new PhysicalLocation()
                {
                    Location
                        = new Point2D() { X = 1, Y = 2 },
                    Owner = entity,
                    Name =
                        "PhysicalLocation",
                    Id = 3
                };
                entity.Components.Add("2", locationComponent);

                MicroMapper.MicroMapper.CreateMap<ComponentContainer, ComponentContainerDTO>()
                    .Include<Entity, EntityDTO>();

                MicroMapper.MicroMapper.CreateMap<Entity, EntityDTO>();

                MicroMapper.MicroMapper.CreateMap<Component, ComponentDTO>()
                    .Include<Health, HealthDTO>()
                    .Include<PhysicalLocation, PhysicalLocationDTO>();

                MicroMapper.MicroMapper.CreateMap<Health, HealthDTO>();
                MicroMapper.MicroMapper.CreateMap<PhysicalLocation, PhysicalLocationDTO>();

                MicroMapper.MicroMapper.AssertConfigurationIsValid();

                EntityDTO targetEntity = MicroMapper.MicroMapper.Map<Entity, EntityDTO>(entity);

                targetEntity.Components.Count.ShouldEqual(2);

                targetEntity.Components.Last().Value.Name.ShouldEqual("PhysicalLocation");

                targetEntity.Components.First().Value.ShouldBeInstanceOf<HealthDTO>();

                targetEntity.Components.Last().Value.ShouldBeInstanceOf<PhysicalLocationDTO>();
            }
        }
    }
}