namespace MicroMapper
{
    using System;
    using System.Reflection;
    using System.Linq.Expressions;

    /// <summary>
    /// Reflection helper class
    /// </summary>
    internal class ReflectionHelper
    {
        /// <summary>
        /// Finds the property information from the lambda expression.
        /// </summary>
        /// <param name="lamdbaexpression">The lamdba expression.</param>
        /// <returns>The property information.</returns>
        public static PropertyInfo FindProperty(LambdaExpression lamdbaexpression)
        {
            return ((MemberExpression)lamdbaexpression.Body).Member as PropertyInfo;
        }
    }
}
