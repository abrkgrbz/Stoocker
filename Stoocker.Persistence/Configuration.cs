
using Microsoft.Extensions.Configuration; 

namespace Stoocker.Persistence
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
                catch(Exception e)
                {
                   Console.Write(e);
                }

                return configurationManager.GetConnectionString("SqlConnection");
            }
        }
    }
}
