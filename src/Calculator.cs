using System;
using System.Collections.Generic;

public static class Calculator
{
    public static decimal calculate(IEnumerable<Token> tokens)
    {
        return ReversePolishEvaluate(ShuntingYard(tokens));
    }

    private static IEnumerable<Token> ShuntingYard(IEnumerable<Token> tokens)
    {
        var output = new Queue<Token>();
        var operators = new Stack<OperatorToken>();

        foreach (Token token in tokens)
        {
            if (token.IsNumber())
            {
                output.Enqueue(token);
                continue;
            }

            OperatorToken operatorToken = (OperatorToken)token;
            while (operators.Count > 0 &&
                operators.Peek().IsHigherOrEqualPrecedenceThan(operatorToken))
            {
                output.Enqueue(operators.Pop());
            }

            operators.Push(operatorToken);
        }

        while (operators.Count > 0) {
            output.Enqueue(operators.Pop());
        }

        return output;
    }

    private static decimal ReversePolishEvaluate(IEnumerable<Token> tokens)
    {
        var stack = new Stack<NumberToken>();

        foreach (Token token in tokens)
        {
            if (token.IsNumber()) {
                stack.Push((NumberToken)token);
                continue;
            }

            OperatorToken operatorToken = (OperatorToken)token;
            NumberToken operand2 = stack.Pop();
            NumberToken operand1 = stack.Pop();
            stack.Push(operatorToken.apply(operand1, operand2));
        }

        return stack.Pop().Value;
    }
}