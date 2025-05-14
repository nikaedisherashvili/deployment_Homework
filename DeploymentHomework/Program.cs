using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

/* ─── Force HTTP only ───────────────────────────────────────────── */
builder.WebHost.ConfigureKestrel(opt =>
{
    // Listen on whatever port Visual Studio gives (set in launchSettings.json),
    // but make it HTTP-only by NOT adding an HTTPS endpoint.
    //
    // When you later publish to AWS Elastic Beanstalk or IIS, they’ll handle
    // port 80 automatically, so you don’t have to hard-code :80 here.
    //
    // No need for opt.ListenAnyIP(80) while developing (would require admin).
});
/* ───────────────────────────────────────────────────────────────── */

var app = builder.Build();

/* In-memory store for the latest string */
string latest = "no data yet";

/* 1️⃣  POST /api/create-answer  { "data": "some-text" } */
app.MapPost("/api/create-answer", (AnswerDto dto) =>
{
    latest = dto.Data;
    return Results.Ok(new { ok = true });
});

/* 2️⃣  GET /answer  – tiny HTML page with the span */
app.MapGet("/answer", () => Results.Content($$"""
    <!doctype html>
    <html>
      <head><meta charset="utf-8"><title>Answer</title></head>
      <body style="font-family:sans-serif;padding:32px">
        <h1>Most recent data:</h1>
        <span id="answer">{{latest}}</span>
      </body>
    </html>
""", "text/html"));

/* Optional: make / redirect to /answer so VS opens something useful */
app.MapGet("/", () => Results.Redirect("/answer"));

app.Run();

/* DTO for the POST */
record AnswerDto(string Data);
