﻿using LASI.Algorithm.LexicalLookup;
using LASI.Algorithm.Aliasing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm.RelationshipLookups
{
    /// <summary>
    /// Provides convenient extension methods for working with IEntity and IVerbal constructs in the context of an applicable IRelationshipLookup.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Returns an object containing all of the IVerbals on which the two IEntity constructs are related or null if they have no established IVerbal relationships.
        /// </summary>
        /// <param name="performer">The first IEntity, the peformer of the action.</param>
        /// <param name="receiver">The second IEntity, the receiver of the action.</param>
        /// <returns>An object containing all of the IVerbals on which the two IEntity constructs are related or null if they have no established IVerbal relationships.</returns>
        public static ActionsRelatedOn? IsRelatedTo(this IEntity performer, IEntity receiver) {
            Func<IEntity, IEntity, bool> predicate = (L, R) => L.IsAliasFor(R) || L.IsSimilarTo(R);

            var lookupTable = entityLookupContexts.ContainsKey(performer) ? entityLookupContexts[performer] : entityLookupContexts.ContainsKey(receiver) ? entityLookupContexts[receiver] : null;
            if (lookupTable != null) {

                var actions = lookupTable[performer, predicate, receiver, predicate];
                if (actions.Any())
                    return new ActionsRelatedOn(actions);
                else
                    return null;
            } else {
                throw new InvalidOperationException(string.Format(@"There is no relationship lookup Context associated with {0} or {1}.\n
                    Please associate a context by calling {2}.SetRelationshipLookup or {3}.SetRelationshipLookup appropriately.",
                    performer, receiver,
                    performer, receiver));
            }

        }
        /// <summary>
        /// Determines if the Given ActionsRelatedOn object contains the provided IVerbal.
        /// </summary>
        /// <param name="relatorSet">The object whose contents are to be searched. This parameter can be null. If it is null, false is returned.</param>
        /// <param name="relator">The IVerbal for which to search.</param>
        /// <returns>True if the given ActionsRelatedOn set contains the provided IVerbal, false if theActionsRelatedOn set does not contain the provided IVerbal or is null.</returns>
        public static bool On(this ActionsRelatedOn? relatorSet, IVerbal relator) {

            return relatorSet.HasValue ? relatorSet.Value.RelatedOn.Contains(relator, (l, r) => l.Text == r.Text) : false;
        }
        /// <summary>
        /// Associates the given IEntity to the given IRelationshipLookup. All future searches involving the provided entity will be done in the context of the provided lookup.
        /// </summary>
        /// <param name="entity">The IEntity to associate to a lookup context.</param>
        /// <param name="relationshipLookup">The IRelationshipLookup instance providing a lookup context for the entity.</param>
        public static void SetRelationshipLookup(this IEntity entity, IRelationshipLookup<IEntity, IVerbal> relationshipLookup) {
            entityLookupContexts.AddOrUpdate(entity, relationshipLookup, (k, v) => relationshipLookup);
        }




        private static ConcurrentDictionary<IEntity, IRelationshipLookup<IEntity, IVerbal>> entityLookupContexts = new ConcurrentDictionary<IEntity, IRelationshipLookup<IEntity, IVerbal>>();
        /// <summary>
        /// Provides a nullable sequence of IVerbal constructs. Instances of the structure will implicitely collapse to true or false as needed
        /// </summary>
        public struct ActionsRelatedOn
        {

            internal ActionsRelatedOn(IEnumerable<IVerbal> actionsRelatedOn)
                : this() {
                RelatedOn = actionsRelatedOn;
            }

            internal IEnumerable<IVerbal> RelatedOn {
                get;
                private set;
            }
            /// <summary>
            /// Returms true only if the set is not null;
            /// </summary>
            /// <param name="set"></param>
            /// <returns></returns>
            public static bool operator true(ActionsRelatedOn? set) {
                return set != null;
            }

            /// <summary>
            /// Returms true onl if the set is null;
            /// </summary>
            /// <param name="set"></param>
            /// <returns></returns>
            public static bool operator false(ActionsRelatedOn? set) {
                return set == null;
            }
        }
    }
}
