namespace VizStatusOverEmberLib.Socket
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using Ember;
    using VizStatusOverEmberLib;

    public class TextCommandClient : IDisposable
    {
        private readonly object _sync = new object();
        private readonly TextCommandReader _commandReader = new TextCommandReader();

        public TextCommandClient(TextCommandListener host, Socket socket, Dispatcher dispatcher)
        {
            Host = host;
            Socket = socket;
            Dispatcher = dispatcher;

            _commandReader.CommandReceived += (sender, args) => DoCommand(args.Command);
        }

        public TextCommandListener Host { get; private set; }

        public Socket Socket { get; private set; }

        public Dispatcher Dispatcher { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Read(byte[] buffer, int count)
        {
            _commandReader.Read(buffer, count);
        }

        private void DoCommand(Command command)
        {
            try
            {
                command.ThrowIfDefaultValues();

                switch (command.Category.ToLower())
                {
                    case "input":
                        var path = $"/vizengine/inputs/input{command.Name}/tally";
                        var status = command.Value.ToLower();
                        if (status == "active" || status == "inactive")
                        {
                            Send(
                                command.Id,
                                EmberTree.SetParameter(Dispatcher, path, status == "active") ? "success" : "failed");
                        }
                        else
                        {
                            Send(command.Id, "unknown status");
                        }

                        break;
                }
            }
            catch (Exception exception)
            {
                Send(command?.Id ?? -1, exception.Message);
            }
        }

        private void Send(int id, string message)
        {
            if (id < 0)
            {
                return;
            }

            Socket socket;
            TextCommandListener host;

            lock (_sync)
            {
                socket = Socket;
                host = Host;
            }

            if (socket == null)
            {
                return;
            }

            var buffer = Encoding.ASCII.GetBytes($"{id} {message}\n\r");

            try
            {
                socket.Send(buffer, buffer.Length, SocketFlags.None);
            }
            catch (SocketException)
            {
                host?.CloseClient(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Socket socket;

                lock (_sync)
                {
                    socket = Socket;

                    Socket = null;
                    Host = null;
                }

                if (socket != null)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
    }
}