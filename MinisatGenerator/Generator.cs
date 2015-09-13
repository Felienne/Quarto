using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Collections;

namespace SatGenerator
{
    class Generator
    {
        // The board, with 4 vars per sqaure
        public static CNFify.Term[, ,] board;
        public static List<CNFify.Term> AllPropositions = new List<CNFify.Term>();

        static void initBoard()
        {
            board = new CNFify.Term[4, 4, 4];

            for (int i = 0; i < 4; i++)
			{
			    for (int j = 0; j < 4; j++)
			    {
			        for (int k = 0; k < 4; k++)
			        {
                        board[i, j, k] = CNFify.Term.NewVar(i.ToString() + j.ToString() + k.ToString());
			        } 
			    } 
			}
        }

        static void Main(string[] args)
        {
            initBoard();

            //loop over the number of properties
            //every property should not be equal for all row, column and diagonal

            for (int k = 0; k < 4; k++)
            {
                GenerateRowRules(k);
                GenerateColumnRules(k);
                GenerateDiagonalRules(k);
            }

            //add the rules that not all stones should be the same
            NotAllTheSame();

            //combine all rules
            FSharpList<CNFify.Term> FSharpAllPropositions = ListModule.OfSeq(AllPropositions);
            CNFify.Term all = CNFify.createAndFromList(FSharpAllPropositions);

            var printedCNF = CNFify.makeDimacs(all);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("..\\..\\..\\Minisat\\input"))
            {
                file.WriteLine("p cnf 64 " + printedCNF);
            }
        }

        private static void NotAllTheSame()
        {
            //for every field, there should not be another field with all 4 properties the same.

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int i2 = 0; i2 < 4; i2++)
                    {
                        for (int j2 = 0; j2 < 4; j2++)
                        {
                            if (i != i2 || j != j2)
                            {
                                //so we create one and for all properties for these two fields

                                List<CNFify.Term> AllProperties = new List<CNFify.Term>();

                                for (int k = 0; k < 4; k++)
                                {
                                    var thisSquare = board[i, j, k];
                                    var otherSquare = board[i2, j2, k];

                                    var Equal = CNFify.equal(thisSquare, otherSquare);

                                    AllProperties.Add(Equal);
                                }

                                AllPropositions.Add(NotAllEqual(AllProperties));
                            }

                        }
                        
                    }
                }
            }

        }

        private static void GenerateDiagonalRules(int k)
        {

            List<CNFify.Term> Diagonal1Equalities = new List<CNFify.Term>();
            List<CNFify.Term> Diagonal2Equalities = new List<CNFify.Term>();

            //step through the four diagonal cells
            for (int i = 0; i < 3; i++)
            {
                //one diagonal

                var thisSquare = board[i, i, k];
                var neighbourSquare = board[i + 1, i+1, k];

                //these two should not be equal,
                //the not is added at the upper level, here
                //we just spec the biimplication
                var Equal = CNFify.equal(thisSquare, neighbourSquare);

                Diagonal1Equalities.Add(Equal);

                //the other diagonal

                var thisSquare2 = board[i, 3-i, k];
                var neighbourSquare2 = board[i + 1, 3-(i+1), k];

                //these two should not be equal,
                //the not is added at the upper level, here
                //we just spec the equality
                var Equal2 = CNFify.equal(thisSquare, neighbourSquare);

                Diagonal2Equalities.Add(Equal2);
            }

            AllPropositions.Add(NotAllEqual(Diagonal1Equalities));
            AllPropositions.Add(NotAllEqual(Diagonal2Equalities));
                   
        }

        private static CNFify.Term NotAllEqual(List<CNFify.Term> input)
        {
            //we now have a list of all squares that should not be equal, make a
            //not - and combi of them:

            // voor in de slides: gekut met types kan wel wat beter Microsoft
            //eerste poging ipv ListModule.OfSeq was: (Microsoft.FSharp.Collections.FSharpList<CNFify.Term>)AllAnds;

            CNFify.Term NotAllSame =
                CNFify.Term.NewNot(
                    CNFify.createAndFromList(ListModule.OfSeq(input))
                );

            return NotAllSame;
        }

        private static void GenerateRowRules(int k)
        {
            //loop over all rows
            for (int i = 0; i < 4; i++)
            {
                List<CNFify.Term> AllEqualities = new List<CNFify.Term>();

                //loop over the number of columns
                for (int j = 0; j < 3; j++)
                {
                    var thisSquare = board[i, j, k];
                    var neighbourSquare = board[i, j + 1, k];

                    //these two should not be equal,
                    //the not is added at the upper level, here
                    //we just spec the equality
                    var Equal = CNFify.equal(thisSquare, neighbourSquare);
                    AllEqualities.Add(Equal);
                }

                AllPropositions.Add(NotAllEqual(AllEqualities)); 
            }
        }

        private static void GenerateColumnRules(int k)
        {
            //loop over all columns
            for (int j = 0; j < 4; j++)
            {

                List<CNFify.Term> AllEqualities = new List<CNFify.Term>();

                //loop over the number of rows
                for (int i = 0; i < 3; i++)
                {
                    var thisSquare = board[i, j, k];
                    var neighbourSquare = board[i+1, j, k];

                    //these two should not be equal,
                    //the not is added at the upper level, here
                    //we just spec the equality 
                    var Equal = CNFify.equal(thisSquare, neighbourSquare);

                    AllEqualities.Add(Equal);

                }

                AllPropositions.Add(NotAllEqual(AllEqualities));
                
            }
        }
    }
}
