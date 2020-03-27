// <copyright file="AnalysisFunctionParser.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Analysis.Value.AnalysisFunction
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AnalysisValueFunction;
    using Extensions;

    /// <summary>
    /// Implementation of analysis function parser
    /// </summary>
    public class AnalysisFunctionParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionParser"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="functionString">Funstion string</param>
        public AnalysisFunctionParser(Analysis analysis, string functionString)
        {
            this.Analysis = analysis;
            var parserString = new AnalysisParserString(functionString);
            var result = this.Parse(parserString);

            if (result.Error != null)
            {
                this.Error = result.Error;
            }
            else if (parserString.Complete)
            {
                this.Function = result.Result;
            }
            else
            {
                this.Error = new Exception("extra characters");
            }
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets error
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets function
        /// </summary>
        public AnalysisValueFunction Function { get; private set; }

        /// <summary>
        /// Gets unparsed
        /// </summary>
        public string Unparsed { get; private set; }

        private AnalysisFunctionFormulaParseResult Parse(AnalysisParserString parserString)
        {
            List<object> argumentArray;
            int nextArgumentIndex;
            string javascriptFormula;
            if (parserString.CurrentString?.StartsWith("$") ?? false)
            {
                AnalysisFunctionFormulaParseResult result = this.ParseVariable(parserString);
                if (result.Result == null || parserString.Complete || parserString.NextCharacter() == ')')
                {
                    return result;
                }

                argumentArray = new List<object> { result.Result };
                javascriptFormula = "v[0]";
                nextArgumentIndex = 1;
            }
            else
            {
                javascriptFormula = string.Empty;
                nextArgumentIndex = 0;
                argumentArray = new List<object>();
            }

            int bracketCount = 0;
            string copyString;
            int copyPos = parserString.CurrentPos;
            AnalysisFunctionFormulaParseResult parseResult;
            var builder = new StringBuilder();
            builder.Append(javascriptFormula);
            while (!parserString.Complete)
            {
                switch (parserString.NextCharacter())
                {
                    case '(':
                        bracketCount++;
                        parserString.SkipChar();
                        break;
                    case ')':
                        if (bracketCount == 0)
                        {
                            copyString = parserString.StringFromPos(copyPos);
                            if (copyString?.Length > 0)
                            {
                                builder.Append(copyString);
                            }

                            var function = new AnalysisFunctionFormula(javascriptFormula, argumentArray, this.Analysis);
                            if (function != null)
                            {
                                return new AnalysisFunctionFormulaParseResult(function);
                            }

                            return new AnalysisFunctionFormulaParseResult("function not supported");
                        }
                        else
                        {
                            parserString.SkipChar();
                        }

                        bracketCount--;
                        break;
                    case '$':
                        copyString = parserString.StringFromPos(copyPos);
                        if (copyString?.Length > 0)
                        {
                            builder.Append(copyString);
                        }

                        parseResult = this.ParseVariable(parserString);
                        if (parseResult.Error != null)
                        {
                            return parseResult;
                        }

                        builder.Append($"v[{nextArgumentIndex++}]");
                        argumentArray.Add(parseResult.Result);
                        copyPos = parserString.CurrentPos;
                        break;
                    default:
                        parserString.SkipChar();
                        break;
                }
            }

            javascriptFormula = builder.ToString();

            if (bracketCount > 0)
            {
                return new AnalysisFunctionFormulaParseResult("missing closing bracket");
            }

            copyString = parserString.StringFromPos(copyPos);
            if (copyString?.Length > 0)
            {
                builder.Append(copyString);
            }

            AnalysisFunctionFormula function1 = new AnalysisFunctionFormula(javascriptFormula, argumentArray, this.Analysis);
            if (function1 != null)
            {
                return new AnalysisFunctionFormulaParseResult(function1);
            }

            return new AnalysisFunctionFormulaParseResult("function not supported");
        }

        private AnalysisFunctionFormulaParseResult ParseVariable(AnalysisParserString parserString)
        {
            if (!parserString.CurrentString.StartsWith("$"))
            {
                return new AnalysisFunctionFormulaParseResult("invalid function token");
            }

            parserString.SkipChar();
            bool isTextFunction = false;
            if (parserString.NextCharacter() == '!')
            {
                isTextFunction = true;
                parserString.SkipChar();
            }

            string functionName = parserString.NextWord();
            parserString.SkipBlanks();
            if (!isTextFunction)
            {
                AnalysisFunction func = AnalysisFunction.FunctionWithName(functionName);
                if (func != null)
                {
                    if (parserString.NextCharacter() == '(')
                    {
                        parserString.SkipCharWithBlanks();
                        AnalysisFunctionFormulaParseResult parseResult = this.Parse(parserString);
                        if (parseResult.Result != null)
                        {
                            AnalysisFunctionFunc f = new AnalysisFunctionFunc(func, parseResult.Result, this.Analysis);
                            if (parserString.NextCharacter() == ')')
                            {
                                parserString.SkipCharWithBlanks();
                                return new AnalysisFunctionFormulaParseResult(f);
                            }
                            else
                            {
                                return new AnalysisFunctionFormulaParseResult("invalid function end");
                            }
                        }
                    }
                }
            }

            string indexPart0 = parserString.IndexString();
            string indexPart1 = null;
            if (indexPart0 != null)
            {
                indexPart1 = parserString.IndexString();
            }

            int occurrence = 0;
            if (indexPart1?.Length > 0)
            {
                occurrence = indexPart0.ToInt();
                indexPart0 = indexPart1;
            }

            bool record = false;
            int fieldNumber = -1;
            if (indexPart0 != null)
            {
                if (indexPart0 == "r")
                {
                    record = true;
                }

                fieldNumber = indexPart0.ToInt();
            }
            else
            {
                record = true;
            }

            AnalysisTable table = this.Analysis.TableWithInfoAreaIdOccurrence(functionName, occurrence);
            if (table == null)
            {
                return new AnalysisFunctionFormulaParseResult($"unknown table {functionName}/{occurrence}");
            }

            if (record)
            {
                AnalysisFunctionInfoAreaField f = new AnalysisFunctionInfoAreaField(table.CountField(), this.Analysis);
                return new AnalysisFunctionFormulaParseResult(f);
            }

            AnalysisSourceField sourceField = table.FieldWithIndex(fieldNumber);
            if (sourceField == null)
            {
                return new AnalysisFunctionFormulaParseResult($"unknown field {functionName}/{occurrence}.{fieldNumber}");
            }

            AnalysisFunctionSimpleField functionField;
            if (isTextFunction)
            {
                functionField = new AnalysisFunctionSimpleTextField(sourceField, this.Analysis);
            }
            else
            {
                functionField = new AnalysisFunctionSimpleNumberField(sourceField, this.Analysis);
            }

            return new AnalysisFunctionFormulaParseResult(functionField);
        }
    }
}
