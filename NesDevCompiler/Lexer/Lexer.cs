using NesDevCompiler.CharacterStream;
using System.Diagnostics;

namespace NesDevCompiler.Lexer;

public class Lexer : ILexer
{
	private ICharacterStream _stream;

	public Lexer(ICharacterStream stream)
	{
		_stream = stream;
	}

	public Token Peek(bool ignoreWhitespace = true, int t = 0)
	{
		ICharacterStream stream = (ICharacterStream)_stream.Clone();
		Token token = GetToken(stream);

		if (t == 0 && ignoreWhitespace)
		{
			while (token.Value == " ")
			{
				token = GetToken(stream);
			}
			return token;
		}
		for (int i = 0; i < t; i++)
		{
			if (ignoreWhitespace)
			{
				if (token.Value != " ")
				{
					token = GetToken(stream);
				}
				while (token.Value == " ")
				{
					token = GetToken(stream);
				}
			}
			else
			{
				token = GetToken(stream);
			}
		}

		return token;
	}

	public Token Next(bool ignoreWhitespace = true)
	{
		Token token = GetToken(_stream);

		while (token.Value == " ")
		{
			token = GetToken(_stream);
		}

		return token;
	}

	public bool End()
	{
		return _stream.End();
	}

	public Token GetToken(ICharacterStream stream)
	{
		string token = "";
		string prev = "";
		while (true)
		{
			if (stream.End())
				return new Token(TokenType.End, "");
			char c = stream.Peek();
			prev = token;
			token += c;

			TokenType type = TokenIdentifier.GetTokenType(token);

			if (type == TokenType.Punctuation)
			{
				stream.Read();
				return new Token(type, token);
			}
			if (!string.IsNullOrEmpty(prev))
			{
				if (TokenIdentifier.GetTokenType(c.ToString()) == TokenType.Punctuation)
				{
					TokenType idenType = TokenIdentifier.GetTokenType(prev);
					return new Token(idenType, prev);
				}
			}
			stream.Read();
		}
	}
}