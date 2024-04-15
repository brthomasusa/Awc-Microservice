// using AWC.Server.Contracts;
// using AWC.Server.Extensions;
// using AWC.Server.Interceptors;
// using AWC.Server.Middleware;
using MediatR;
using NLog;
using NLog.Web;

namespace AWC.EmployeeMgmt.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllersWithViews();
            services.AddRazorPages();


        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}