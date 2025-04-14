using Doctor_App.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Doctor_App.Data;
using Doctor_App.Infrastructure.Data;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Infrastructure.Data.Common;
using Doctor_App.Core.Services.PatientServices;
using Microsoft.AspNetCore.Hosting;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Core.Services.AppointmentService;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Core.Services.BillingServices;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DoctorAppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDataProtection();
// Add services to the container.
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DoctorAppDbContext>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
});
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<DoctorAppDbContext>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalRecordService,  MedicalRecordService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddRazorPages();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await Doctor_App.Infrastructure.SeedData.SeedRolesAndAdminAsync(services);
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();