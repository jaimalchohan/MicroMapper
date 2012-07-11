using MicroMapper.Configuration;

namespace MicroMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class AssemblyGenerator
    {
        public object GenerateAndActivateDynamicAssembly(IList<Mapping> _mappings)
        {
            var assemblyName = new AssemblyName("MicroMapper.Mapper");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually
            // the assembly name plus an extension.
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder customMapperBuilder = moduleBuilder.DefineType("CustomMicroMapper", TypeAttributes.Public);

            foreach (var mapping in _mappings) 
            {
                var methodBuilder = customMapperBuilder.DefineMethod(
                    String.Format("Map{0}To{1}Wrapper", mapping.Source.Name, mapping.Destination.Name),
                    System.Reflection.MethodAttributes.Public | MethodAttributes.Static);

                methodBuilder.SetParameters(mapping.Source);
                methodBuilder.SetReturnType(mapping.Destination);

                var memberMap = GetMembers(mapping);

                var mappingMethodBodyIL = methodBuilder.GetILGenerator();
                var endOfMethodLabel = mappingMethodBodyIL.DefineLabel();
                var destinationVariable = mappingMethodBodyIL.DeclareLocal(mapping.Destination);
                mappingMethodBodyIL.DeclareLocal(mapping.Destination);

                EmitInitDestinationType(mappingMethodBodyIL, mapping, destinationVariable);

                foreach (var item in memberMap)
                {
                    var sourcePropertyInfo = item.Item1 as PropertyInfo;
                    var sourceFieldInfo = item.Item1 as FieldInfo;

                    var destPropertyInfo = item.Item2 as PropertyInfo;
                    var destFieldInfo = item.Item2 as FieldInfo;

                    if (mapping.Destination.IsValueType)
                    {
                        mappingMethodBodyIL.Emit(OpCodes.Ldloca_S, destinationVariable);
                    }
                    else if (mapping.Destination.IsClass)
                    {
                        mappingMethodBodyIL.Emit(OpCodes.Ldloc_0);
                    }

                    mappingMethodBodyIL.Emit(OpCodes.Ldarg_0);

                    if (sourcePropertyInfo != null)
                    {
                        mappingMethodBodyIL.Emit(OpCodes.Callvirt, sourcePropertyInfo.GetGetMethod());
                    }
                    else
                    {
                        mappingMethodBodyIL.Emit(OpCodes.Ldfld, sourceFieldInfo);
                    }

                    if (destPropertyInfo != null)
                    {
                        if (mapping.Destination.IsValueType)
                        {
                            mappingMethodBodyIL.Emit(OpCodes.Call, destPropertyInfo.GetSetMethod()); ;
                        }
                        else if (mapping.Destination.IsClass)
                        {
                            mappingMethodBodyIL.Emit(OpCodes.Callvirt, destPropertyInfo.GetSetMethod());
                        }
                    }
                    else
                    {
                        mappingMethodBodyIL.Emit(OpCodes.Stfld, destFieldInfo);
                    }

                    mappingMethodBodyIL.Emit(OpCodes.Nop);
                }

                EmitEndMethod(mappingMethodBodyIL, endOfMethodLabel);
            }

            Type tFinished = customMapperBuilder.CreateType();
            assemblyBuilder.Save(assemblyName + ".dll");

            return Activator.CreateInstance(tFinished);
        }

        public List<Tuple<MemberInfo, MemberInfo>> GetMembers(Mapping mapping)
        {
            var memberMap = new List<Tuple<MemberInfo, MemberInfo>>();

            // add custom maps
            foreach (var customMap in mapping.CustomMaps.Where(c => c.Value.IgnoreMapping == false))
            {
                memberMap.Add(new Tuple<MemberInfo, MemberInfo>(customMap.Value.SourceMember, customMap.Key));
            }

            var sourceMembers = mapping.Source.GetProperties().Cast<MemberInfo>()
                                              .Concat(mapping.Source.GetFields().Cast<MemberInfo>());

            var destinationMembers = mapping.Destination.GetProperties().Cast<MemberInfo>()
                                                        .Concat(mapping.Destination.GetFields().Cast<MemberInfo>());

            // properties with the same name
            memberMap.AddRange(
                 from sMember in sourceMembers
                 join dMember in destinationMembers on sMember.Name equals dMember.Name
                 where !mapping.CustomMaps.Any(c => c.Key == dMember)
                 select new Tuple<MemberInfo, MemberInfo>(sMember, dMember));

            return memberMap;
        }

        private void EmitInitDestinationType(ILGenerator mappingMethodBodyIL, Mapping mapping, LocalBuilder destinationVariable)
        {
            mappingMethodBodyIL.Emit(OpCodes.Nop);

            if (mapping.Destination.IsValueType)
            {
                mappingMethodBodyIL.Emit(OpCodes.Ldloca_S, destinationVariable);
                mappingMethodBodyIL.Emit(OpCodes.Initobj, mapping.Destination);
            }
            else if (mapping.Destination.IsClass)
            {
                var ctorDestination = mapping.Destination.GetConstructor(new Type[0]);
                mappingMethodBodyIL.Emit(OpCodes.Newobj, ctorDestination);
                mappingMethodBodyIL.Emit(OpCodes.Stloc_0);
            }
        }
        
        private static void EmitEndMethod(ILGenerator mappingMethodBodyIL, Label endOfMethodLabel)
        {
            mappingMethodBodyIL.Emit(OpCodes.Ldloc_0);
            mappingMethodBodyIL.Emit(OpCodes.Stloc_1);

            mappingMethodBodyIL.Emit(OpCodes.Br_S, endOfMethodLabel);

            mappingMethodBodyIL.MarkLabel(endOfMethodLabel);
            mappingMethodBodyIL.Emit(OpCodes.Ldloc_1);
            mappingMethodBodyIL.Emit(OpCodes.Ret);
        }
    }
}
