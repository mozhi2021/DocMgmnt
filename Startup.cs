using DocMgmnt.Interface;
using DocMgmnt.Repositories;
using DocMgmnt.Models;
using DocMgmnt.Middleware;
using Amazon.S3;

namespace DocMgmnt;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        var builder = WebApplication.CreateBuilder();

        services.AddControllers();

        //services.Configure<AWSConfig>(builder.Configuration.GetSection("AWSCredentials"));

        services.Configure<AWSConfig>(builder.Configuration.GetSection("DevAwsObjects"));
        services.AddCors(options => options.AddDefaultPolicy(
                       builder => builder
                                   .AllowAnyMethod()
                                   .AllowAnyHeader()
                                   .AllowAnyOrigin()));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IDocumentHandler, DocumentHandler>();
        services.AddScoped<IApiHandler, ApiHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors();

        app.UseMiddleware<ApiKeyMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}