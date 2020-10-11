using Common;
using Common.Middlewares;
using ExecutionService.Hubs;
using ExecutionService.Middlewares;
using ExecutionService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExecutionService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddCors(options => options.AddPolicy("WebAppPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(Configuration.GetValue<string>("WebAppUrl")) ;
            }));
            services.AddSignalR();
            services.AddCommonServices(Configuration);
            services.AddSingleton<WindowsExecutionService>();
            services.AddSingleton<DockerExecutionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseMiddleware<SignalRExceptionHandlerMiddleware>();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                //app.UseMiddleware<SignalRExceptionHandlerMiddleware>();
                app.UseHsts();
            }

            app.UseCors("WebAppPolicy");
            app.UseHttpsRedirection();            
            app.UseMiddleware<JWTCookieToHeaderMiddleware>();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ExecutionHub>("/ExecutionHub");
            });
        }
    }
}
