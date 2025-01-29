using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Gelf.Extensions.Logging;

public class TcpTlsGelfClient : TcpGelfClient
{
    public TcpTlsGelfClient(GelfLoggerOptions options) : base(options)
    {
    }

    protected override Stream GetStream(bool recreate)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(_options.CertificatePath));
        ArgumentException.ThrowIfNullOrEmpty(nameof(_options.CertificateKeyPath));

        if (recreate || _client == null || _stream == null || !_client.Connected)
        {
            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch
            {
                // Ignore any error during the closing of the client or stream
            }

            _client = new TcpClient(_options.Host!, _options.Port);

            var certificateContents = File.ReadAllText(_options.CertificatePath);
            var keyContents = File.ReadAllText(_options.CertificateKeyPath);
            var clientCertificate = X509Certificate2.CreateFromPem(certificateContents, keyContents);
            var pfxCertificate = new X509Certificate2(clientCertificate.Export(X509ContentType.Pfx));
            var clientCertificates = new X509CertificateCollection { pfxCertificate };

            var sslStream = new SslStream(_client.GetStream(), false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate));

            // Authenticate the client
            sslStream.AuthenticateAsClient(_options.Host, clientCertificates, System.Security.Authentication.SslProtocols.Tls12, false);

            _stream = sslStream;
        }

        return _stream;
    }

    private static bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        return sslPolicyErrors == SslPolicyErrors.None; // Modify this to allow specific errors if necessary
    }
}
