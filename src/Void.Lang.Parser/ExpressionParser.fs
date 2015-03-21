namespace Void.Lang.Parser

type BinaryKind = TODO
type VariableValue = TODO
type Value = TODO
type RegisterName = TODO
type VariableName = TODO

// TODO sent Jared an email about this... I took it (with some mods) from VsVim.
// I need to figure out what the legalities of that are so rather than trying to
// puzzle through all the license documents myself, I'm asking him.
type Expression =
    | Binary of BinaryKind * Expression * Expression
    | ConstantValue of Value 
    | OptionName of string
    | RegisterName of RegisterName
    | Variable of VariableName
    | FunctionCall of VariableName * Expression list