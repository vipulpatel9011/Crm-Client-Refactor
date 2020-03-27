// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSERowValue.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   Row Value
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;

    /// <summary>
    /// Row Value
    /// </summary>
    public class UPSERowValue
    {
        private int integerValue;
        private double doubleValue;
        private object value;

        /// <summary>
        /// Gets or sets the initial value.
        /// </summary>
        /// <value>
        /// The initial value.
        /// </value>
        public object InitialValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether [save unchanged].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [save unchanged]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveUnchanged { get; private set; }

        /// <summary>
        /// Gets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        public object OriginalValue { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.integerValue = int.MinValue;
                this.doubleValue = double.MaxValue;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowValue"/> class.
        /// </summary>
        /// <param name="_value">The value.</param>
        /// <param name="_saveUnchanged">if set to <c>true</c> [save unchanged].</param>
        public UPSERowValue(object _value, bool _saveUnchanged)
        {
            this.value = _value;
            this.OriginalValue = _value;
            this.SaveUnchanged = _saveUnchanged;
            this.doubleValue = double.MaxValue;
            this.integerValue = int.MinValue;
            this.InitialValue = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowValue"/> class.
        /// </summary>
        /// <param name="_value">The value.</param>
        public UPSERowValue(object _value)
            : this(_value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowValue"/> class.
        /// </summary>
        public UPSERowValue()
            : this(null, false)
        {
        }

        /// <summary>
        /// Sets the unchanged.
        /// </summary>
        public void SetUnchanged()
        {
            this.OriginalValue = this.value;
        }

        /// <summary>
        /// Remembers the current as initial value.
        /// </summary>
        public void RememberCurrentAsInitialValue()
        {
            this.InitialValue = this.value;
        }

        /// <summary>
        /// Resets to initial value.
        /// </summary>
        public void ResetToInitialValue()
        {
            this.OriginalValue = this.InitialValue;
            this.value = this.InitialValue;
        }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        /// <value>
        /// The integer value.
        /// </value>
        public int IntegerValue
        {
            get
            {
                if (this.integerValue == int.MinValue)
                {
                    this.integerValue = Convert.ToInt32(this.Value?.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                }

                return this.integerValue;
            }
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        public double DoubleValue
        {
            get
            {
                if (this.doubleValue == double.MaxValue)
                {
                    this.doubleValue = Convert.ToDouble(this.value, System.Globalization.CultureInfo.InvariantCulture);
                }

                return this.doubleValue;
            }
        }
    }
}
