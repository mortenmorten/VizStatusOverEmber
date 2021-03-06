﻿namespace VizStatusOverEmberLib.Ember
{
    public class StringParameter : Parameter<string>
   {
      public StringParameter(int number, Element parent, string identifier, Dispatcher dispatcher, bool isWriteable)
      : base(number, parent, identifier, dispatcher, isWriteable)
      {
      }

      public override TResult Accept<TState, TResult>(IElementVisitor<TState, TResult> visitor, TState state)
      {
         return visitor.Visit(this, state);
      }
   }
}
