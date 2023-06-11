namespace MarkForth.Html;

internal sealed class Attribute
{
    public Attribute(string name, string value)
    {
        Name = name;
        Value = value;
    }

    internal string Name { get; }

    internal string Value { get; }

}
