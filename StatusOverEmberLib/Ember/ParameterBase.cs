namespace VizStatusOverEmberLib.Ember
{
    public abstract class ParameterBase : Element
    {
        protected ParameterBase(int number, Element parent, string identifier, Dispatcher dispatcher, bool isWriteable)
            : base(number, parent, identifier)
        {
            Dispatcher = dispatcher;
            IsWriteable = isWriteable;
        }

        public bool IsWriteable { get; }

        protected Dispatcher Dispatcher { get; }
    }
}
