using LocalSmtpMailHelperLib;
using Topshelf;

namespace TestSmtp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LocalSmtpMailHelper.SetRelativePickupDirectoryLocation("Mails");
            HostFactory.Run(x => //1
            {
                x.Service<Sample>(s => //2
                {
                    s.ConstructUsing(name => new Sample()); //3
                    s.WhenStarted(tc => tc.Start()); //4
                    s.WhenStopped(tc => tc.Stop()); //5
                });
                x.RunAsLocalSystem(); //6
                x.UseNLog();
                x.SetDescription("Sample"); //7
                x.SetDisplayName("Sample"); //8
                x.SetServiceName("Sample"); //9
            });
        }
    }
}