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

        public TextCommandClient(TextCommandListener host, Socket socket, Dispatcher dispatcher)
        {
            Host = host;
            Socket = socket;
            Dispatcher = dispatcher;
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
            var text = Encoding.ASCII.GetString(buffer, 0, count);

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var parts = text.Split(' ');

            var response = "unknown";
            if (parts.Length > 0)
            {
                switch (parts[0].ToLower())
                {
                    case "input":
                        try
                        {
                            if (parts.Length > 1)
                            {
                                if (!int.TryParse(parts[1], out var inputNo))
                                {
                                    response = "bad input format";
                                }
                                else
                                {
                                    if (parts.Length > 2)
                                    {
                                        var path = $"/vizengine/inputs/input{inputNo}/tally";
                                        var status = parts[2].ToLower();
                                        if (status == "active" || status == "inactive")
                                        {
                                            response = EmberTree.SetParameter(
                                                Dispatcher,
                                                path,
                                                status == "active")
                                                ? "success"
                                                : "failed";
                                        }
                                        else
                                        {
                                            response = "unknown status";
                                        }
                                    }
                                    else
                                    {
                                        response = "missing status";
                                    }
                                }
                            }
                            else
                            {
                                response = "missing input";
                            }
                        }
                        catch
                        {
                            response = "bad format";
                        }

                        break;
                }
            }

            Send(response);
        }

        private void Send(string message)
        {
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

            var buffer = Encoding.ASCII.GetBytes(message + "\n\r");

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