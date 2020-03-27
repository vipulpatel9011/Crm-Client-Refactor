// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmReps.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   The Crm Reps class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Extensions;
    using Query;
    using Session;

    /// <summary>
    /// Rep type
    /// </summary>
    [Flags]
    public enum UPCRMRepType
    {
        /// <summary>
        /// The rep
        /// </summary>
        Rep = 1,

        /// <summary>
        /// The group
        /// </summary>
        Group = 2,

        /// <summary>
        /// The resource
        /// </summary>
        Resource = 4,

        /// <summary>
        /// All
        /// </summary>
        All = 7
    }

    /// <summary>
    /// CRM Reps definition
    /// </summary>
    public class UPCRMReps
    {
        private Dictionary<string, UPCRMRep> repDictionary;
        private List<UPCRMRep> repArray;
        private Dictionary<string, UPCRMRep> missingRepDictionary;
        private Dictionary<string, string> emptyRepDictionary;
        private Dictionary<string, Dictionary<string, string>> repNameDictionaryForTypeMasks;

        /// <summary>
        /// Reps the type from string rep type identifier.
        /// </summary>
        /// <param name="repType">Type of the rep.</param>
        /// <returns></returns>
        public UPCRMRepType RepTypeFromStringRepTypeId(string repType)      // TODO: Verify this
        {
            if (repType == "1")
            {
                return UPCRMRepType.Group;
            }

            return repType == "2" ? UPCRMRepType.Resource : UPCRMRepType.Rep;
        }

        /// <summary>
        /// Reads the rep with identifier.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <returns></returns>
        public UPCRMRep ReadRepWithId(string repId)
        {
            if (string.IsNullOrEmpty(repId))
            {
                return null;
            }

            if (this.missingRepDictionary == null)
            {
                this.missingRepDictionary = new Dictionary<string, UPCRMRep>();
            }
            else
            {
                var returnRep = this.missingRepDictionary.ValueOrDefault(repId);
                if (returnRep != null)
                {
                    return returnRep;
                }

                if (this.emptyRepDictionary.ValueOrDefault(repId) != null)
                {
                    return null;
                }
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            var repSearchAndList = configStore.SearchAndListByName("IDSystem");
            FieldControl fieldControl = null;
            if (repSearchAndList != null)
            {
                fieldControl = configStore.FieldControlByNameFromGroup("List", "IDSystem");
            }

            if (fieldControl?.Fields == null ||
                fieldControl.Fields.Count == 0)
            {
                return null;
            }

            UPCRMField vField = fieldControl.Fields[0].Field;
            var fromCondition = new UPInfoAreaConditionLeaf(fieldControl.InfoAreaId, vField.FieldId, "=", repId);
            var metaInfo = new UPContainerMetaInfo(fieldControl);
            metaInfo.RootInfoAreaMetaInfo.AddCondition(fromCondition);
            UPCRMResult result = metaInfo.Find();
            int count = result.RowCount;
            if (count > 0)
            {
                lock (this.missingRepDictionary)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var row = result.ResultRowAtIndex(i);
                        var rep = new UPCRMRep(
                            row.RawValueAtIndex(0),
                            row.RawValueAtIndex(2),
                            row.RawValueAtIndex(1),
                            row.RootRecordId,
                            this.RepTypeFromStringRepTypeId(row.RawValueAtIndex(3)));

                        this.missingRepDictionary[rep.RepId] = rep;
                    }

                    foreach (var rep in this.repArray)
                    {
                        if (string.IsNullOrEmpty(rep.RepOrgGroupId))
                        {
                            continue;
                        }

                        var parentRep = this.missingRepDictionary.ValueOrDefault(rep.RepOrgGroupId);
                        parentRep?.AddChildRep(rep);
                    }
                }

                return this.missingRepDictionary.ValueOrDefault(repId);
            }

            if (this.emptyRepDictionary == null)
            {
                this.emptyRepDictionary = new Dictionary<string, string>();
            }

            this.emptyRepDictionary[repId] = repId;
            return null;
        }

        /// <summary>
        /// Loads the reps.
        /// </summary>
        public void LoadReps()
        {
            lock (this)
            {
                if (this.repDictionary != null)
                {
                    return;
                }

                var configStore = ConfigurationUnitStore.DefaultStore;
                var repSearchAndList = configStore.SearchAndListByName("IDSystem");
                FieldControl fieldControl;
                UPConfigFilter filter = null;
                if (repSearchAndList != null)
                {
                    fieldControl = configStore.FieldControlByNameFromGroup("List", "IDSystem");
                    if (!string.IsNullOrEmpty(repSearchAndList.FilterName))
                    {
                        filter = configStore.FilterByName(repSearchAndList.FilterName);
                    }
                }
                else
                {
                    fieldControl = configStore.FieldControlByNameFromGroup("List", "IDSystem");
                }

                if (fieldControl == null)
                {
                    return;
                }

                var metaInfo = new UPContainerMetaInfo(fieldControl);
                if (filter != null)
                {
                    metaInfo.ApplyFilter(filter);
                }

                var result = metaInfo.Find();
                var count = result != null ? result.RowCount : 0;
                if (count <= 0)
                {
                    return;
                }

                var _repDictionary = new Dictionary<string, UPCRMRep>(count);
                var _repArray = new List<UPCRMRep>(count);
                for (var i = 0; i < count; i++)
                {
                    var row = result.ResultRowAtIndex(i);
                    var rep = new UPCRMRep(
                        row.RawValueAtIndex(0),
                        row.RawValueAtIndex(2),
                        row.RawValueAtIndex(1),
                        row.RootRecordId,
                        this.RepTypeFromStringRepTypeId(row.RawValueAtIndex(3)));
                    _repDictionary[rep.RepId] = rep;
                    _repArray.Add(rep);
                }

                foreach (var rep in _repArray)
                {
                    if (string.IsNullOrEmpty(rep.RepOrgGroupId))
                    {
                        continue;
                    }

                    var parentRep = _repDictionary.ValueOrDefault(rep.RepOrgGroupId);
                    parentRep?.AddChildRep(rep);
                }

                this.repDictionary = _repDictionary;
                this.repArray = _repArray;
            }
        }

        /// <summary>
        /// Gets the Reps dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UPCRMRep> RepDictionary()
        {
            if (this.repDictionary == null)
            {
                this.LoadReps();
            }

            return this.repDictionary;
        }

        /// <summary>
        /// Gets the rep array.
        /// </summary>
        /// <value>
        /// The rep array.
        /// </value>
        public List<UPCRMRep> RepArray
        {
            get
            {
                if (this.repArray == null)
                {
                    this.LoadReps();
                }

                return this.repArray;
            }
        }

        /// <summary>
        /// Gets all the reps types.
        /// </summary>
        /// <param name="typeMask">The type mask.</param>
        /// <returns></returns>
        public List<UPCRMRep> AllRepsOfTypes(UPCRMRepType typeMask)
        {
            if (typeMask == UPCRMRepType.All)
            {
                return this.RepArray;
            }

            var list = new List<UPCRMRep>();

            foreach (UPCRMRepType repType in Enum.GetValues(typeof(UPCRMRepType)).Cast<UPCRMRepType>())
            {
                if (typeMask.HasFlag(repType))
                {
                    list.AddRange(this.RepArray.Where(rep => rep.RepType == repType));
                }
            }

            return list;
        }

        /// <summary>
        /// Alls the type of the root groups with.
        /// </summary>
        /// <param name="repType">Type of the rep.</param>
        /// <returns></returns>
        public List<UPCRMRep> AllRootGroupsWithType(UPCRMRepType repType)
        {
            var repGroupArray = new List<UPCRMRep>();
            foreach (var repGroup in this.RepArray)
            {
                if (repGroup.RepType != UPCRMRepType.Group ||
                    (!string.IsNullOrEmpty(repGroup.RepOrgGroupId) && this.repDictionary.ValueOrDefault(repGroup.RepOrgGroupId) != null))
                {
                    continue;
                }

                if (this.RepGroupHasChildrenOfTypeRecursive(repGroup, repType, true))
                {
                    repGroupArray.Add(repGroup);
                }
            }

            return repGroupArray;
        }

        /// <summary>
        /// Gets all the reps without group.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRep> AllRepsWithoutGroup()
        {
            return this.RepArray.Where(rep => rep.RepType == UPCRMRepType.Rep
                                              && (string.IsNullOrEmpty(rep.RepOrgGroupId) || this.repDictionary.ValueOrDefault(rep.RepOrgGroupId) == null)).ToList();
        }

        /// <summary>
        /// Internals the rep group has children of type recursive.
        /// </summary>
        /// <param name="rep">The rep.</param>
        /// <param name="repType">Type of the rep.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public bool InternalRepGroupHasChildrenOfTypeRecursive(UPCRMRep rep, UPCRMRepType repType, bool recursive)
        {
            return rep.RepChildren?.Any(childrep => (repType.HasFlag(childrep.RepType) || repType == UPCRMRepType.All)
                            || (recursive && this.InternalRepGroupHasChildrenOfTypeRecursive(childrep, repType, true))) ?? false;
        }

        /// <summary>
        /// Childrens the of internal group with type recursive.
        /// </summary>
        /// <param name="rep">The rep.</param>
        /// <param name="repType">Type of the rep.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public List<UPCRMRep> ChildrenOfInternalGroupWithTypeRecursive(UPCRMRep rep, UPCRMRepType repType, bool recursive)
        {
            var repChildren = new List<UPCRMRep>();
            foreach (var childrep in rep.RepChildren)
            {
                if (childrep.RepType == repType)
                {
                    repChildren.Add(childrep);
                }

                if (recursive)
                {
                    var childRepArray = this.ChildrenOfInternalGroupWithTypeRecursive(childrep, repType, true);
                    if (childRepArray.Count > 0)
                    {
                        repChildren.AddRange(childRepArray);
                    }
                }
            }

            return repChildren.Count > 0 ? repChildren : null;
        }

        /// <summary>
        /// Reps the group with identifier has children of type recursive.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <param name="repType">Type of the rep.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public bool RepGroupWithIdHasChildrenOfTypeRecursive(string repId, UPCRMRepType repType, bool recursive)
        {
            var rep = this.repDictionary.ValueOrDefault(repId);
            return rep != null && this.InternalRepGroupHasChildrenOfTypeRecursive(rep, repType, recursive);
        }

        /// <summary>
        /// Childrens the of group with identifier with type recursive.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <param name="repType">Type of the rep.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public List<UPCRMRep> ChildrenOfGroupWithIdWithTypeRecursive(string repId, UPCRMRepType repType, bool recursive)
        {
            var rep = this.repDictionary.ValueOrDefault(repId);
            return rep != null ? this.ChildrenOfInternalGroupWithTypeRecursive(rep, repType, recursive) : null;
        }

        /// <summary>
        /// Reps the group has children of type recursive.
        /// </summary>
        /// <param name="rep">The rep.</param>
        /// <param name="repType">Type of the rep.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public bool RepGroupHasChildrenOfTypeRecursive(UPCRMRep rep, UPCRMRepType repType, bool recursive)
        {
            return this.RepGroupWithIdHasChildrenOfTypeRecursive(rep.RepId, repType, recursive);
        }

        /// <summary>
        /// Childrens the of group with type recursive.
        /// </summary>
        /// <param name="rep">The rep.</param>
        /// <param name="repType">Type of the rep.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public List<UPCRMRep> ChildrenOfGroupWithTypeRecursive(UPCRMRep rep, UPCRMRepType repType, bool recursive)
        {
            return this.ChildrenOfGroupWithIdWithTypeRecursive(rep.RepId, repType, recursive);
        }

        /// <summary>
        /// Alls the rep names of types.
        /// </summary>
        /// <param name="typeMask">The type mask.</param>
        /// <returns></returns>
        public List<string> AllRepNamesOfTypes(UPCRMRepType typeMask)
        {
            return this.RepArray.Where(r => r.RepType == typeMask).Select(r => r.RepName).ToList();
        }

        /// <summary>
        /// Alls the rep ids of types.
        /// </summary>
        /// <param name="typeMask">The type mask.</param>
        /// <returns></returns>
        public List<string> AllRepIdsOfTypes(UPCRMRepType typeMask)
        {
            if (typeMask == UPCRMRepType.All)
            {
                return this.RepArray.Select(r => r.RepId).ToList();
            }
            else
            {
                return this.RepArray.Where(r => r.RepType == typeMask).Select(r => r.RepId).ToList();
            }
        }

        /// <summary>
        /// Formatteds the rep identifier.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <returns></returns>
        public static string FormattedRepId(string repId)
        {
            if (string.IsNullOrEmpty(repId))
            {
                return repId;
            }

            if (repId.Length == 9 && repId.StartsWith("U"))
            {
                return $"10{repId.Substring(1)}";
            }

            switch (repId.Length)
            {
                case 1:
                    return $"00000000{repId}";
                case 2:
                    return $"0000000{repId}";
                case 3:
                    return $"000000{repId}";
                case 4:
                    return $"00000{repId}";
                case 5:
                    return $"0000{repId}";
                case 6:
                    return $"000{repId}";
                case 7:
                    return $"00{repId}";
                case 8:
                    return $"0{repId}";
                default:
                    return repId;
            }
        }

        /// <summary>
        /// Reps the with identifier.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <returns></returns>
        public UPCRMRep RepWithId(string repId)
        {
            repId = FormattedRepId(repId);
            var reps = this.RepDictionary();
            return reps?.ValueOrDefault(repId);
        }

        /// <summary>
        /// Names the of rep identifier string.
        /// </summary>
        /// <param name="repIdString">The rep identifier string.</param>
        /// <returns></returns>
        public string NameOfRepIdString(string repIdString)
        {
            return this.NameOfRepId(repIdString.RepId());
        }

        /// <summary>
        /// Names the of rep identifier.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <returns></returns>
        public string NameOfRepId(int repId)
        {
            var tempRep = this.RepWithId(StringExtensions.NineDigitStringFromRep(repId)) ??
                          this.ReadRepWithId(StringExtensions.NineDigitStringFromRep(repId));

            return tempRep?.RepName;
        }

        /// <summary>
        /// Records the identification of identifier.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <returns></returns>
        public string RecordIdentificationOfId(string repId)
        {
            var rep = this.RepWithId(repId);
            return rep?.RecordIdentification;
        }

        /// <summary>
        /// Gets the current rep identifier.
        /// </summary>
        /// <value>
        /// The current rep identifier.
        /// </value>
        public string CurrentRepId => ServerSession.CurrentSession.AttributeWithKey("repId");

        /// <summary>
        /// Gets the current rep.
        /// </summary>
        /// <value>
        /// The current rep.
        /// </value>
        public UPCRMRep CurrentRep => this.RepWithId(this.CurrentRepId);

        /// <summary>
        /// Gets the current rep record identification.
        /// </summary>
        /// <value>
        /// The current rep record identification.
        /// </value>
        public string CurrentRepRecordIdentification => this.RecordIdentificationOfId(this.CurrentRepId);

        /// <summary>
        /// Gets the Rep Name dictionary with type string.
        /// </summary>
        /// <param name="repTypeString">The rep type string.</param>
        /// <returns></returns>
        public Dictionary<string, string> RepNameDictionaryWithTypeString(string repTypeString)
        {
            var existingDictionary = this.repNameDictionaryForTypeMasks.ValueOrDefault(repTypeString);
            if (existingDictionary != null)
            {
                return existingDictionary;
            }

            var repType = RepTypeFromString(repTypeString);
            var reps = this.AllRepsOfTypes(repType);
            var _repDictionary = new Dictionary<string, string>(reps.Count);
            foreach (var rep in reps)
            {
                _repDictionary[rep.RepId] = rep.RepName;
            }

            if (this.repNameDictionaryForTypeMasks == null)
            {
                this.repNameDictionaryForTypeMasks = new Dictionary<string, Dictionary<string, string>>
                {
                    { repTypeString, _repDictionary }
                };
            }
            else
            {
                this.repNameDictionaryForTypeMasks[repTypeString] = _repDictionary;
            }

            return _repDictionary;
        }

        /// <summary>
        /// Gets the Rep type from string.
        /// </summary>
        /// <param name="strRepType">Type of the string rep.</param>
        /// <returns></returns>
        public static UPCRMRepType RepTypeFromString(string strRepType)
        {
            UPCRMRepType repType = 0;
            if (strRepType == "Rep" || strRepType.IndexOf("Rep.Rep", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                repType = UPCRMRepType.Rep;
            }

            if (strRepType.IndexOf("Rep.Group", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                repType |= UPCRMRepType.Group;
            }

            if (strRepType.IndexOf("Rep.Resource", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                repType |= UPCRMRepType.Resource;
            }

            return repType;
        }
    }
}
