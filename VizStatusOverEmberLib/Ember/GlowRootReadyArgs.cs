namespace VizStatusOverEmberLib.Ember
{
    using System;
    using EmberLib.Glow;

    public class GlowRootReadyArgs : EventArgs
    {
        public GlowRootReadyArgs(GlowContainer root, Client sourceClient)
        {
            Root = root;
            SourceClient = sourceClient;
        }

        public GlowContainer Root { get; }

        public Client SourceClient { get; }
    }
}
