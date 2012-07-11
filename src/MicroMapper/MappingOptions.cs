namespace MicroMapper
{
    using System;
    using System.Reflection;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents options that can be appied to the source and destination properties.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public class MappingOptions<TSource> : IMappingOptions
    {
        /// <summary>
        /// Gets or sets the source property information.
        /// </summary>
        /// <value>The source property information.</value>
        public MemberInfo SourceMember { get; set; }

        /// <summary>
        /// Gets a value indicating whether the mapping should be ignored.
        /// </summary>
        /// <value><c>true</c> if the mapping should be ignored; otherwise, <c>false</c>.</value>
        public bool IgnoreMapping { get; private set; }

        /// <summary>
        /// Represents how the source property shoould be mapped to the destination property.
        /// </summary>
        /// <param name="sourceMember">The source property.</param>
        public void MapFrom(Expression<Func<TSource, object>> sourceMember)
        {
            SourceMember = ReflectionHelper.FindProperty(sourceMember);
        }

        /// <summary>
        /// Ignores any mapping for this property.
        /// </summary>
        public void Ignore()
        {
            IgnoreMapping = true;
        }
    }
}