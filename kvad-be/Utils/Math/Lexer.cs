//Service gets system information
using System.Collections.Generic;

struct Lookup
{
    HashSet<string> Delimiters = [
        ",",
    "(",
    ")",
    "[",
    "]",
    "{",
    "}",
    "\"",
    "\'",
    ";",
    "+",
    "-",
    "*",
    ".*",
    "/",
    "./",
    "%",
    "^",
    ".^",
    "~",
    "!",
    "&",
    "|",
    "^|",
    "=",
    ":",
    "?",
    "==",
    "!=",
    "<",
    ">",
    "<=",
    ">=",
    "<<",
    ">>",
    ">>>"
    ];

    HashSet<string> NamedDelimiters = [
        "mod",
    "to",
    "in",
    "and",
    "xor",
    "or",
    "not"
    ];

    HashSet<string> Constants = [
        "true",
    "false",
    "null",
    "undefined"
    ];

    HashSet<string> NumericConstants = [
        "NaN'",
    "Infinity"
    ];

    Dictionary<string, string> EscapeChars = new()
{
    { "n", "\n" },
    { "r", "\r" },
    { "t", "\t" },
    { "b", "\b" },
    { "f", "\f" },
    { "v", "\v" },
    { "0", "\0" },
    { "a", "\a" },
    { "e", "\u001B" }, // Escape character
    { "\\", "\\" },
    { "\"", "\"" },
    { "\'", "\'" },
    { "/", "/" }
};

    public Lookup()
    {
    }
}
enum TokKind { Null, Delimiter, Number, Symbol, Unknown }



readonly record struct Tok(TokKind Kind, string Text);

struct State(string expression)
{
    public string Expression = expression;
    public int Index = 0;
    public string Token = "";
    public TokKind TokenType = TokKind.Null;
    public int NestingLevel = 0;
    public int? ConditionalLevel = null;
    public Dictionary<string, object> ExtraNodes = [];
}

sealed class Lexer(string expression)
{
    State state = new(expression);

    void Next()
    {
        this.state.Index++;
    }

    void Prev()
    {
        this.state.Index--;
    }

}