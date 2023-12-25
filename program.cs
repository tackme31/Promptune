using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Promptune.Services;
using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Electron
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

// AntDesign
builder.Services.AddAntDesign();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<SecretKeyService>();
builder.Services.AddSingleton<PromptService>();
builder.Services.AddSingleton<FileExportService>();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Open the Electron-Window here
await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
{
    Width = 1100,
    Height = 800,
    ThickFrame = true,
    Title = "Promptune",
});

app.Run();
