using Amazon.DynamoDBv2;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ophelia.Data;
using Ophelia.Services;
using Ophelia.Services.Affirmations;
using Ophelia.Services.Aws;
using Ophelia.Services.Flows;
using Ophelia.Services.OpenAI;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});


// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var clientConfig = new AmazonDynamoDBConfig
    {
        RegionEndpoint = Amazon.RegionEndpoint.USWest2
    };
    return new AmazonDynamoDBClient(clientConfig);
});
builder.Services.AddSingleton<IS3Service, S3Service>();
builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>();
builder.Services.AddSingleton<IParameterStoreService, ParameterStoreService>();
builder.Services.AddScoped<IAffirmationService, AffirmationService>();
builder.Services.AddScoped<IAffirmationRepository, AffirmationRepository>();
builder.Services.AddScoped<IFlowService, FlowService>();
builder.Services.AddScoped<IFlowRepository, FlowRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    options.Authority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_kc8ZlrSKD";
    options.Audience = "48ei0241femhuar40na80bfsf6";
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.WebHost.UseUrls("http://0.0.0.0:5013");

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
