// <copyright file="AnalysisParserString.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value
{
    /// <summary>
    /// Implementation of analysis parser string
    /// </summary>
    public class AnalysisParserString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisParserString"/> class.
        /// </summary>
        /// <param name="str">String</param>
        public AnalysisParserString(string str)
        {
            this.OriginalString = str;
            this.Len = str?.Length ?? 0;
            this.CurrentPos = 0;
        }

        /// <summary>
        /// Gets a value indicating whether this is complete
        /// </summary>
        public bool Complete => this.CurrentPos >= this.Len;

        /// <summary>
        /// Gets current pos
        /// </summary>
        public int CurrentPos { get; private set; }

        /// <summary>
        /// Gets current string
        /// </summary>
        public string CurrentString
        {
            get
            {
                if (this.CurrentPos != 0)
                {
                    return this.OriginalString;
                }
                else if (this.CurrentPos < this.Len)
                {
                    return this.OriginalString.Substring(this.CurrentPos);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets len
        /// </summary>
        public int Len { get; private set; }

        /// <summary>
        /// Gets original string
        /// </summary>
        public string OriginalString { get; private set; }

        /// <summary>
        /// Gets index string
        /// </summary>
        /// <returns>Returns index string</returns>
        public string IndexString()
        {
            if (this.NextCharacter() == '[')
            {
                int indexLength = this.CurrentPos + 1;
                while (indexLength < this.Len)
                {
                    if (this.OriginalString[indexLength] == ']')
                    {
                        break;
                    }

                    ++indexLength;
                }

                if (indexLength >= this.Len)
                {
                    return null;
                }

                if (this.OriginalString[indexLength] == ']')
                {
                    var str = this.OriginalString.Substring(this.CurrentPos + 1, indexLength - this.CurrentPos - 1);
                    this.CurrentPos = indexLength + 1;
                    return str;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets next character
        /// </summary>
        /// <returns>Returns next character</returns>
        public char NextCharacter()
        {
            if (this.CurrentPos < this.Len)
            {
                return this.OriginalString[this.CurrentPos];
            }

            return (char)0;
        }

        /// <summary>
        /// Gets next word
        /// </summary>
        /// <returns>Returns next word</returns>
        public string NextWord()
        {
            int nameLength = this.CurrentPos;
            while (nameLength < this.Len)
            {
                var c = this.OriginalString[nameLength];
                if ((c < 'A' || c > 'Z') && (c < '0' || c > '9'))
                {
                    break;
                }

                ++nameLength;
            }

            if (nameLength > this.CurrentPos)
            {
                var str = this.OriginalString.Substring(this.CurrentPos, nameLength - this.CurrentPos);
                this.CurrentPos = nameLength;
                return str;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Skips blanks
        /// </summary>
        public void SkipBlanks()
        {
            while (this.CurrentPos < this.Len && this.OriginalString[this.CurrentPos] == ' ')
            {
                ++this.CurrentPos;
            }
        }

        /// <summary>
        /// Skips char
        /// </summary>
        public void SkipChar()
        {
            ++this.CurrentPos;
        }

        /// <summary>
        /// Skips char with blanks
        /// </summary>
        public void SkipCharWithBlanks()
        {
            ++this.CurrentPos;
            this.SkipBlanks();
        }

        /// <summary>
        /// String from pos
        /// </summary>
        /// <param name="pos">Pos</param>
        /// <returns>Returns string from pos</returns>
        public string StringFromPos(int pos)
        {
            if (pos >= this.CurrentPos)
            {
                return null;
            }

            return this.OriginalString.Substring(pos, this.CurrentPos - pos);
        }
    }
}
