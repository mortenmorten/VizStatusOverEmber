﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace StatusOverEmberService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        internal static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new VizStatusOverEmberService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
