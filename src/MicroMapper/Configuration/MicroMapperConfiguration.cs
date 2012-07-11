namespace MicroMapper.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class MicroMapperConfiguration<TSource, TDestination> : IMicroMapperConfiguration<TSource, TDestination>
    {
        private Mapping _mapping = null;

        internal MicroMapperConfiguration(Mapping mapping)
        {
            _mapping = mapping;
        }

        public IMicroMapperConfiguration<TSource, TDestination> ForMember(Action<MappingOptions<TSource>> mappingOptions, Expression<Func<TDestination, object>> destinationMember)
        {
            var destinationMemberInfo = ReflectionHelper.FindProperty(destinationMember);

            _mapping.AddCustomMap<TSource>(mappingOptions, destinationMemberInfo);

            return this;
        }
    }
}