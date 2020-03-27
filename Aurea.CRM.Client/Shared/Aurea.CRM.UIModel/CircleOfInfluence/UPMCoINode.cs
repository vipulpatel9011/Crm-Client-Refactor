// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoINode.cs" company="Aurea Software Gmbh">
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
//   COI Node
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// COI Node
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMContainer" />
    public class UPMCoINode : UPMContainer
    {
        private List<Tuple<UPMCoINode, UPMCoIEdge>> additionalRelations;
        private Dictionary<string, Tuple<UPMCoINode, UPMCoIEdge>> allRelations;
        private List<string> allRelationsIndizes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoINode"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMCoINode(IIdentifier identifier)
            : base(identifier)
        {
            this.additionalRelations = new List<Tuple<UPMCoINode, UPMCoIEdge>>();
            this.allRelations = new Dictionary<string, Tuple<UPMCoINode, UPMCoIEdge>>();
            this.allRelationsIndizes = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [online data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online data]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlineData { get; set; }

        /// <summary>
        /// Gets or sets the initial field.
        /// </summary>
        /// <value>
        /// The initial field.
        /// </value>
        public UPMStringField InitialField { get; set; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <value>
        /// The parent element.
        /// </value>
        public new UPMCoINode Parent { get; set; }

        /// <summary>
        /// Gets or sets the parent relation.
        /// </summary>
        /// <value>
        /// The parent relation.
        /// </value>
        public UPMCoIEdge ParentRelation { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPMStringField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public /*UIImage*/ object Icon { get; set; }    // CRM-5007

        /// <summary>
        /// Gets or sets the information fields.
        /// </summary>
        /// <value>
        /// The information fields.
        /// </value>
        public List<UPMStringField> InfoFields { get; set; }

        /// <summary>
        /// Gets or sets the color of the information area.
        /// </summary>
        /// <value>
        /// The color of the information area.
        /// </value>
        public AureaColor InfoAreaColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [childs loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [childs loaded]; otherwise, <c>false</c>.
        /// </value>
        public bool ChildsLoaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [group node].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [group node]; otherwise, <c>false</c>.
        /// </value>
        public bool GroupNode { get; set; }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        /// <param name="appliedIdentifiers">The applied identifiers.</param>
        public override void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
            base.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            for (int i = 0; i < this.AllRelationsCount; i++)
            {
                this.AllRelationEdgeAtIndex(i).ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            }
        }

        /// <summary>
        /// Adds the child node child relation.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="edge">The edge.</param>
        public void AddChildNodeChildRelation(UPMCoINode node, UPMCoIEdge edge)
        {
            node.ParentRelation = edge;
            this.Children.Add(node);
        }

        /// <summary>
        /// Childs the index of the node at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoINode ChildNodeAtIndex(int index)
        {
            return this.Children[index] as UPMCoINode;
        }

        /// <summary>
        /// Gets the child count.
        /// </summary>
        /// <value>
        /// The child count.
        /// </value>
        public int ChildCount => this.Children.Count;

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification => this.GroupNode ? null : ((RecordIdentifier)this.Identifier).RecordIdentification;

        /// <summary>
        /// Adds all relation to relation.
        /// </summary>
        /// <param name="toNode">To node.</param>
        /// <param name="edge">The edge.</param>
        public void AddAllRelationToRelation(UPMCoINode toNode, UPMCoIEdge edge)
        {
            this.allRelations[edge.Identifier.IdentifierAsString] = new Tuple<UPMCoINode, UPMCoIEdge>(toNode, edge);

            if (!this.allRelationsIndizes.Contains(edge.Identifier.IdentifierAsString))
            {
                this.allRelationsIndizes.Add(edge.Identifier.IdentifierAsString);
            }
        }

        /// <summary>
        /// Gets all relations count.
        /// </summary>
        /// <value>
        /// All relations count.
        /// </value>
        public int AllRelationsCount => this.allRelations.Count;

        /// <summary>
        /// Alls the index of the relation node at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoINode AllRelationNodeAtIndex(int index)
        {
            return this.allRelations[this.allRelationsIndizes[index]].Item1;
        }

        /// <summary>
        /// Alls the index of the relation edge at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoIEdge AllRelationEdgeAtIndex(int index)
        {
            return this.allRelations[this.allRelationsIndizes[index]].Item2;
        }

        /// <summary>
        /// Adds the additional relation to relation.
        /// </summary>
        /// <param name="toNode">To node.</param>
        /// <param name="edge">The edge.</param>
        public void AddAdditionalRelationToRelation(UPMCoINode toNode, UPMCoIEdge edge)
        {
            this.additionalRelations.Add(new Tuple<UPMCoINode, UPMCoIEdge>(toNode, edge));
        }

        /// <summary>
        /// Gets the additional relations count.
        /// </summary>
        /// <value>
        /// The additional relations count.
        /// </value>
        public int AdditionalRelationsCount => this.additionalRelations.Count;

        /// <summary>
        /// Additionals the index of the relation node at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoINode AdditionalRelationNodeAtIndex(int index)
        {
            return this.additionalRelations[index].Item1;
        }

        /// <summary>
        /// Additionals the index of the relation edge at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoIEdge AdditionalRelationEdgeAtIndex(int index)
        {
            return this.additionalRelations[index].Item2;
        }

        /// <summary>
        /// Levels the order.
        /// </summary>
        /// <returns></returns>
        public List<UPMCoINode> LevelOrder()
        {
            List<UPMCoINode> array = new List<UPMCoINode>();
            LevelOrderRekArray(this, array);
            return array;
        }

        private static void LevelOrderRekArray(UPMCoINode node, List<UPMCoINode> array)
        {
            array.Add(node);
            foreach (UPMCoINode childNode in node.Children)
            {
                LevelOrderRekArray(childNode, array);
            }
        }
    }
}
