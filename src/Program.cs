using System.Collections.Concurrent;
using RinhaBackend2023Q3;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = new ConcurrentDictionary<Guid, PessoaGetModel>();

app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context
        => await Results.UnprocessableEntity("Erro inesperado.").ExecuteAsync(context)));

app.MapPost("/pessoas", (PessoaPostModel request) =>
{
    if (db.Any(x => x.Value.Apelido == request.Apelido))
        return Results.UnprocessableEntity("Apelido duplicado.");
    if (request.Apelido == null || request.Nome == null)
        return Results.UnprocessableEntity("Atributo obrigatório não informado.");
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
    if (string.IsNullOrWhiteSpace(t))
        return Results.BadRequest("Termo é obrigatório");

    var lower = t.ToLower();
    var result = db
        .Where(x => x.Value.Apelido.ToLower().Contains(lower)
            || x.Value.Nome.ToLower().Contains(lower)
            || (x.Value.Stack != null && x.Value.Stack.Any(y => y.ToLower().Contains(lower))))
        .Select(x => x.Value)
        .ToArray();
    return Results.Ok(result);
});

app.MapGet("/contagem-pessoas", () => Results.Ok(db.Count));

app.Run();
