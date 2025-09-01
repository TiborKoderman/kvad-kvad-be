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

struct State
{
    public string Expression;
    public int Index;
    public string Token;
    public TokKind TokenType;
    public int NestingLevel;
    public int? ConditionalLevel;
    public Dictionary<string, object> ExtraNodes;

    public State(string expression)
    {
        Expression = expression;
        Index = 0;
        Token = "";
        TokenType = TokKind.Null;
        NestingLevel = 0;
        ConditionalLevel = null;
        ExtraNodes = [];
    }
}

sealed class Lexer
{
    State state;

    public Lexer(string expression)
    {
        state = new(expression);
    }

    void Next()
    {
        this.state.Index++;
    }

    void Prev()
    {
        this.state.Index--;
    }

}