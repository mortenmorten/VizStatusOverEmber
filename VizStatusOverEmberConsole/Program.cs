namespace VizStatusOverEmberConsole
{
    using System;
    using VizStatusOverEmberLib;

    internal class Program
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            Log.InfoFormat("Viz Engine Status over EmBER v{0} ({1}) started.",
                typeof(Program).Assembly.GetName().Version,
                StatusOverEmber.GetEmberInfo());

            using (var service = StatusOverEmber.Start(args))
            {
                Log.InfoFormat("EmBER listening on port {0}. ", service.EmberPort);
                Log.InfoFormat("Text input listening on port {0}", service.TextPort);

                Log.InfoFormat("Press Enter to quit...");
                Console.ReadLine();
            }
        }
    }
}
