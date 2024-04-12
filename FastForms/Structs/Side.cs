namespace FastForms.Structs;

[Flags]
public enum Side
{
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    All = Up | Down | Left | Right,
}