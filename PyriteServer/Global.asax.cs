﻿// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="Global.asax.cs" company="Microsoft Corporation">
// // // Copyright (c) Microsoft Corporation. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace PyriteServer
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http.Formatting;
    using System.Web;
    using System.Web.Http;
    using PyriteServer.Contracts;
    using PyriteServer.DataAccess;

    public class WebApiApplication : HttpApplication
    {
        private bool disposed = false;
        private UriStorage storage = null;

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Application_Start()
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, ".\\Data");
            dataPath = Path.GetFullPath(dataPath);
            Trace.WriteLine(String.Format("Data path: {0}", dataPath));

            //string connSecretsPath = Path.Combine(dataPath, "accountkey.txt");
            //ISecretsProvider connProvider = new FileSecretsProvider(connSecretsPath);

            // .NET AppSettings provider
            ISecretsProvider connProvider = new AzureAppSettingsProvider();

            string setRootUrl = ConfigurationManager.AppSettings["SetRootUrl"];
            if (string.IsNullOrWhiteSpace(setRootUrl))
            {
                throw new ConfigurationErrorsException("SetRootUrl not specified");
            }

            if (connProvider.Exists)
            {
                this.storage = new AzureUriStorage(connProvider.Value, setRootUrl);
            }
            else
            {
                this.storage = new UriStorage(setRootUrl);
            }

            Dependency.Storage = this.storage;

            // wait reasonable amount of time for first load
            this.storage.WaitLoadCompleted.WaitOne(30000);

            GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            GlobalConfiguration.Configuration.Formatters.Add(new BsonMediaTypeFormatter());
            GlobalConfiguration.Configuration.EnsureInitialized();
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || this.disposed)
            {
                return;
            }

            if (this.storage != null)
            {
                this.storage.Dispose();
            }

            this.disposed = true;

            base.Dispose();
        }
    }
}