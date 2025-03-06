using ASPShopBag.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_Muffin_Kingdon.Areas.Identity.Pages.Account;
using MVC_Muffin_Kingdon.Data;


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Добавяне на Identity услуги
builder.Services.AddDefaultIdentity<User>(options =>
                                options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


// Добавяне на допълнителни услуги и конфигурации
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//IMvcBuilder buildMVC = builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers(
    options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

//if (builder.Environment.IsDevelopment())
//{
//    builder.AddRazorRuntimeCompilation();
//}

var app = builder.Build();

// Конфигурация на мидълуерите и HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.PrepareDataBase().Wait();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



