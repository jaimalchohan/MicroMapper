using MicroMapper.Configuration;

namespace MicroMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// MicroMapper for managing class mappings.
    /// </summary>
    public static class MicroMapper
    {
        static Dictionary<string, Func<object, object>> _handles;

        static readonly object _this = new Object();

        static IList<Mapping> _mappings = null;

        public static void Init()
        {
            if (_handles != null)
            {
                throw new InvalidOperationException("MicroMapper has already been initialized.");
            }

            lock(_this)
            {
                var obj = new AssemblyGenerator().GenerateAndActivateDynamicAssembly(_mappings);

                _handles = new Dictionary<string, Func<object, object>>();
                var methodInfos = obj.GetType().GetMethods().Where(m => m.Name.EndsWith("Wrapper"));

                foreach (var methodInfo in methodInfos)
                {
                    _handles.Add(
                        methodInfo.Name,
                        delegate(object o) { return methodInfo.Invoke(null, new object[] { o }); }
                    );
                }
            }
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return (TDestination)_handles[string.Format("Map{0}To{1}Wrapper", typeof(TSource).Name, typeof(TDestination).Name)].Invoke(source);
        }

        public static IMicroMapperConfiguration<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            if (_mappings == null)
            {
                _mappings = new List<Mapping>();
            }

            if (_mappings.Any(mp => mp.Source == typeof(TSource) && mp.Destination == typeof(TDestination)))
            {
                throw new InvalidOperationException(string.Format("Existing mapping found for {0} to {1}", typeof(TSource).Name, typeof(TDestination).Name));
            }

            var mapping = new Mapping(typeof(TSource), typeof(TDestination));
            
            _mappings.Add(mapping);

            return new MicroMapperConfiguration<TSource, TDestination>(mapping);
        }

        public static void Clear()
        {
            _mappings = null;
            _handles = null;
        }
    }
}