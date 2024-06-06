using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyGymProgress.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyGymProgress
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            //Configure DbContext with conditional connection string
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"] ?? "";
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;
            }
            //Add other service configurations
            //Configuration for DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder =>
                    builder.AllowAnyOrigin()   //Allows requests from any source
                           .AllowAnyMethod()   //Allows all HTTP methods
                           .AllowAnyHeader()); //Allows all headers
            });

            //Register the IHttpClientFactory
            services.AddHttpClient();

            services.AddHealthChecks();

            //Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Gym Progress API", Version = "v1" });

                //Configure Swagger to understand IFormFile
                c.SchemaFilter<SwaggerFileSchemaFilter>();

                // This part ensures that file parameters are described as file uploads
                c.OperationFilter<FileUploadOperation>();
            });

        }

        //This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Gym Progress API V1");
                c.RoutePrefix = string.Empty;  //Set the Swagger UI at the root
            });

            //To additionally serve Swagger at "/docs", setup another Swagger UI middleware
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Gym Progress API V1");
                c.RoutePrefix = "docs";  //Set the Swagger UI at the "/docs" path
            });




            app.UseCors("AllowAnyOrigin");

            app.UseEndpoints(endpoints =>
            { 
                endpoints.MapControllers();

                //Map health checks to a specific endpoint
                endpoints.MapHealthChecks("/health");
            });
        }
public class SwaggerFileSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                if (context.Type == typeof(IFormFile))
                {
                    schema.Type = "string";
                    schema.Format = "binary";
                }
            }
        }

        public class FileUploadOperation : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Parameters == null)
                    return;

                var formFileParameters = context.ApiDescription.ActionDescriptor.Parameters
                    .Where(param => param.ParameterType == typeof(IFormFile))
                    .Select(param => param.Name)
                    .ToList();

                var formFileParameterNames = operation.Parameters
                    .Where(param => formFileParameters.Contains(param.Name))
                    .Select(param => param.Name)
                    .ToList();

                foreach (var paramName in formFileParameterNames)
                {
                    var parameter = operation.Parameters.Single(p => p.Name == paramName);
                    operation.Parameters.Remove(parameter);
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                [paramName] = new OpenApiSchema
                                {
                                    Description = "Upload File",
                                    Type = "file",
                                    Format = "binary"
                                }
                            },
                            Required = new HashSet<string> { paramName }
                        }
                    }
                }
                    };
                }
            }
        }



    }


}
