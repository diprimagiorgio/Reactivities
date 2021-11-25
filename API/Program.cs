using Application.Activities;

var builder = WebApplication.CreateBuilder(args);

// add services to the container

builder.Services.AddControllers(opt => {
    // we ensure that every endpoint require authorization
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
})
.AddFluentValidation(config =>{
    config.RegisterValidatorsFromAssemblyContaining<Create>();
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIndentityServices(builder.Configuration);

// configure the HTTP request pipeline

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseXContentTypeOptions();
app.UseReferrerPolicy(opt => opt.NoReferrer());
app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
app.UseXfo(opt => opt.Deny());
// most important, I cas start with report only to check
app.UseCsp(opt => opt
    .BlockAllMixedContent()// no http and https. Just https
    .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com/"))// from where css came from, we are ok just from our domain
    .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com/", "data:"))
    .FormActions(s => s.Self())
    .FrameAncestors(s => s.Self())
    .ScriptSources(s => s.Self().CustomSources("sha256-kXwZFeDqzQYQxMANlJcsdedkJvek1q5ncjzFrCq4x+I="))// for js 
    .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com/"))
    
);
if (app.Environment.IsDevelopment())
{
    // orders metter and first is going to call be called first
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}else{
    app.UseHsts();
}

//app.UseHttpsRedirection();

// here we have to add support to static file
app.UseDefaultFiles();// going to check in wwwroot folder and index file
app.UseStaticFiles();



app.UseCors("CorsPolicy");// here the order is important

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);
//by using the using keyword we are going to dispose the scope when the method is done
using var scope = app.Services.CreateScope();

var services  = scope.ServiceProvider;
try{
    // we have added DataContext as a service.
    // we are using the service locator pattern
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();// this is going to create the databse if we don´t have it
    await Seed.SeedData(context,userManager );
}catch (Exception ex){
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during the migration");
}
await app.RunAsync();
