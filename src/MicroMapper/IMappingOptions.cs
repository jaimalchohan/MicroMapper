namespace MicroMapper
{
    using System.Reflection;

    /// <summary>
    /// Represents options that can be appied to the source and destination properties.
    /// </summary>
    public interface IMappingOptions
    {
        /// <summary>
        /// Gets or sets the source property information.
        /// </summary>
        /// <value>The source property information.</value>
        MemberInfo SourceMember { get; }

        /// <summary>
        /// Gets a value indicating whether the mapping should be ignored.
        /// </summary>
        /// <value><c>true</c> if the mapping should be ignored; otherwise, <c>false</c>.</value>
        bool IgnoreMapping { get; }
    }
}
