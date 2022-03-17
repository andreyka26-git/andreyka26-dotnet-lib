using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthorizationSample.Infrastructure;
using AuthorizationSample.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection");

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityContext>();

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Configure Identity to use the same JWT claims as OpenIddict instead
//    // of the legacy WS-Federation claims it uses by default (ClaimTypes),
//    // which saves you from doing the mapping in your authorization controller.
//    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
//    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
//    options.ClaimsIdentity.RoleClaimType = Claims.Role;
//    options.ClaimsIdentity.EmailClaimType = Claims.Email;

//    // Note: to require account confirmation before login,
//    // register an email sender service (IEmailSender) and
//    // set options.SignIn.RequireConfirmedAccount to true.
//    //
//    // For more information, visit https://aka.ms/aspaccountconf.
//    options.SignIn.RequireConfirmedAccount = false;
//});

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

var provider = app.Services.GetRequiredService<IServiceProvider>();
using (var scope = provider.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var user = new ApplicationUser
    {
        UserName = "b.andriy.b2000@gmail.com",
        Email = "b.andriy.b2000@gmail.com",
        //NormalizedEmail = "B.ANDRIY.B2000@GMAIL.COM",
        EmailConfirmed = true
    };

    var res = await userManager.CreateAsync(user, "Qwerty123*");
}

app.Run();
