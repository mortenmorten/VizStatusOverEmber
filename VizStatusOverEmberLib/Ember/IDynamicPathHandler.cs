namespace VizStatusOverEmberLib.Ember
{
    using EmberLib.Glow;

    public interface IDynamicPathHandler
    {
        void HandleParameter(GlowParameterBase parameter, int[] path, Client source);

        void HandleCommand(GlowCommand command, int[] path, Client source);
    }
}