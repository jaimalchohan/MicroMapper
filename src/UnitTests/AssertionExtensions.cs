using System;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests
{
	public static class AssertionExtensions
	{
		public static void ShouldNotBeThrownBy(this Type exception, Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				if (exception.IsInstanceOfType(ex))
				{
					throw new AssertionException(string.Format("Expected no exception of type {0} to be thrown.", exception), ex);
				}
			}
		}
	}
}