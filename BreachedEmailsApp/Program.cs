using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;

namespace BreachedEmailsApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            //build the silo
            .UseOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();

                //konfiguracija storage providerja za persistanje grain state - a - seznama breached emailov(v table storage)
                // ko je to skonfigurirano lahko dostopamo do tega storage providerja iz graina
                siloBuilder.AddAzureBlobGrainStorage(      // BLOB: siloBuilder.AddAzureBlobGrainStorage TABLE: AddAzureTableGrainStorage
                name: "emailsStore",
                configureOptions: options =>
                {
                    // Use JSON for serializing the state in storage
                    options.UseJson = true;

                    // Configure the storage connection key
                    // TODO: change credentials accordingly
                    string accountName = "myAccountName";
                    string accountKey = "myAccountKey";
                    string conn_string = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, accountKey);
                    options.ConnectionString = conn_string;    
                });
            });
    }
}
