namespace MacPrint
{
    using Ninject.Modules;
    using NLog;
    using Classes;
    using Interfaces;
    using System;
    using System.Configuration;

    class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            var emailPort = Convert.ToInt32(ConfigurationManager.AppSettings["sendEmailPort"]);
            var emailHost = ConfigurationManager.AppSettings["sendEmailHost"];

            Bind<ILog>().ToMethod(x =>
            {
                var scope = x.Request.ParentRequest.Service.FullName;
                var log = (ILog)LogManager.GetLogger(scope, typeof(Log));
                return log;
            });
            Bind<IApp>().To<App>();
            Bind<IPurgeFiles>().To<PurgeFiles>();
            Bind<IPrintHandler>().To<PrintHandler>();
            Bind<IEmailSender>().To<EmailSender>().WithConstructorArgument("clientPort", emailPort).WithConstructorArgument("clientHost", emailHost);
        }
    }
}
