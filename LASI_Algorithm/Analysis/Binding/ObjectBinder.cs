﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm.Binding
{
    public class ObjectBinder
    {
        /// <summary>
        /// Initializes a new instances of ObjectBinder class.
        /// </summary>
        public ObjectBinder() {

            St0 = new State0(this);
            St1 = new State1(this);
            St2 = new State2(this);
            St4 = new State4(this);
            St5 = new State5(this);
            St6 = new State6(this);

        }
        public void Bind(Sentence sentence) {

            var phrases = sentence.Phrases.ToList();
            var verbPhraseIndex = phrases.FindIndex(r => r is VerbPhrase);
            _bindingTarget = sentence.Phrases.ElementAt(verbPhraseIndex) as VerbPhrase;
            var remainingPhrases = phrases.Skip(verbPhraseIndex + 1).Reverse();
            if (remainingPhrases.Count() > 0) {
                inputstream.PushAll(remainingPhrases);

                St0.ProcessNext(inputstream.PopDynamic());

            }
        }
        private void AssociateDirect() {
            foreach (var e in entities) {
                _bindingTarget.BindDirectObject(e);
            }
            entities.Clear();
            ConjunctNounPhrases.Clear();

        }

        private void AssociateIndirect() {
            foreach (var e in entities) {
                _bindingTarget.BindIndirectObject(e);
            }
            entities.Clear();
            ConjunctNounPhrases.Clear();

        }
        protected Stack<Phrase> inputstream = new Stack<Phrase>();
        protected VerbPhrase _bindingTarget;
        protected IEntity directObject;
        protected IEntity indirectObject;
        protected bool directFound = false;

        protected List<AdjectivePhrase> lastAdjectivals = new List<AdjectivePhrase>();
        protected List<NounPhrase> ConjunctNounPhrases = new List<NounPhrase>();
        protected List<AdjectivePhrase> ConjunctAdjectivePhrases = new List<AdjectivePhrase>();
        protected Stack<NounPhrase> entities = new Stack<NounPhrase>();
        private State0 St0;
        private State1 St1;
        private State2 St2;
        private State4 St4;
        private State5 St5;
        private State6 St6;
        private ConjunctionPhrase lastConjunctive;
        private PrepositionalPhrase lastPrepositional;
        public bool indirectFound;


        private void BindBuiltupAdjectivePhrases(NounPhrase phrase) {
            foreach (var adjp in this.lastAdjectivals) {
                phrase.BindDescriber(adjp);
            }
            this.lastAdjectivals.Clear();
        }


        abstract class State
        {
            protected State(ObjectBinder machine) {
                _machine = machine;
                _stream = machine.inputstream;
            }

            public virtual void ProcessNext(Phrase phrase) {
                throw new InvalidStateTransitionException(StateName, phrase);
            }

            protected void Universal(Phrase Phrase) {
                Machine.LastPhrase = Phrase;
            }

            protected string StateName {
                get {
                    return stateName;
                }
                set {
                    stateName = value;
                }
            }


            public ObjectBinder Machine {
                get {
                    return _machine;
                }
                set {
                    _machine = value;
                }
            }


            protected Stack<Phrase> Stream {
                get {
                    return _stream;
                }
                set {
                    _stream = value;
                }
            }



            private string stateName;
            private Stack<Phrase> _stream;
            private ObjectBinder _machine;


        }
        class State0 : State
        {
            public State0(ObjectBinder machine)
                : base(machine) {
                StateName = "s0";

            }
            public void ProcessNext(PrepositionalPhrase phrase) {
                Machine.lastPrepositional = phrase;
                Machine.St0.ProcessNext(Stream.PopDynamic());
            }
            public virtual void ProcessNext(VerbPhrase phrase) {
                new ObjectBinder().Bind(new Sentence(new[] { phrase }.Concat(Stream.ToList())));
            }

            public virtual void ProcessNext(AdverbPhrase phrase) {
                Machine._bindingTarget.ModifyWith(phrase);
                Universal(phrase);
                if (Stream.Count < 1)
                    return;
                Machine.St0.ProcessNext(Stream.PopDynamic());

            }
            public virtual void ProcessNext(NounPhrase phrase) {
                if (Machine.lastPrepositional != null) {
                    phrase.PrepositionOnLeft = Machine.lastPrepositional;
                    Machine.lastPrepositional.OnRightSide = phrase;
                    Machine._bindingTarget.AttachObjectViaPreposition(phrase.PrepositionOnLeft);
                }
                Machine.entities.Push(phrase);
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St2.ProcessNext(Stream.PopDynamic());
            }
            public virtual void ProcessNext(AdjectivePhrase phrase) {
                Machine.lastAdjectivals.Add(phrase);
                Universal(phrase);
                if (Machine.inputstream.Count > 0) {
                    Machine.St1.ProcessNext(Stream.PopDynamic());
                }
            }
        }


        class State1 : State
        {
            public State1(ObjectBinder machine)
                : base(machine) {
                StateName = "s1";
            }
            public void ProcessNext(NounPhrase phrase) {
                Machine.entities.Push(phrase);
                Machine.BindBuiltupAdjectivePhrases(phrase);
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St2.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(ConjunctionPhrase phrase) {
                Machine.lastConjunctive = phrase;
                Universal(phrase);
            }
        }
        class State2 : State
        {


            public State2(ObjectBinder machine)
                : base(machine) {
                StateName = "s2";

            }
            public void ProcessNext(ConjunctionPhrase phrase) {
                phrase.OnLeft = Machine.entities.Peek();
                Machine.lastConjunctive = phrase;
                Machine.ConjunctNounPhrases.Add(Machine.entities.Peek());
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St4.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(AdjectivePhrase phrase) {
                Machine.AssociateIndirect();
                Universal(phrase);

                Machine.St4.ProcessNext(Stream.PopDynamic());
            }

            public void ProcessNext(PrepositionalPhrase phrase) {
                foreach (var e in Machine.entities)
                    Machine._bindingTarget.BindDirectObject(e);
                Machine.lastPrepositional = phrase;

                Machine.entities.Last().PrepositionOnRight = Machine.lastPrepositional;
                phrase.OnLeftSide = Machine.entities.Last();
                Machine.entities.Clear();
                Machine.directFound = true;
                Machine.ConjunctNounPhrases.Clear();
                Universal(phrase);

                Machine.St0.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(NounPhrase phrase) {
                foreach (var e in Machine.entities)
                    Machine._bindingTarget.BindIndirectObject(e);
                Machine.entities.Clear();
                Machine.indirectFound = true;
                Machine.ConjunctNounPhrases.Clear();

                Machine.entities.Push(phrase);

                if (Stream.Count < 1) {

                    Machine.AssociateDirect();
                    return;
                }
                Universal(phrase);

                Machine.St0.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(SubordinateClauseBeginPhrase phrase) {
                Machine.AssociateDirect();
                Machine.AssociateIndirect();
                Universal(phrase);

            }
            public virtual void ProcessNext(AdverbPhrase phrase) {
                Machine._bindingTarget.ModifyWith(phrase);
                Universal(phrase);
                foreach (var e in Machine.entities) if (!Machine.directFound)
                        Machine._bindingTarget.BindDirectObject(e);
                    else
                        Machine._bindingTarget.BindIndirectObject(e);
                if (Stream.Count < 1)
                    return;
                Machine.St0.ProcessNext(Stream.PopDynamic());

            }

        }
        class State4 : State
        {


            public State4(ObjectBinder machine)
                : base(machine) {

                StateName = "s4";
            }
            public void ProcessNext(NounPhrase phrase) {
                Machine.entities.Push(phrase);
                Machine.ConjunctNounPhrases.Add(phrase);
                Machine.lastConjunctive.OnRight = phrase;
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St2.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(AdjectivePhrase phrase) {
                Machine.lastAdjectivals.Add(phrase);
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St5.ProcessNext(Stream.PopDynamic());
            }
        }
        class State5 : State
        {
            public State5(ObjectBinder machine)
                : base(machine) {
                StateName = "s5";
            }

            public void ProcessNext(NounPhrase phrase) {
                Machine.ConjunctNounPhrases.Add(phrase);
                Machine.entities.Push(phrase);
                Machine.BindBuiltupAdjectivePhrases(phrase);
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St2.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(ConjunctionPhrase phrase) {
                phrase.OnLeft = Machine.lastAdjectivals.Last();
                Machine.lastConjunctive = phrase;
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St6.ProcessNext(Stream.PopDynamic());
            }

        }
        class State6 : State
        {


            public State6(ObjectBinder machine)
                : base(machine) {
                StateName = "s6";
            }
            public void ProcessNext(AdjectivePhrase phrase) {
                Machine.lastAdjectivals.Add(phrase);
                Machine.lastConjunctive.OnRight = phrase;
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St5.ProcessNext(Stream.PopDynamic());
            }
            public void ProcessNext(NounPhrase phrase) {
                Machine.entities.Push(phrase);
                Machine.ConjunctNounPhrases.Add(phrase);
                Machine.lastConjunctive.OnRight = phrase;
                Machine.BindBuiltupAdjectivePhrases(phrase);
                if (Machine.inputstream.Count < 1) {
                    if (!Machine.directFound)
                        Machine.AssociateDirect();
                    else
                        Machine.AssociateIndirect();

                    return;
                }
                Universal(phrase);

                Machine.St2.ProcessNext(Stream.PopDynamic());
            }


        }



        public Phrase LastPhrase {
            get;
            set;
        }
    }

    static class DynamicStackExtensions
    {
        /// <summary>
        /// An extension method which pops the next item from the stack, but returns it as a an object of Type dynamic.
        /// This allows the overloaded methods present in each state to be correctly selected based on the run time type of the phrase.
        /// </summary>
        /// <param name="stack">The Stack<LASI.Algorithm.Phrase> instances from which to pop.</param>
        /// <returns>The Phrase at the top of the stack typed as dynamic.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the Stack is empty.</exception>
        internal static dynamic PopDynamic(this Stack<Phrase> stack) {
            return stack.Pop();
        }
        internal static void PushAll(this Stack<Phrase> stack, IEnumerable<Phrase> items) {
            foreach (var i in items)
                stack.Push(i);
        }
    }

}
