var builder = WebApplication.CreateBuilder(args);

// Ajouter les services nécessaires à la session
builder.Services.AddDistributedMemoryCache(); // Cache en mémoire pour stocker les sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Durée de la session
    options.Cookie.HttpOnly = true; // Sécurise le cookie de session
    options.Cookie.IsEssential = true; // Nécessaire pour les cookies essentiels
});

// Ajouter les services MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // La valeur par défaut de HSTS est de 30 jours.
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



// Configurer une route par défaut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
