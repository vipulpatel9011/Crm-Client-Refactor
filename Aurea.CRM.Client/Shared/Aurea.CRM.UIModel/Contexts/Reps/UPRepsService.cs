// <copyright file="UPRepsService.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel.Contexts.Reps
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Reps Service class
    /// </summary>
    public static class UPRepsService
    {
        /// <summary>
        /// Creates RepContainer for given RepType.
        /// </summary>
        /// <param name="repType"><see cref="UPCRMRepType"/> The Rep Type</param>
        /// <returns>Returns <see cref="UPMRepContainer"/> Rep Container</returns>
        public static UPMRepContainer CreateRepContainerForRepType(UPCRMRepType repType)
        {
            var repContainer = new UPMRepContainer();
            var allReps = UPCRMDataStore.DefaultStore.Reps.AllRepsOfTypes(repType);
            var explicitKeyOrder = UPCRMDataStore.DefaultStore.Reps.AllRepIdsOfTypes(repType);
            var rootGroups = UPCRMDataStore.DefaultStore.Reps.AllRootGroupsWithType(repType);
            //if (repType == UPCRMRepType.All)
            //{
            //    repContainer.MultiSelectMode = true;
            //}

            foreach (var repOrgGroup in rootGroups)
            {
                var possibleValue = new UPMRepPossibleValue();
                possibleValue.RepId = repOrgGroup.RepId;
                var valueField = new UPMStringField(new StringIdentifier("x"));
                valueField.StringValue = repOrgGroup.RepName;
                possibleValue.TitleLabelField = valueField;
                repContainer.AddPossibleRepGroupValue(possibleValue, repOrgGroup.RepId);
                UPMRepEditFieldFilter repEditFieldFilter = repContainer.AddRootRepGroup(possibleValue);
                CreateSubFilterForAllReps(repEditFieldFilter, allReps);
            }

            foreach (var rep in allReps)
            {
                var possibleValue = new UPMRepPossibleValue();
                var valueField = new UPMStringField(new StringIdentifier("x"));
                possibleValue.RepId = rep.RepId;
                possibleValue.RepOrgGroupId = rep.RepOrgGroupId;
                valueField.StringValue = rep.RepName;
                possibleValue.TitleLabelField = valueField;
                possibleValue.RepOrgGroupId = rep.RepOrgGroupId;
                repContainer.AddPossibleValue(possibleValue, rep.RepId);
            }

            repContainer.NullValueKey = "0";
            repContainer.ExplicitKeyOrder = explicitKeyOrder;

            return repContainer;
        }

        /// <summary>
        /// Creates Subfilter by given FieldFilter and Rep List
        /// </summary>
        /// <param name="repEditFieldFilter"><see cref="UPMRepEditFieldFilter"/>RepEditField Filter</param>
        /// <param name="allReps">List of reps</param>
        public static void CreateSubFilterForAllReps(UPMRepEditFieldFilter repEditFieldFilter, List<UPCRMRep> allReps)
        {
            foreach (UPCRMRep rep in allReps)
            {
                if (rep.RepType == UPCRMRepType.Group && rep.RepId.Equals(repEditFieldFilter.RepOrgGroup.RepId))
                {
                    foreach (UPCRMRep repChild in rep.RepChildren)
                    {
                        if (repChild.RepType == UPCRMRepType.Group)
                        {
                            UPMRepPossibleValue possibleValue = new UPMRepPossibleValue();
                            possibleValue.RepId = repChild.RepId;
                            UPMStringField valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"));
                            valueField.StringValue = repChild.RepName;
                            possibleValue.TitleLabelField = valueField;
                            UPMRepEditFieldFilter subRepEditFieldFilter = repEditFieldFilter.AddSubFilter(possibleValue);
                            CreateSubFilterForAllReps(subRepEditFieldFilter, allReps);
                        }
                    }
                }
            }
        }
    }
}
