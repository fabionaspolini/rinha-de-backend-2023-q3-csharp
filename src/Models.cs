namespace RinhaBackend2023Q3;

public abstract record class PessoaBaseModel(
    string Apelido,
    string Nome,
    DateOnly Nascimento,
    string[] Stack);

public record class PessoaPostModel(
    string Apelido,
    string Nome,
    DateOnly Nascimento,
    string[] Stack) : PessoaBaseModel(Apelido, Nome, Nascimento, Stack);


public record class PessoaGetModel(
    Guid Id,
    string Apelido,
    string Nome,
    DateOnly Nascimento,
    string[] Stack) : PessoaBaseModel(Apelido, Nome, Nascimento, Stack);
