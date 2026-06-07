var builder = WebApplication.CreateBuilder(args);

// 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "VoiceIntegrationAPI",
        Version = "v1",
        Description = "Ses ve santral sistemlerine (Asterisk, TTS/STT) entegrasyon için tasarlanmış merkezi mesaj simülatörü."
    });
});

var app = builder.Build();

// Test ekranını aktif hale getiriyoruz
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "VoiceIntegrationAPI v1");
});

// istek buraya gelecek: POST /api/chat
app.MapPost("/api/chat", (ChatRequest istek) =>
{
    // Boş mesaj gönderilirse hata dönme kontrolü
    if (string.IsNullOrWhiteSpace(istek.Message))
    {
        return Results.BadRequest(new { error = "Mesaj alanı boş olamaz." });
    }

    // 
    var cevap = new ChatResponse
    {
        Response = "Merhaba, size nasıl yardımcı olabilirim?"
    };

    return Results.Ok(cevap);
});

app.Run();

// istek ve cevap 
public record ChatRequest(string Message);
public record ChatResponse { public string Response { get; set; } = string.Empty; }
