﻿//******************************************************************************************************
//  BooleanExpression.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/12/2016 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Gemstone.Collections.CollectionExtensions;

namespace Gemstone.IO.Parsing;

/// <summary>
/// Represents a boolean expression that can be parsed and executed at runtime.
/// </summary>
/// <remarks>
/// Binary operators have the same level of precedence and are evaluated from right to left.
/// </remarks>
public class BooleanExpression
{
    #region [ Members ]

    // Nested Types

    /// <summary>
    /// Represents a variable that can be tweaked at runtime.
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// The identifier used to refer to the variable.
        /// </summary>
        public readonly string Identifier;

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public bool Value;

        /// <summary>
        /// Creates a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="identifier">The identifier used to refer to the variable.</param>
        public Variable(string identifier)
        {
            Identifier = identifier;
        }
    }

    // Fields
    private readonly Dictionary<string, Variable> m_variables;
    private readonly Func<bool> m_evaluate;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="BooleanExpression"/> class.
    /// </summary>
    /// <param name="expressionText">The expression text to be parsed as a boolean expression.</param>
    /// <remarks>
    /// The default comparer for identifiers is <see cref="StringComparer.OrdinalIgnoreCase"/>.
    /// </remarks>
    public BooleanExpression(string expressionText) : this(expressionText, StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BooleanExpression"/> class.
    /// </summary>
    /// <param name="expressionText">The expression text to be parsed as a boolean expression.</param>
    /// <param name="identifierComparer">Comparer used to compare identifiers.</param>
    public BooleanExpression(string expressionText, IEqualityComparer<string> identifierComparer)
    {
        m_variables = new Dictionary<string, Variable>(identifierComparer);
        StringBuilder builder = new(expressionText);
        Expression expression = ParseExpression(builder);

        if (builder.Length > 0)
            throw new FormatException($"Unexpected character '{builder[0]}' in expression. Expected end of expression.");

        m_evaluate = Expression.Lambda<Func<bool>>(expression).Compile();
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets the list of variables found while parsing the boolean expression.
    /// </summary>
    public List<Variable> Variables
    {
        get
        {
            return m_variables.Values.ToList();
        }
    }

    /// <summary>
    /// Gets the variable identified by the given identifier.
    /// </summary>
    /// <param name="identifier">The identifier used to refer to the variable.</param>
    /// <returns>The variable identified by the given identifier.</returns>
    public Variable this[string identifier]
    {
        get
        {
            return m_variables[identifier];
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Evaluates the expression using the
    /// current values of the variables.
    /// </summary>
    /// <returns>The result of the evaluation.</returns>
    public bool Evaluate()
    {
        return m_evaluate();
    }

    /// <summary>
    /// Attempts to get the variable identified by the given identifier.
    /// </summary>
    /// <param name="identifier">The identifier used to refer to the variable.</param>
    /// <param name="variable">The variable identified by the given identifier.</param>
    /// <returns>True if the variable is present in the expression; false otherwise.</returns>
    public bool TryGetVariable(string identifier, out Variable? variable)
    {
        return m_variables.TryGetValue(identifier, out variable);
    }

    private Expression ParseExpression(StringBuilder builder)
    {
        Expression subexpression = ParseSubexpression(builder);
        ShedWhitespace(builder);

        if (builder.Length <= 0)
            return subexpression;

        char binaryOp = builder[0];

        if (binaryOp == ')')
            return subexpression;

        builder.Remove(0, 1);
        ShedWhitespace(builder);

        return binaryOp switch
        {
            '&' => Expression.And(subexpression, ParseExpression(builder)),
            '|' => Expression.Or(subexpression, ParseExpression(builder)),
            '^' => Expression.ExclusiveOr(subexpression, ParseExpression(builder)),
            _ => throw new FormatException($"Unexpected character '{binaryOp}' in expression. Expected: '&', '|', or '^'."),
        };
    }

    private Expression ParseSubexpression(StringBuilder builder)
    {
        ShedWhitespace(builder);

        if (builder.Length == 0)
            throw new FormatException("Unexpected end of expression. Expected: '(', '!', '~', or identifier.");

        switch (builder[0])
        {
            case '(':
                builder.Remove(0, 1);
                Expression expression = ParseExpression(builder);

                if (builder.Length == 0)
                    throw new FormatException("Unexpected end of expression. Expected: ')'.");

                if (builder[0] != ')')
                    throw new FormatException($"Unexpected character '{builder[0]}' in expression. Expected: ')'.");

                builder.Remove(0, 1);

                return expression;

            case '!':
            case '~':
                builder.Remove(0, 1);
                return Expression.Not(ParseSubexpression(builder));
        }

        return ParseIdentifier(builder);
    }

    private Expression ParseIdentifier(StringBuilder builder)
    {
        StringBuilder nameBuilder = new();

        while (builder.Length > 0 && char.IsLetterOrDigit(builder[0]))
        {
            nameBuilder.Append(builder[0]);
            builder.Remove(0, 1);
        }

        if (nameBuilder.Length == 0)
            throw new FormatException($"Unexpected character '{builder[0]}' in expression. Expected identifier.");

        string name = nameBuilder.ToString();
        Variable identifier = m_variables.GetOrAdd(name, key => new Variable(key));

        return ((Expression<Func<bool>>)(() => identifier.Value)).Body;
    }

    private void ShedWhitespace(StringBuilder builder)
    {
        while (builder.Length > 0 && char.IsWhiteSpace(builder[0]))
            builder.Remove(0, 1);
    }

    #endregion
}
