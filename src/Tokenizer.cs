using System;
using System.Collections.Generic;
using System.Linq;

public static class Tokenizer
{
    private static readonly OperatorToken ADD_TOKEN = new OperatorToken("+", 1, (op1, op2) => op1 + op2);
    private static readonly OperatorToken SUBTRACT_TOKEN = new OperatorToken("-", 1, (op1, op2) => op1 - op2);
    private static readonly OperatorToken MULTIPLY_TOKEN = new OperatorToken("*", 2, (op1, op2) => op1 * op2);
    private static readonly OperatorToken DIVIDE_TOKEN = new OperatorToken("/", 2, (op1, op2) => op1 / op2);

    public static IEnumerable<Token> Tokenize(string expression)
    {
        return expression.Split(' ').Select(ToToken);
    }

    private static Token ToToken(string tokenValue)
    {
        switch(tokenValue)
        {
            case "+":
                return ADD_TOKEN;
            case "-":
                return SUBTRACT_TOKEN;
            case "*":
                return MULTIPLY_TOKEN;
            case "/":
                return DIVIDE_TOKEN;
            default:
                return new NumberToken(decimal.Parse(tokenValue));
        }
    }
}

public interface Token
{
    bool IsNumber();
}

public class OperatorToken : Token
{
    private readonly string representation;
    private readonly int precedence;
    private readonly Func<decimal, decimal, decimal> application;

    public OperatorToken(
        string representation, int precedence, Func<decimal, decimal, decimal> application)
    {
        this.representation = representation;
        this.precedence = precedence;
        this.application = application;
    }

    public bool IsNumber() => false;

    public bool IsHigherOrEqualPrecedenceThan(OperatorToken otherOp)
    {
        return this.precedence >= otherOp.precedence;
    }

    public NumberToken apply(NumberToken operand1, NumberToken operand2) =>
            new NumberToken(application(operand1.Value, operand2.Value));

    public override string ToString() => representation;
}

public class NumberToken : Token
{
    public readonly decimal Value;

    public NumberToken(decimal value)
    {
        this.Value = value;
    }

    public bool IsNumber() => true;

    public override string ToString() => Value.ToString();
}