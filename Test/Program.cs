using Microsoft.EntityFrameworkCore;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.MapControllers();

            app.Run();
        }
    }
}
