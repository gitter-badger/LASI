﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm
{
    public class ProperPluralNoun : ProperNoun, IQuantifiable
    {
        public ProperPluralNoun(string text)
            : base(text) {
        }

        /// <summary>
        /// Gets or sets a Qunatifier which specifies the number of units of the ProperNoun which are referred to in this occurance.
        /// E.g. "[18] Pinkos"
        /// </summary>
        public virtual Quantifier Quantifier {
            get;
            set;
        }

        protected override void ProcessEntityTypeInfo(string found) {

            switch (found) {
                case "person":
                    EntityType = EntityKind.Person;
                    break;
                case "location":
                    EntityType = EntityKind.Location;
                    break;
                case "organization":
                    EntityType = EntityKind.Organization;
                    break;
                default:
                    EntityType = EntityKind.Location;
                    break;

            }
        }
    }
}
