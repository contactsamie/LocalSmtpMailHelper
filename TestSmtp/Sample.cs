using System;
using System.Net.Mail;
using Akka.Actor;
using Microsoft.Owin.Hosting;
using NLog;

namespace TestSmtp
{
    public class Sample
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Start(string serverEndPoint = null, string serverActorSystemName = null, ActorSystem serverActorSystem = null, string serverActorSystemConfig = null)
        {
            try
            {
                OwinRef = WebApp.Start(serverEndPoint, (appBuilder) =>
                {
                    try
                    {
                        var smtp = new SmtpClient();
                        var message = new MailMessage("me@gmail.com", "me@yahoo.com", "My Message Subject", "This is a test message");
                        smtp.Send(message);
                    }
                    catch (Exception e)
                    {
                        //todo log here
                        throw;
                    }
                });

                Log.Debug("initialized successfully");
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        protected IDisposable OwinRef { get; set; }

        public void Stop()
        {
            OwinRef?.Dispose();
        }
    }
}