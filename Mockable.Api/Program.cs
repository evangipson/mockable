using Mockable.Extensions;

namespace Mockable.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Add Mockable services to the services container
            builder.Services.AddMockable();
            
            var app = builder.Build();
            
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Add Mockable routes to the application
            app.AddMockableMiddleware();

            app.Run();
        }
    }
}
