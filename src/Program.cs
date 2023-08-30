using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using RinhaBackend2023Q3;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = new ConcurrentDictionary<Guid, PessoaGetModel>();

app.MapPost("/pessoas", (PessoaPostModel request) =>
{
    if (db.Any(x => x.Value.Apelido == request.Apelido))
        return Results.UnprocessableEntity("Apelido duplicado");
    var model = new PessoaGetModel(
        Id: Guid.NewGuid(),
        Apelido: request.Apelido,
        Nome: request.Nome,
        Nascimento: request.Nascimento,
        Stack: request.Stack);
    if (!db.TryAdd(model.Id, model))
        return Results.UnprocessableEntity("Erro ao incluir no DB");
    return Results.Created($"/pessoas/{model.Id}", model);
});

app.MapGet("/pessoas/{id}", (Guid id) =>
{
    if (db.TryGetValue(id, out var result))
        return Results.Ok(result);
    return Results.NotFound();
});

app.MapGet("/pessoas", (string? t) =>
{
    if (t == null)
        return Results.Ok(db.Select(x => x.Value).ToArray());
    var result = db
        .Where(x => x.Value.Apelido.Contains(t) || x.Value.Nome.Contains(t) || x.Value.Stack.Contains(t))
        .Select(x => x.Value)
        .ToArray();
    return Results.Ok(result);
});

app.MapGet("/contagem-pessoas", () => Results.Ok(db.Count));

app.Run();
