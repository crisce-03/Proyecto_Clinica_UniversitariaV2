var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ===== Inyección de dependencias propias =====
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.ConsultasDatos>();
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.PacienteDatos>();
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.MedicoDatos>();
builder.Services.AddScoped<Proyecto_Clinica_Universitaria.Datos.MedicamentosDatos>();
builder.Services.AddSingleton<Proyecto_Clinica_Universitaria.Servicios.AzureBlobService>();

// ===== HttpContext + Session =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // requerido por Session
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(60);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();



