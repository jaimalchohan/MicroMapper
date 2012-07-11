namespace MicroMapper.Tests.Speed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AutoMapper;

    class Program
    {
        static void Main(string[] args)
        {
            Mapper.CreateMap<Source, Destination>()
                    .ForMember(d => d.SourceProperty, o => o.MapFrom(s => s.SourceProperty));

            var source = new Source() { SourceProperty = "jaimal" };

            long total = 0;

            for (int i = 0; i < 10000; i++)
            {
                var start = DateTime.Now;

                var r = Mapper.Map<Source, Destination>(source);

                var end = DateTime.Now;

                total += (end - start).Ticks;
            }

            Console.WriteLine(total / 10000);

            total = 0;

            MicroMapper.CreateMap<Source, Destination>()
                .ForMember(o => o.MapFrom(s => s.SourceProperty), d => d.SourceProperty);

            MicroMapper.Init();

            for (int i = 0; i < 10000; i++)
            {
                var start = DateTime.Now;

                var r = MicroMapper.Map<Source, Destination>(source);

                var end = DateTime.Now;

                total += (end - start).Ticks;
            }

            Console.WriteLine(total / 10000);

            Console.ReadLine();

        }
    }
}