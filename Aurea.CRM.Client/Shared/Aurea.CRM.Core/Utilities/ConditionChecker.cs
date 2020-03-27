// <copyright file="ConditionChecker.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Utilities
{
    /// <summary>
    /// Implementation of condition checker class.
    /// </summary>
    public class ConditionChecker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionChecker"/> class.
        /// </summary>
        /// <param name="conditionOperator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="valueTo">Value to</param>
        public ConditionChecker(ConditionCheckOperator conditionOperator, string value, string valueTo)
        {
            this.ConditionOperator = conditionOperator;
            this.Value = value;
            this.ValueTo = valueTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionChecker"/> class.
        /// </summary>
        /// <param name="conditionOperatorString">Condition operator string</param>
        /// <param name="value">Value</param>
        /// <param name="valueTo">Value to</param>
        public ConditionChecker(string conditionOperatorString, string value, string valueTo)
            : this(ConditionOperatorFromString(conditionOperatorString), value, valueTo)
        {
        }

        /// <summary>
        /// Gets condition operator
        /// </summary>
        public ConditionCheckOperator ConditionOperator { get; private set; }

        /// <summary>
        /// Gets or sets value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets value to
        /// </summary>
        public string ValueTo { get; private set; }

        /// <summary>
        /// Generates condition operator from string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Condition check operator</returns>
        public static ConditionCheckOperator ConditionOperatorFromString(string str)
        {
            if (str == "=")
            {
                return ConditionCheckOperator.Equal;
            }
            else if (str == ">=")
            {
                return ConditionCheckOperator.GreaterEqual;
            }
            else if (str == "<=")
            {
                return ConditionCheckOperator.LessEqual;
            }
            else if (str == "<>")
            {
                return ConditionCheckOperator.NotEqual;
            }
            else if (str == ">")
            {
                return ConditionCheckOperator.Greater;
            }
            else if (str == "<")
            {
                return ConditionCheckOperator.Less;
            }
            else if (str == "><")
            {
                return ConditionCheckOperator.Between;
            }

            return ConditionCheckOperator.Equal;
        }

        /// <summary>
        /// Checks if matches string
        /// </summary>
        /// <param name="candidate">Text to check</param>
        /// <returns>Returns true if matches</returns>
        public bool MatchesString(string candidate)
        {
            var result = this.Value.CompareTo(candidate);
            switch (this.ConditionOperator)
            {
                case ConditionCheckOperator.Like:
                    return result == 0;
                case ConditionCheckOperator.Equal:
                    return result == 0;
                case ConditionCheckOperator.NotEqual:
                    return !(result == 0);
                case ConditionCheckOperator.Greater:
                    return result > 0;
                case ConditionCheckOperator.GreaterEqual:
                    return result >= 0;
                case ConditionCheckOperator.Less:
                    return result < 0;
                case ConditionCheckOperator.LessEqual:
                    return result <= 0;
                case ConditionCheckOperator.Between:
                    return result >= 0 && this.ValueTo.CompareTo(candidate) <= 0;
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.ValueTo?.Length > 0)
            {
                return $"{this.StringConditionOperator()}[{this.Value},{this.ValueTo}]";
            }
            else
            {
                return this.StringConditionOperator();
            }
        }

        private string StringConditionOperator()
        {
            switch (this.ConditionOperator)
            {
                case ConditionCheckOperator.Between:
                    return "><";
                case ConditionCheckOperator.Equal:
                    return "=";
                case ConditionCheckOperator.NotEqual:
                    return "<>";
                case ConditionCheckOperator.Greater:
                    return ">";
                case ConditionCheckOperator.GreaterEqual:
                    return ">=";
                case ConditionCheckOperator.Less:
                    return "<";
                case ConditionCheckOperator.LessEqual:
                    return "<=";
                case ConditionCheckOperator.Like:
                    return "LIKE";
            }

            return "??";
        }
    }
}
