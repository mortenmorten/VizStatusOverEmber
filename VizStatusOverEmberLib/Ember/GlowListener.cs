namespace VizStatusOverEmberLib.Ember
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class GlowListener : IDisposable
    {
        private readonly List<Client> _clients = new List<Client>();
        private readonly byte[] _buffer = new byte[1024];
        private readonly object _sync = new object();

        public GlowListener(int port, int maxPackageLength, Dispatcher dispatcher)
        {
            Port = port;
            MaxPackageLength = maxPackageLength;
            Dispatcher = dispatcher;

            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptSocket(AcceptCallback, listener);

            dispatcher.GlowRootReady += Dispatcher_GlowRootReady;
        }

        public int Port { get; }

        public int MaxPackageLength { get; }

        public Dispatcher Dispatcher { get; }

        public void CloseClient(Client client)
        {
            lock (_sync)
            {
                _clients.Remove(client);
            }

            client.Dispose();
        }

        public void Dispose()
        {
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
                var client = new Client(this, socket, MaxPackageLength, Dispatcher);

                lock (_sync)
                {
                    _clients.Add(client);
                }

                listener.BeginAcceptSocket(AcceptCallback, listener);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Accept error: {0}", ex);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            var client = (Client)result.AsyncState;
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

        private void Dispatcher_GlowRootReady(object sender, GlowRootReadyArgs e)
        {
            lock (_sync)
            {
                foreach (var client in _clients)
                {
                    if (client != e.SourceClient)
                    {
                        client.Write(e.Root);
                    }
                }
            }
        }
    }
}
