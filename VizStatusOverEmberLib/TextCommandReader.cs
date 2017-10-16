namespace VizStatusOverEmberLib
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public class TextCommandReader
    {
        public event EventHandler<CommandReceivedArgs> CommandReceived;

        public void Read(byte[] buffer, int count)
        {
            var text = Encoding.ASCII.GetString(buffer, 0, count);

            foreach (var line in Regex.Split(text, "(\n\r)|\0"))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                CommandReceived?.Invoke(this, new CommandReceivedArgs(new Command(line)));
            }
        }
    }
}
