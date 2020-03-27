// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMResultSection.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The PMResultRowProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// The PMResultRowProvider interface.
    /// </summary>
    public interface UPMResultRowProvider
    {
        /// <summary>
        /// The number of result rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int NumberOfResultRows();

        /// <summary>
        /// The row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        UPMResultRow RowAtIndex(int index);
    }

    /// <summary>
    /// The upm result section.
    /// </summary>
    public class UPMResultSection : UPMContainer
    {
        /// <summary>
        /// The created children.
        /// </summary>
        private List<UPMResultRow> createdChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMResultSection"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMResultSection(IIdentifier identifier)
            : base(identifier)
        {
            // Default Style Initialization
            // this.SectionField.valueFontStyle = UPMFontStylePlain;
            // this.SectionField.ValueFontColor = AureaColor.ColorWithString(@"0.33;0.33;0.33;1.0"); // dark Grey
            this.BarColor = AureaColor.ColorWithString(@"1.0;0.0;0.0;1.0");

            // this.SectionField.valueFontSize = 19.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMResultSection"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="resultRowProvider">
        /// The result row provider.
        /// </param>
        public UPMResultSection(IIdentifier identifier, UPMResultRowProvider resultRowProvider)
            : this(identifier)
        {
            this.ResultRowProvider = resultRowProvider;
            this.Optimized = true;

            this.createdChildren = new List<UPMResultRow>(resultRowProvider.NumberOfResultRows());
        }

        /// <summary>
        /// Gets or sets the bar color.
        /// </summary>
        public AureaColor BarColor { get; set; }

        /// <summary>
        /// Gets or sets the global search icon name.
        /// </summary>
        public string GlobalSearchIconName { get; set; }

        /// <summary>
        /// The number of result rows.
        /// </summary>
        public int NumberOfResultRows => this.ResultRowProvider?.NumberOfResultRows() ?? this.Children.Count;

        /// <summary>
        /// Gets a value indicating whether optimized.
        /// </summary>
        public bool Optimized { get; private set; }

        /// <summary>
        /// Gets the result row provider.
        /// </summary>
        public UPMResultRowProvider ResultRowProvider { get; private set; }

        /// <summary>
        /// Gets or sets the section field.
        /// </summary>
        public UPMField SectionField { get; set; }

        /// <summary>
        /// Gets or sets the section index key.
        /// </summary>
        public string SectionIndexKey { get; set; }

        /// <summary>
        /// The add result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        public void AddResultRow(UPMResultRow resultRow)
        {
            this.AddChild(resultRow);
        }

        /// <summary>
        /// The remove result row.
        /// </summary>
        /// <param name="resultRowIdentifier">
        /// The result row identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveResultRow(IIdentifier resultRowIdentifier)
        {
            if (this.ResultRowProvider != null)
            {
                for (int i = 0; i < this.NumberOfResultRows; i++)
                {
                    this.ResultRowAtIndex(i);
                }

                foreach (UPMResultRow resultRow in this.createdChildren)
                {
                    if (resultRow.Identifier.MatchesIdentifier(resultRowIdentifier))
                    {
                        this.createdChildren.Remove(resultRow);
                        return true;
                    }
                }
            }
            else
            {
                foreach (var resultRow in this.Children)
                {
                    if (((UPMResultRow)resultRow).Identifier.MatchesIdentifier(resultRowIdentifier))
                    {
                        this.Children.Remove(resultRow);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The result row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        public UPMResultRow ResultRowAtIndex(int index)
        {
            if (this.ResultRowProvider != null)
            {
                if (this.createdChildren.Count > index)
                {
                    var resultRowObject = this.createdChildren[index];
                    if (resultRowObject != null)
                    {
                        return resultRowObject;
                    }
                }

                UPMResultRow resultRow = this.ResultRowProvider.RowAtIndex(index);

                if (this.createdChildren.Count == index)
                {
                    this.createdChildren.Add(resultRow);
                }
                else if (this.createdChildren.Count > index)
                {
                    this.createdChildren[index] = resultRow;
                }
                else
                {
                    for (int i = this.createdChildren.Count; i < index; i++)
                    {
                        this.createdChildren.Add(null);
                    }

                    this.createdChildren.Add(resultRow);
                }

                return resultRow;
            }

            return this.Children[index] as UPMResultRow;
        }
    }
}
