using System;

namespace GetInstanceTest
{
    using static Console;

    class Program
    {
        private string _data;

        public static readonly Program Field = new Program(nameof(Field));
        public static Program Property { get; } = new Program(nameof(Property));

        public static Program Get()
            => new Program("*");
        public static Program Get(string data)
            => new Program(data);

        public Program() { }
        public Program(string data)
        {
            _data = data;
        }

        public override string ToString()
            => $"{{{ (_data == null ? null : " \"" + _data + '"') } }}";

        static void Main(string[] args)
        {
            var type = typeof(Program);
            WriteLine(TypeUtility.GetInstance<object>(type, null));
            WriteLine(TypeUtility.GetInstance(type, type, null, ".ctor"));

            WriteLine(TypeUtility.GetInstance<object>(type, nameof(Field)));
            WriteLine(TypeUtility.GetInstance(type, type, nameof(Property)));

            WriteLine(TypeUtility.GetInstance<object>(type, nameof(Get)));
            WriteLine(TypeUtility.GetInstance(type, type, nameof(Get), nameof(Get)));

            ReadKey();
        }
    }
}
