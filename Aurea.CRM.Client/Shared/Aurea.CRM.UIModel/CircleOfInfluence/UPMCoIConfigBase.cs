// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoIConfigBase.cs" company="Aurea Software Gmbh">
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
//   The UPMCoIConfigBase
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPMCoIConfigBase
    /// </summary>
    public class UPMCoIConfigBase
    {
        /// <summary>
        /// Applies the json dictionary.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        public void ApplyJsonDictionary(Dictionary<string, object> dict)
        {
#if PORTING
            foreach (string key in dict.Keys)
            {
                //SEL sel = NSSelectorFromString(key);
                if (key.StartsWith("field") || key.StartsWith("selectedfield"))
                {
                    this.SetValueForKey(dict[key], key);
                }

                const char type = this.TypeOfPropertyNamed(key);
                if (type)
                {
                    string typeInfo = NSString.StringWithCStringEncoding(type, NSUTF8StringEncoding);
                    object value;
                    if (typeInfo.HasPrefix("T@\"UPColor\""))
                    {
                        // UPColor conversion
                        AureaColor upcolor = AureaColor.ColorWithString((string)dict[key]);
                        value = upcolor;
                    }
                    else if (typeInfo.HasPrefix("T{CGSize=ff}"))
                    {
                        ArrayList array = (ArrayList)dict[key];
                        NSNumber width = array[0];
                        NSNumber height = array[1];
                        value = NSValue.ValueWithCGSize(CGSizeMake(width.FloatValue(), height.FloatValue()));
                    }
                    else if (typeInfo.HasPrefix("T@\"UIFont\""))
                    {
                        ArrayList array = (ArrayList)dict.ObjectForKey(key);
                        string familyName = array[0];
                        NSNumber size = array[1];
                        value = UIFont.FontWithNameSize(familyName, size.FloatValue());
                    }
                    else if (typeInfo.HasPrefix("T@\"UPMCoIEdgeViewConfig\"") || typeInfo.HasPrefix("T@\"UPMCoINodeViewConfig\""))
                    {
                        UPMCoIConfigBase base = this.ValueForKey(key);
                        value = dict.ObjectForKey(key);
                        if (value.IsKindOfClass(NSDictionary.TheClass))
                        {
                            base.ApplyJsonDictionary(value);
                        }

                        continue;
                    }
                    else
                    {
                        value = dict[key];
                    }

                    this.SetValueForKey(value, key);
                }
            }
#endif
        }

        private void SetValueForUndefinedKey(object value, string key)
        {
            // Do Nothing
        }

#if PORTING
        char TypeOfPropertyNamed(string name)
        {
            // Caching
            objc_property_t property = class_getProperty(typeof(this), name.UTF8String());
            if (property == NULL) return (NULL);

            return property_getAttributes(property);
        }
#endif
    }
}
