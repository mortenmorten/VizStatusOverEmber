namespace StatusOverEmberService
{
    using System.ServiceProcess;
    using VizStatusOverEmberLib;

    public partial class VizStatusOverEmberService : ServiceBase
    {
        private StatusOverEmber statusOverEmber;

        public VizStatusOverEmberService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            statusOverEmber?.Dispose();
            statusOverEmber = StatusOverEmber.Start(args);
        }

        protected override void OnStop()
        {
            statusOverEmber?.Dispose();
            statusOverEmber = null;
        }
    }
}
