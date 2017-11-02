namespace VizStatusOverEmberLib.Ember
{
    using System;
    using System.Net.Sockets;
    using System.Xml;
    using EmberLib;
    using EmberLib.Framing;
    using EmberLib.Glow;
    using EmberLib.Glow.Framing;
    using EmberLib.Xml;

    public class Client : IDisposable
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _sync = new object();
        private GlowReader _reader;

        public Client(GlowListener host, Socket socket, int maxPackageLength, Dispatcher dispatcher)
        {
            Host = host;
            Socket = socket;
            MaxPackageLength = maxPackageLength;
            Dispatcher = dispatcher;

            _reader = new GlowReader(GlowReader_RootReady, GlowReader_KeepAliveRequestReceived);
            _reader.Error += GlowReader_Error;
            _reader.FramingError += GlowReader_FramingError;
        }

        public GlowListener Host { get; private set; }

        public Socket Socket { get; private set; }

        public int MaxPackageLength { get; }

        public Dispatcher Dispatcher { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Read(byte[] buffer, int count)
        {
            GlowReader reader;

            lock (_sync)
            {
                reader = _reader;
                Log.DebugFormat("Received {0} bytes from {1}", count, Socket.RemoteEndPoint);
            }

            reader?.ReadBytes(buffer, 0, count);
        }

        public void Write(GlowContainer glow)
        {
            var output = CreateOutput();

            glow.Encode(output);

            output.Finish();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Socket socket;
            GlowReader reader;

            lock (_sync)
            {
                socket = Socket;
                reader = _reader;

                Socket = null;
                _reader = null;
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

            reader?.Dispose();
        }

        private void GlowReader_RootReady(object sender, AsyncDomReader.RootReadyArgs e)
        {
            if (e.Root is GlowContainer root)
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    IndentChars = "  ",
                };

                using (var writer = XmlWriter.Create(Console.Out, settings))
                {
                    XmlExport.Export(root, writer);
                }

                Dispatcher.DispatchGlow(root, this);
            }
            else
            {
                Log.WarnFormat("Unexpected Ember Root: {0} ({1})", e.Root, e.Root.GetType());
            }
        }

        private void GlowReader_Error(object sender, GlowReader.ErrorArgs e)
        {
            Log.WarnFormat("GlowReader error {0}: {1}", e.ErrorCode, e.Message);
        }

        private void GlowReader_FramingError(object sender, FramingReader.FramingErrorArgs e)
        {
            Log.WarnFormat("GlowReader framing error: {0}", e.Message);
        }

        private void GlowReader_KeepAliveRequestReceived(object sender, FramingReader.KeepAliveRequestReceivedArgs e)
        {
            Socket socket;

            lock (_sync)
            {
                socket = Socket;
            }

            socket?.Send(e.Response, e.ResponseLength, SocketFlags.None);
        }

        private GlowOutput CreateOutput()
        {
            return new GlowOutput(
                MaxPackageLength,
                0,
                (_, e) =>
                {
                    Socket socket;
                    GlowListener host;

                    lock (_sync)
                    {
                        socket = Socket;
                        host = Host;
                    }

                    if (socket == null)
                    {
                        return;
                    }

                    try
                    {
                        socket.Send(e.FramedPackage, e.FramedPackageLength, SocketFlags.None);
                    }
                    catch (SocketException)
                    {
                        host?.CloseClient(this);
                    }
                });
        }
    }
}
