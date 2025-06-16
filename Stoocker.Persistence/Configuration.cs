
using Microsoft.Extensions.Configuration; 

namespace Stoocker.Persistence
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; } 
    }
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

        static public JwtSettings GetJwtSettings()
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
            return configurationManager.GetSection("JwtSettings").Get<JwtSettings>();
        }
    }
}
