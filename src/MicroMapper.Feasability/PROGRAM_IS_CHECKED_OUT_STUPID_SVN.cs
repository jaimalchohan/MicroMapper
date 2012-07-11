namespace MicroMapper.Feasability
{
    using System;
    using System.Linq;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Reflection;


    class Program
    {
        static void Main(string[] args)
        {
            Mapper.CreateMap<Source, Destination>()
                .ForMember(d => d.SourceProperty, o => o.MapFrom(s => s.SourceProperty));

            MicroMapper.Init();

            var source = new Source() { SourceProperty = "jaimal" };

            long total = 0;

            for (int i = 0; i < 10000; i++)
            {
                var start = DateTime.Now;

                //var r = Mapper.Map<Source, Destination>(source);
                var r = MicroMapper.Map<Source, Destination>(source);

                var end = DateTime.Now;

                total += (end - start).Ticks;
            }

            Console.WriteLine(total / 10000);

            Console.ReadLine();

        }
    }

    public class Source
    {
        public string SourceProperty { get; set; }
        public string SourceProperty2 { get; set; }
        public string SourceProperty3 { get; set; }
        public string SourceProperty4 { get; set; }
    }

    public class Destination
    {
        public string SourceProperty { get; set; }
        public string SourceProperty2 { get; set; }
        public string SourceProperty3 { get; set; }
        public string SourceProperty4 { get; set; }
    }

    public static class MicroMapper
    {
        public static Dictionary<string, Func<object, object>> _handles;

        static MicroMapper()
        {
            // read config
            // emit CustomMapper
            var obj = GenerateAndActiveDynamicAssembly(typeof(Source), typeof(Destination));

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

        private static object GenerateAndActiveDynamicAssembly(Type source, Type destination)
        {
            AssemblyName aName = new AssemblyName("MicroMapper.Mapper");
            AssemblyBuilder ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually
            // the assembly name plus an extension.
            ModuleBuilder mb =
                ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");


            TypeBuilder customMapperBuilder = mb.DefineType(
                "CustomMapper",
                 TypeAttributes.Public);

            var methodBuilder = customMapperBuilder.DefineMethod("MapSourceToDestinationWrapper", System.Reflection.MethodAttributes.Public | MethodAttributes.Static);//, System.Reflection.CallingConventions.Any, typeof(Source), new[] { typeof(Destination) });
            methodBuilder.SetParameters(typeof(Source));
            methodBuilder.SetReturnType(typeof(Destination));

            var mappingMethodBodyIL = methodBuilder.GetILGenerator();
            var endOfMethod = mappingMethodBodyIL.DefineLabel();
            mappingMethodBodyIL.DeclareLocal(typeof(Destination));
            mappingMethodBodyIL.DeclareLocal(typeof(Destination));

            IEnumerable<Tuple<PropertyInfo, PropertyInfo>> propertyMap = from sProp in source.GetProperties()
                                                                         join dProp in destination.GetProperties() on sProp.Name equals dProp.Name
                                                                         select new Tuple<PropertyInfo, PropertyInfo>(sProp, dProp);

            var ctorDestination = typeof(Destination).GetConstructor(new Type[0]);

            mappingMethodBodyIL.Emit(OpCodes.Nop);
            mappingMethodBodyIL.Emit(OpCodes.Newobj, ctorDestination);
            mappingMethodBodyIL.Emit(OpCodes.Stloc_0);

            foreach (var item in propertyMap)
            {
                mappingMethodBodyIL.Emit(OpCodes.Ldloc_0);
                mappingMethodBodyIL.Emit(OpCodes.Ldarg_0);
                mappingMethodBodyIL.Emit(OpCodes.Callvirt, item.Item1.GetGetMethod());
                mappingMethodBodyIL.Emit(OpCodes.Callvirt, item.Item2.GetSetMethod());
                mappingMethodBodyIL.Emit(OpCodes.Nop);
            }
            mappingMethodBodyIL.Emit(OpCodes.Ldloc_0);
            mappingMethodBodyIL.Emit(OpCodes.Stloc_1);

            mappingMethodBodyIL.Emit(OpCodes.Br_S, endOfMethod);

            mappingMethodBodyIL.MarkLabel(endOfMethod);
            mappingMethodBodyIL.Emit(OpCodes.Ldloc_1);
            mappingMethodBodyIL.Emit(OpCodes.Ret);

            Type tFinished = customMapperBuilder.CreateType();
            ab.Save(aName + ".dll");

            return Activator.CreateInstance(tFinished);
        }

        public static void Init()
        {
        }

        public static D Map<S, D>(S source)
        {
            return (D)_handles[string.Format("Map{0}To{1}Wrapper", typeof(S).Name, typeof(D).Name)].Invoke(source);
        }

    }

    //public class CustomMapper
    //{
    //    public static Destination MapSourceToDestinationWrapper(object source)
    //    {
    //        return CustomMapper.MapSourceToDestination(source as Source);
    //    }

    //    public static Destination MapSourceToDestination(Source source)
    //    {
    //        var destination = new Destination();
    //        destination.SourceProperty = source.SourceProperty;
    //        destination.SourceProperty2 = source.SourceProperty2;
    //        destination.SourceProperty3 = source.SourceProperty3;
    //        destination.SourceProperty4 = source.SourceProperty4;
    //        return destination;
    //    }
    //}
}
