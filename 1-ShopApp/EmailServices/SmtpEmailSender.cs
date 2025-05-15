using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace _1_ShopApp.EmailServices
{
    public class SmtpEmailSender : IEmailSender
{
    private readonly string _host;
    private readonly int _port;
    private readonly bool _enableSSL;
    private readonly string _userName;
    private readonly string _password;

    public SmtpEmailSender(string host, int port, bool enableSSL, string userName, string password)
    {
        _host = host;
        _port = port;
        _enableSSL = enableSSL;  // TLS aktif olması için SSL'i kullanmıyoruz.
        _userName = userName;
        _password = password;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        using var client = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_userName, _password),
            EnableSsl = _enableSSL  // SSL/TLS kullanımı buradan yönetilir.
        };

        var message = new MailMessage
        {
            From = new MailAddress(_userName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}
    }
