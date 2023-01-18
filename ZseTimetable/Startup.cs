using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimetableLib.DataAccess;
using TimetableLib.DBAccess;
using ZseTimetable.Controllers;
using ZseTimetable.Services;

namespace ZseTimetable
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
            services.AddHttpClient("baseHttp",HttpClient => HttpClient.BaseAddress = new Uri("https://plan.zse.bydgoszcz.pl"));
            services.AddControllers();
            services.AddHostedService<TimetablesService>();
            //services.AddHostedService<ChangesService>();
            services.AddSingleton<IDataWrapper,DatabaseService>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}