using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyMATEUpload.Data;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Services;
using System.Text;

namespace StudyMATEUpload
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[AppConstant.Secret]));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //Validate credentials
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        //set valid params
                        ValidIssuer = Configuration[AppConstant.Issuer],
                        ValidAudience = Configuration[AppConstant.Audience],
                        IssuerSigningKey = securityKey
                    };
                });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudyMATE API", Version = "v1" });
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IModelManager<Course>, ModelManager<Course>>();
            services.AddTransient<IModelManager<Test>, ModelManager<Test>>();
            services.AddTransient<IModelManager<Quiz>, ModelManager<Quiz>>();
            services.AddTransient<IModelManager<Option>, ModelManager<Option>>();
            services.AddTransient<IModelManager<UserCourse>, ModelManager<UserCourse>>();
            services.AddTransient<IModelManager<ApplicationUser>, ModelManager<ApplicationUser>>();
            services.AddTransient<IModelManager<Award>, ModelManager<Award>>();
            services.AddTransient<IModelManager<UserAward>, ModelManager<UserAward>>();
            services.AddTransient<IModelManager<Feedback>, ModelManager<Feedback>>();
            //services.AddTransient<IModelManager<LearnCourse>, ModelManager<LearnCourse>>();
            //services.AddTransient<IModelManager<UserLearnCourse>, ModelManager<UserLearnCourse>>();
            services.AddTransient<IModelManager<Referral>, ModelManager<Referral>>();
            services.AddTransient<IModelManager<Subscription>, ModelManager<Subscription>>();
            services.AddTransient<IModelManager<UserFeedback>, ModelManager<UserFeedback>>();
            services.AddTransient<IModelManager<UserQuiz>, ModelManager<UserQuiz>>();
            services.AddTransient<IModelManager<UserTest>, ModelManager<UserTest>>();
            services.AddTransient<IModelManager<UserSubscription>, ModelManager<UserSubscription>>();
            services.AddTransient<IModelManager<UserVideo>, ModelManager<UserVideo>>();
            services.AddTransient<IModelManager<UserPreference>, ModelManager<UserPreference>>();
            services.AddTransient<IModelManager<Preference>, ModelManager<Preference>>();
            services.AddTransient<IModelManager<Video>, ModelManager<Video>>();
            services.AddTransient<AuthRepository>();
            services.AddTransient<UserManager>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEmailSender, EmailSender>();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
