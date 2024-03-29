using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMicroMapper.MicroMapper.Mappers;
using NUnit.Framework;

namespace AutoMicroMapper.MicroMapper.UnitTests.Mappers
{
    [TestFixture]
    public class TypeHelperTests
    {
        [Test]
        public void CanReturnElementTypeOnCollectionThatImplementsTheSameGenericInterfaceMultipleTimes()
        {
            Type myType = typeof(ChargeCollection);

            Type elementType = TypeHelper.GetElementType(myType);

            Assert.IsNotNull(elementType);
        }

        public class Charge { }

        public interface IChargeCollection : IEnumerable<object> { }

        public class ChargeCollection : Collection<Charge>, IChargeCollection
        {
            public new IEnumerator<object> GetEnumerator()
            {
                return null;
            }
        }
    }
}
