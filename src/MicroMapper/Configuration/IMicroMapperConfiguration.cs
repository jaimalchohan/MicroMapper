namespace MicroMapper.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    
    public interface IMicroMapperConfiguration<TSource, TDestination>
    {
        IMicroMapperConfiguration<TSource, TDestination> ForMember(Action<MappingOptions<TSource>> mappingOptions, Expression<Func<TDestination, object>> destinationMember);
    }
}
