﻿
namespace LASI.Algorithm.FundamentalSyntacticInterfaces
{
    public interface IPrepositional : ILexical
    {
        /// <summary>
        /// Gets or sets the IPropositionLinkable construct on the Right side of the IPrepositional.
        /// </summary>
        IPrepositionLinkable OnRightSide {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the IPropositionLinkable construct on the Left side of the IPrepositional.
        /// </summary>
        IPrepositionLinkable OnLeftSide {
            get;
            set;
        }
        /// <summary>
        /// Gets the object of the IPrepositional construct.
        /// </summary>
        ILexical PrepositionalObject {
            get;
        }
        /// <summary>
        /// Binds an ILexical construct as the object of the IPrepositional. 
        /// Lexical constructs include Word, Phrase, and Clause Types.
        /// </summary>
        /// <param name="prepositionalObject">The ILexical construct as the object of the IPrepositional.</param>
        void BindObjectOfPreposition(ILexical prepositionalObject);
    }
}