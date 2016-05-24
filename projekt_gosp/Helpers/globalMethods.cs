using projekt_gosp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace projekt_gosp.Helpers
{
    public static class GlobalMethods
    {
        public static Object o = new Object();

        public static int GetShopId(int userid, db context, bool isAuthenticated, HttpSessionStateBase session)
        {
            int shopid = 0;
            if (isAuthenticated)
            {
                shopid = (from p in context.Uzytkownicy
                              where p.ID_klienta == userid
                              select p.selectedShopId).FirstOrDefault();
                if (shopid == 0)
                {
                    shopid = GlobalMethods.GetDefaultShopId(context);
                }
            }
            else
            {
                if (session["shopid"] == null)
                {
                    shopid = GlobalMethods.GetDefaultShopId(context);
                }
                else
                {
                    shopid = Int32.Parse(session["shopid"].ToString());
                }
            }
            return shopid;
        }

        public static int GetDefaultShopId(db context)
        {
            int defaultId = (from p in context.Sklepy
                             select p.ID_sklepu).First();

            return defaultId;
        }

        public static void SendMailThread(string emailAddress, string subject, string body)
        {
            additionalModels.emailContent e = new additionalModels.emailContent
            {
                emailAddress = emailAddress,
                subject = subject,
                body = body,
            };
            Thread t = new Thread(SendMail);
            t.Start(e);
        }

        private static void SendMail(Object parameter)
        {

            additionalModels.emailContent emailContent = (additionalModels.emailContent)parameter;

            string line;
            List<string> senderEmail = new List<string>(4);

            using (System.IO.StreamReader file = new System.IO.StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/mail/mailconfig.txt")))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                        continue;

                    senderEmail.Add(line);
                }
            }

            var message = new MailMessage();
            message.To.Add(new MailAddress(emailContent.emailAddress));
            message.From = new MailAddress(senderEmail[0]);
            message.Subject = emailContent.subject;
            message.Body = emailContent.body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = senderEmail[0],
                    Password = senderEmail[1]
                };
                smtp.Credentials = credential;
                smtp.Host = senderEmail[2];
                smtp.Port = Int32.Parse(senderEmail[3]);
                smtp.EnableSsl = true;
                try
                {
                    lock (o)
                    {
                        smtp.Send(message);
                    }
                }
                catch (System.Net.Mail.SmtpException e)
                {
                    Debug.Write(e.Message);
                }
            }
        }

        public static void SendSmsToClientThread(string phoneNumber, string message)
        {
            additionalModels.sms e = new additionalModels.sms
            {
                phoneNumber = phoneNumber,
                message = message
            };
            //Thread t = new Thread(SendSmsToClient);
            //t.Start(e);
        }

        private static void SendSmsToClient(Object obj)
        {
            additionalModels.sms sms = (additionalModels.sms)obj;

            try
            {
                SMSApi.Api.Client client = new SMSApi.Api.Client("rwiktorek@wp.pl");
                client.SetPasswordHash("931bd0e1cc9baae10e9ff6ca7002e834");

                var smsApi = new SMSApi.Api.SMSFactory(client);

                var result =
                    smsApi.ActionSend()
                        .SetText(sms.message)
                        .SetTo(sms.phoneNumber)
                        .SetSender("ECO")
                        //.SetSender("e-Warzywko") //Pole nadawcy lub typ wiadomość 'ECO', '2Way'
                        .Execute();
            }
            catch (SMSApi.Api.ActionException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.ClientException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.HostException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.ProxyException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
}