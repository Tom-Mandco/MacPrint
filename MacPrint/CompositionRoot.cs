namespace Print
{
    using PrintLogger;
    using Ninject.Modules;

    public class CompositionRoot : NinjectModule
    {
        public override void Load()
        {
            Bind<INLogger>().To<NLogger>();
            Bind<IPrintUtilsHelper>().To<PrintUtilslHelper>();
        }
    }
}