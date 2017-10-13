namespace VizStatusOverEmberLib
{
    using System;
    using System.Collections.Generic;
    using Ember;
    using EmberLib.Framing;
    using EmberLib.Glow.Framing;
    using Socket;

    public class StatusOverEmber : IDisposable
    {
        private readonly GlowListener _glowListener;
        private readonly TextCommandListener _textCommandListener;

        private StatusOverEmber(int emberPort, int textPort, int maxPackageLength = ProtocolParameters.MaximumPackageLength)
        {
            var dispatcher = new Dispatcher();
            dispatcher.Root = EmberTree.Create(dispatcher);

            EmberPort = emberPort;
            TextPort = textPort;

            _glowListener = new GlowListener(emberPort, maxPackageLength, dispatcher);
            _textCommandListener = new TextCommandListener(textPort, dispatcher);
        }

        public int EmberPort { get; }

        public int TextPort { get; }

        public static string GetEmberInfo()
        {
            return
                $"GlowDTD v{GlowReader.UshortVersionToString(EmberLib.Glow.GlowDtd.Version)} - EmBER v{GlowReader.UshortVersionToString(EmberLib.EmberEncoding.Version)}";
        }

        public static StatusOverEmber Start(IEnumerable<string> args)
        {
            Arguments.Parse(args, out var emberPort, out var maxPackageLength, out var textPort);
            return new StatusOverEmber(emberPort, textPort);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textCommandListener.Dispose();
                _glowListener.Dispose();
            }
        }
    }
}
