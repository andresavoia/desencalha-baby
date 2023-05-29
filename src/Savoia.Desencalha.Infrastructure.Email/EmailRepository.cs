using Savoia.Desencalha.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Infrastructure.Email
{
    public class EmailRepository : IEmailRepository
    {
        public async Task EnviarAsync(string de, string deNome, List<string> para, List<string> copia, List<string> copiaOculta, List<string> anexo, string assunto, string corpo, string servidor, string usuario, string senha, int porta, bool SSL, bool UseDefaultCredentials, bool htmlCorpo)
        {
            MailMessage email = new MailMessage();

            email.From = new MailAddress(de, deNome);
            email.Subject = assunto;
            email.Body = corpo;

            if (para != null)
            {
                foreach (string item in para)
                {
                    email.To.Add(item);
                }
            }
            if (copia != null)
            {
                foreach (string item in copia)
                {
                    email.CC.Add(item);
                }
            }
            if (copiaOculta != null)
            {
                foreach (string item in copiaOculta)
                {
                    email.Bcc.Add(item);
                }
            }

            if (anexo != null)
            {
                foreach (string item in anexo)
                {
                    email.Attachments.Add(new Attachment(item));
                }
            }

            email.IsBodyHtml = htmlCorpo;

            SmtpClient smtp = new SmtpClient(servidor);
            //smtp.UseDefaultCredentials = UseDefaultCredentials;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Port = porta;
            smtp.Timeout = 10000;
            smtp.EnableSsl = SSL;
            smtp.Credentials = new System.Net.NetworkCredential(usuario, senha);

            smtp.Send(email);
        }
    }
}
