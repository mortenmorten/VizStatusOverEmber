namespace VizStatusOverEmberConsole
{
    using System;
    using VizStatusOverEmberLib;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Viz Engine Status over EmBER v{0} ({1}) started.",
                typeof(Program).Assembly.GetName().Version,
                StatusOverEmber.GetEmberInfo());

            using (var service = StatusOverEmber.Start(args))
            {
                Console.WriteLine("EmBER listening on port {0}. ", service.EmberPort);
                Console.WriteLine("Text input listening on port {0}", service.TextPort);

                Console.WriteLine("Press Enter to quit...");
                Console.ReadLine();
            }
        }
    }
}
