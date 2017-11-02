namespace VizStatusOverEmberLib.Socket
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using Ember;

    public class TextCommandListener : IDisposable
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<TextCommandClient> _clients = new List<TextCommandClient>();
        private readonly byte[] _buffer = new byte[1024];
        private readonly object _sync = new object();

        public TextCommandListener(int port, Dispatcher dispatcher)
        {
            Port = port;
            Dispatcher = dispatcher;

            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptSocket(AcceptCallback, listener);
        }

        public int Port { get; }

        public Dispatcher Dispatcher { get; }

        public void CloseClient(TextCommandClient client)
        {
            Log.DebugFormat("Closing client {0}", client.Socket.RemoteEndPoint);

            lock (_sync)
            {
                _clients.Remove(client);
            }

            client.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (_sync)
            {
                foreach (var client in _clients)
                {
                    client.Dispose();
                }

                _clients.Clear();
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            var listener = (TcpListener)result.AsyncState;

            try
            {
                var socket = listener.EndAcceptSocket(result);
                var client = new TextCommandClient(this, socket, Dispatcher);

                Log.DebugFormat("Accepting connection from {0}", client.Socket.RemoteEndPoint);

                lock (_sync)
                {
                    _clients.Add(client);
                }

                listener.BeginAcceptSocket(AcceptCallback, listener);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
            }
            catch (SocketException ex)
            {
                Log.Error("Accept error", ex);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            var client = (TextCommandClient)result.AsyncState;
            var socket = client.Socket;

            if (socket == null)
            {
                return;
            }

            try
            {
                var count = socket.EndReceive(result);

                if (count > 0)
                {
                    client.Read(_buffer, count);

                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
                }
                else
                {
                    CloseClient(client);
                }
            }
            catch (SocketException)
            {
                CloseClient(client);
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}
