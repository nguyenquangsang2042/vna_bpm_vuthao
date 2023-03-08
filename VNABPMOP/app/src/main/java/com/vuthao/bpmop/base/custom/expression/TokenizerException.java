package com.vuthao.bpmop.base.custom.expression;

public class TokenizerException extends Expression.ExpressionException {
    public TokenizerException(String message, int characterPosition) {
        super(message, characterPosition);
    }
}
