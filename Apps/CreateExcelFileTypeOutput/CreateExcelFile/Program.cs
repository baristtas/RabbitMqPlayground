using CreateExcelFile.Models;
using ImageWatermarkRabbitMQ.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CreateExcelFile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string uri = "";
            try
            {
                StreamReader sr = new StreamReader("C:\\Users\\baris.tas\\Desktop\\rbmq\\amqpinstanceuri.txt");
                uri = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL1"));

            });
            builder.Services.AddControllersWithViews();
            
            builder.Services.AddSingleton(sp => new ConnectionFactory()
            {
                Uri = new Uri(uri),
                DispatchConsumersAsync = true
            });

            builder.Services.AddSingleton<RabbitMQClientService>();
            
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<AppDbContext>();



            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                appDbContext.Database.Migrate();

                if(!appDbContext.Users.Any()) 
                {
                    userManager.CreateAsync(new IdentityUser()
                    {
                        UserName = "test1"
                    ,
                        Email = "test1@gmail.com"
                    }, password:"Password12*").Wait();

                    userManager.CreateAsync(new IdentityUser()
                    {
                        UserName = "test2"
,
                        Email = "test2@gmail.com"
                    }, password: "Password12*").Wait();
                }
            }



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            

            app.Run();
        }
    }
}