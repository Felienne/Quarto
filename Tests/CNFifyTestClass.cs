using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;


namespace CNFifyTests
{
    [TestClass]
    public class CNFifyTestClass
    {
        //
        public CNFify.Term a = CNFify.Term.NewVar("a");
        public CNFify.Term b = CNFify.Term.NewVar("b");
        public CNFify.Term c = CNFify.Term.NewVar("c");

        public CNFify.Term p = CNFify.Term.NewVar("p");

        public CNFify.Term q = CNFify.Term.NewVar("q");
        public CNFify.Term r = CNFify.Term.NewVar("r");

        [TestMethod]
        public void SimpleDistribution()
        {
            // p ∨ (q ∧ r) ----> (q ∨ p) ∧ (r ∨ p)

            var T = CNFify.Term.NewOr(p,CNFify.Term.NewAnd(q, r));

            var result = CNFify.normalize(T);

            var expected = CNFify.Term.NewAnd(CNFify.Term.NewOr(p, q), CNFify.Term.NewOr(p, r));

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void InnerDistribution()
        {
            // a ∧ (p ∨ (q ∧ r)) ----> a ∧ (q ∨ p) ∧ (r ∨ p)

            var T = CNFify.Term.NewAnd(a,CNFify.Term.NewOr(p, CNFify.Term.NewAnd(q, r)));

            var result = CNFify.normalize(T);

            var expected = CNFify.Term.NewAnd(a,CNFify.Term.NewAnd(CNFify.Term.NewOr(p, q), CNFify.Term.NewOr(p, r)));

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SimpleDeMorgan()
        {
            // ! (q ∧ r) ----> !q ∨ !r

            var T = CNFify.Term.NewNot(CNFify.Term.NewAnd(q, r));

            var result = CNFify.normalize(T);

            var expected = CNFify.Term.NewOr(CNFify.Term.NewNot(q), CNFify.Term.NewNot(r));

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void SimpleEquality()
        {
            // (a <=> b) ----> (a ∨ !b) ∧ (b ∨ !a)

            var T = CNFify.equal(a, b);

            var result = CNFify.normalize(T);

            var expected = CNFify.Term.NewAnd(
                    CNFify.Term.NewOr(CNFify.Term.NewNot(a), b),
                    CNFify.Term.NewOr(CNFify.Term.NewNot(b), a)
                );

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SimpleRemoveNotBImp()
        {
            // !(a <=> b) ----> (a ∨ b) ∧ (!b ∨ !a)

            var T = CNFify.Term.NewNot(CNFify.equal(a, b));

            var result = CNFify.normalize(T);

            var expected = CNFify.Term.NewAnd(
                CNFify.Term.NewOr(b, a),
                CNFify.Term.NewOr(CNFify.Term.NewNot(a), CNFify.Term.NewNot(b))

                );

            Assert.AreEqual(expected, result);
        }




        //[TestMethod]
        //public void MakeAndList()
        //{
        //    // (a ∧ (b ∧ c)  ----> AndList (a; b; c) 

        //    var T = CNFify.Term.NewAnd(a, CNFify.Term.NewAnd(b, c));

        //    //var result = CNFify.createAndClauseFromList(T);

        //    List<CNFify.Term> abcList = new List<CNFify.Term>() { a, b, c };

        //    FSharpList<CNFify.Term> expected = ListModule.OfSeq(abcList);

        //    Assert.AreEqual(expected, result);
        //}

        //[TestMethod]
        //public void MakeAndListWithOr()
        //{
        //    // (a ∧ (b ∨ c))  ----> AndList (a; b ∨ c) 

        //    var T = CNFify.Term.NewAnd(a, CNFify.Term.NewOr(b, c));

        //    //var result = CNFify.createAndClauseFromList(T);

        //    List<CNFify.Term> abcList = new List<CNFify.Term>() { a, CNFify.Term.NewOr(b, c) };

        //    FSharpList<CNFify.Term> expected = ListModule.OfSeq(abcList);

        //    Assert.AreEqual(expected, result);
        //}



        //[TestMethod]
        //public void parseOutputSAT()
        //{
        //    string output = "SAT -1 2 -3 4 5 6 -7 8 9 -10 11 -12 13 14 15 16 17 18 19 -20 21 -22 23 24 -25 26 27 28 29 -30 -31 32 -33 -34 -35 36 -37 38 39 -40 -41 -42 43 -44 45 46 -47 -48 -49 -50 51 52 -53 54 -55 -56 57 -58 -59 -60 -61 -62 -63 -64";

        //    //var result = CNFify.isSatisfiable(output);

        //    //Assert.AreEqual(true, result);
        //}


        //[TestMethod]
        //public void parseOutputUNSAT()
        //{
        //    string output = "UNSAT";

        //    //var result = CNFify.isSatisfiable(output);

        //    //Assert.AreEqual(false, result);
        //}

        //[TestMethod]
        //public void getAssignment()
        //{
        //    CNFify.Term A = CNFify.Term.NewVar("A");
        //    CNFify.Term B = CNFify.Term.NewVar("B");
        //    CNFify.Term T = CNFify.Term.NewAnd(A,B);

        //    string output = "SAT 1 2";

        //    //var result = CNFify.getAssignment(T,output);

        //    Assert.AreEqual(2, result.Length);
        //}

        //[TestMethod]
        //public void getAssignment()
        //{            
        //    CNFify.Term A = CNFify.Term.NewVar("A");
        //    CNFify.Term B = CNFify.Term.NewVar("B");
        //    CNFify.Term T = CNFify.Term.NewAnd(A, B);

        //    string output = CNFify.makeDimacs(T);

        //    var result = CNFify.runMinisat(output);

        //    Assert.AreEqual(CNFify.minisatResult.MiniUNSAT, result);
        //}

    }
}
