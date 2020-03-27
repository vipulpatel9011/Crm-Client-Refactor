// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MPercentEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm percent edit field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;

    // TODO: Finish porting

    /// <summary>
    /// The upm percent edit field.
    /// </summary>
    public class UPMPercentEditField : UPMNumberEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMPercentEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMPercentEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        // static NSNumberFormatter UpNumberFormatterDisplay()
        // {
        // static dispatch_once_t once;
        // static NSNumberFormatter displayFormatter;
        // dispatch_once(once, delegate ()
        // {
        // displayFormatter = NSNumberFormatter.TheNew();
        // displayFormatter.SetNumberStyle(NSNumberFormatterDecimalStyle);
        // displayFormatter.SetUsesGroupingSeparator(true);
        // displayFormatter.SetMaximumFractionDigits(2);
        // });
        // return displayFormatter;
        // }

        // static NSNumberFormatter UpNumberFormatterEdit()
        // {
        // static dispatch_once_t once;
        // static NSNumberFormatter editFormatter;
        // dispatch_once(once, delegate ()
        // {
        // editFormatter = NSNumberFormatter.TheNew();
        // editFormatter.SetNumberStyle(NSNumberFormatterDecimalStyle);
        // editFormatter.SetUsesGroupingSeparator(false);
        // });
        // return editFormatter;
        // }

        // string StringDisplayValue()
        // {
        // string vStringValue = displayFormatter.StringFromNumber((NSNumber)this.FieldValue);
        // if (vStringValue.Length == 0)
        // {
        // return "";
        // }
        // else
        // {
        // NSNumber number = (NSNumber)this.FieldValue;
        // string displayValue = displayFormatter.StringFromNumber(NSNumber.NumberWithFloat(number.FloatValue * 100));
        // return NSString.StringWithFormat("%@ %%", displayValue);
        // }

        // }

        // NSNumber NumberValue()
        // {
        // return (NSNumber)this.FieldValue;
        // }

        // void SetNumberValue(NSNumber _numberValue)
        // {
        // this.FieldValue = _numberValue;
        // }

        // void SetNumberFromStringValue(string _numberAsString)
        // {
        // NSNumber number = editFormatter.NumberFromString(_numberAsString);
        // this.FieldValue = NSNumber.NumberWithFloat(number.FloatValue / 100);
        // }

        // NSNumber NumberFromStringValue(string _numberAsString)
        // {
        // NSNumber number = editFormatter.NumberFromString(_numberAsString);
        // return NSNumber.NumberWithFloat(number.FloatValue / 100);
        // }

        // string StringEditValue()
        // {
        // NSNumber number = (NSNumber)this.FieldValue;
        // return editFormatter.StringFromNumber(NSNumber.NumberWithFloat(number.FloatValue * 100));
        // }
    }
}
