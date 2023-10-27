﻿using System.Text.RegularExpressions;
using DbcParserLib.Observers;

namespace DbcParserLib.Parsers
{
    internal class ValueTableDefinitionLineParser : ILineParser
    {
        private const string ValueTableDefinitionLineStarter = "VAL_TABLE_ ";
        private const string ValueTableDefinitionParsingRegex = @"VAL_TABLE_\s+([a-zA-Z_][\w]*)\s+((?:\d+\s+(?:""[^""]*"")\s+)*)\s*;";
        private const string ValueTableNewLineRegex = @"(""[^""]*""\s+)";
        private const string ValueTableNewLineRegexReplace = "$1\n";

        private readonly IParseFailureObserver m_observer;

        public ValueTableDefinitionLineParser(IParseFailureObserver observer)
        {
            m_observer = observer;
        }

        public bool TryParse(string line, IDbcBuilder builder, INextLineProvider nextLineProvider)
        {
            var cleanLine = line.Trim(' ');

            if (cleanLine.StartsWith(ValueTableDefinitionLineStarter) == false)
                return false;

            var match = Regex.Match(cleanLine, ValueTableDefinitionParsingRegex);
            if (match.Success)
            {
                var valueTable = Regex.Replace(match.Groups[2].Value.TrimStart(), ValueTableNewLineRegex, ValueTableNewLineRegexReplace);
                var valueTableDictionary = valueTable.ToDictionary();
                builder.AddNamedValueTable(match.Groups[1].Value, valueTableDictionary, valueTable);
            }
            else
                m_observer.ValueTableDefinitionSyntaxError();

            return true;
        }
    }
}