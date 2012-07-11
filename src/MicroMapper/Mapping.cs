namespace MicroMapper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    internal class Mapping
    {
        /// <summary>
        /// Gets the source type.
        /// </summary>
        /// <value>The source.</value>
        public Type Source { get; private set; }

        /// <summary>
        /// Gets the destination type.
        /// </summary>
        /// <value>The destination.</value>
        public Type Destination { get; private set; }

        /// <summary>
        /// Gets the custom property mappings.
        /// </summary>
        /// <value>The custom maps.</value>
        public Dictionary<MemberInfo, IMappingOptions> CustomMaps { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="source">The source type.</param>
        /// <param name="destination">The destination type.</param>
        public Mapping(Type source, Type destination)
        {
            Source = source;
            Destination = destination;
            CustomMaps = new Dictionary<MemberInfo, IMappingOptions>();
        }

        /// <summary>
        /// Adds a custom property mapping.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="mappingOptions">The mapping options.</param>
        /// <param name="destinationPropertyInfo">The property information representing the destination property.</param>
        public void AddCustomMap<TSource>(Action<MappingOptions<TSource>> mappingOptions, MemberInfo destinationMemberInfo)
        {
            var m = new MappingOptions<TSource>();

            mappingOptions.Invoke(m);

            CustomMaps.Add(destinationMemberInfo, m);
        }
    }
}