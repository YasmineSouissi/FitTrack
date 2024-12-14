var builder = WebApplication.CreateBuilder(args);

// Ajouter les services n�cessaires � la session
builder.Services.AddDistributedMemoryCache(); // Cache en m�moire pour stocker les sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Dur�e de la session
    options.Cookie.HttpOnly = true; // S�curise le cookie de session
    options.Cookie.IsEssential = true; // N�cessaire pour les cookies essentiels
});

// Ajouter les services MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // La valeur par d�faut de HSTS est de 30 jours.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ajouter l'utilisation de la session
app.UseSession();

app.UseAuthorization();

// Configurer la route pour `Add`
app.MapControllerRoute(
    name: "add",
    pattern: "Articles/Add",
    defaults: new { controller = "Articles", action = "Add" }
);



// Configurer une route par d�faut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
