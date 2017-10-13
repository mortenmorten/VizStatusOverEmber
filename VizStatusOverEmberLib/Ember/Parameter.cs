namespace VizStatusOverEmberLib.Ember
{
    public abstract class Parameter<T> : ParameterBase
    {
        private T _value;

        protected Parameter(int number, Element parent, string identifier, Dispatcher dispatcher, bool isWriteable)
        : base(number, parent, identifier, dispatcher, isWriteable)
        {
        }

        public T Value
        {
            get => _value;
            set
            {
                lock (SyncRoot)
                {
                    _value = value;

                    Dispatcher.NotifyParameterValueChanged(this);
                }
            }
        }
    }
}
