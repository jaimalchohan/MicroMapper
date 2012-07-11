namespace MicroMapper.Feasability
{
    using System;
    using System.Linq;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;


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
    }

    public class Destination
    {
        public string SourceProperty { get; set; }
    }

}
