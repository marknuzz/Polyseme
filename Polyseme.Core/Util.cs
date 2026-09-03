namespace Polyseme.Core
{
    public static class Util
    {
        public static string AssemblyName { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;

        public static void ConsoleLog(string message) => Console.WriteLine(message);
    }
}
