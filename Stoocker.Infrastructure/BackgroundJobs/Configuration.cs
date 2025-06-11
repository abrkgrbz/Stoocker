using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {

                ConfigurationManager configurationManager = new();
                try
                {
                    configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Stoocker.API"));
                    configurationManager.AddJsonFile("appsettings.json");
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }

                return configurationManager.GetConnectionString("HangfireConnection");
            }
        }
    }
}
