using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cryptolens.Licensing.Tests;

internal sealed class LoopbackServer : IDisposable
{
    private readonly TcpListener listener;
    private readonly Task serverTask;
    private readonly int statusCode;
    private readonly string reasonPhrase;
    private readonly string responseBody;

    internal LoopbackServer(int statusCode, string reasonPhrase, string responseBody)
    {
        this.statusCode = statusCode;
        this.reasonPhrase = reasonPhrase;
        this.responseBody = responseBody;

        listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        BaseUrl = "http://127.0.0.1:" + port;
        serverTask = Task.Run(ServeOneRequestAsync);
    }

    internal string BaseUrl { get; }

    public void Dispose()
    {
        listener.Stop();

        try
        {
            serverTask.GetAwaiter().GetResult();
        }
        catch (ObjectDisposedException)
        {
        }
        catch (SocketException)
        {
        }
    }

    private async Task ServeOneRequestAsync()
    {
        using TcpClient client = await listener.AcceptTcpClientAsync();
        using NetworkStream stream = client.GetStream();

        await ReadRequestAsync(stream);

        byte[] bodyBytes = Encoding.UTF8.GetBytes(responseBody);
        string headers = string.Format(
            CultureInfo.InvariantCulture,
            "HTTP/1.1 {0} {1}\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: {2}\r\nConnection: close\r\n\r\n",
            statusCode,
            reasonPhrase,
            bodyBytes.Length);

        byte[] headerBytes = Encoding.ASCII.GetBytes(headers);
        await stream.WriteAsync(headerBytes);
        if (bodyBytes.Length > 0)
        {
            await stream.WriteAsync(bodyBytes);
        }

        await stream.FlushAsync();
    }

    private static async Task ReadRequestAsync(NetworkStream stream)
    {
        List<byte> request = new List<byte>();
        byte[] buffer = new byte[1024];
        int headerEnd = -1;
        int expectedLength = int.MaxValue;

        while (request.Count < expectedLength)
        {
            int read = await stream.ReadAsync(buffer);
            if (read == 0)
            {
                break;
            }

            for (int index = 0; index < read; index++)
            {
                request.Add(buffer[index]);
            }

            if (headerEnd < 0)
            {
                headerEnd = FindHeaderEnd(request);
                if (headerEnd >= 0)
                {
                    int contentLength = ParseContentLength(request, headerEnd);
                    expectedLength = headerEnd + 4 + contentLength;
                }
            }
        }
    }

    private static int FindHeaderEnd(List<byte> request)
    {
        for (int index = 0; index <= request.Count - 4; index++)
        {
            if (request[index] == 13 &&
                request[index + 1] == 10 &&
                request[index + 2] == 13 &&
                request[index + 3] == 10)
            {
                return index;
            }
        }

        return -1;
    }

    private static int ParseContentLength(List<byte> request, int headerEnd)
    {
        string headers = Encoding.ASCII.GetString(request.ToArray(), 0, headerEnd);
        foreach (string line in headers.Split(new[] { "\r\n" }, StringSplitOptions.None))
        {
            if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
            {
                return int.Parse(line.Substring("Content-Length:".Length).Trim(), CultureInfo.InvariantCulture);
            }
        }

        return 0;
    }
}
