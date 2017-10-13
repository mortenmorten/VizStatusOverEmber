namespace VizStatusOverEmberLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EmberLib.Framing;

    public static class Arguments
    {
        /// <summary>
        /// Parses the command line arguments and fills some out variables
        /// with the parsed information.
        /// </summary>
        /// <param name="args">Command line arguments as passed to the entry point method.</param>
        /// <param name="emberPort">Receives the port number to listen to for EmBER requests.</param>
        /// <param name="maxPackageLength">Receives the maximum package length for tx packages.</param>
        /// <param name="textPort">Receives the port number to listen to for socket text commands.</param>
        public static void Parse(IEnumerable<string> args, out int emberPort, out int maxPackageLength, out int textPort)
        {
            emberPort = 9098;
            maxPackageLength = ProtocolParameters.MaximumPackageLength;
            textPort = emberPort + 1;

            var argTokens = from arg in args
                where arg.StartsWith("-") || arg.StartsWith("/")
                let tokens = arg.Split('=')
                where tokens.Length == 2
                select Tuple.Create(tokens[0].ToLower().TrimStart('-', '/'), tokens[1]);

            foreach (var token in argTokens)
            {
                switch (token.Item1)
                {
                    case "emberport":
                        int.TryParse(token.Item2, out emberPort);
                        break;
                    case "maxpackagelength":
                        int.TryParse(token.Item2, out maxPackageLength);
                        break;
                    case "textport":
                        int.TryParse(token.Item2, out textPort);
                        break;
                }
            }
        }
    }
}
