using Excersize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excersize.Tokens;
using System.Threading;
using ParseTreeProject.ProductionClasses;

namespace ParseTreeProject
{
    public class Parser
    {
        public ProductionE E = new ProductionE("E");

        private ProductionGroup CurrentProducitonGroup;
        public ProductionEPrime EPrime = new ProductionEPrime("EPrime");
            
           
        public Parser(TokenCollection tokens)
        {
            CurrentProducitonGroup = E;
        }
        public IEnumerable<Token> Filter(TokenCollection tokens)
        {
            return tokens.Where(x => x.GetType() != typeof(WhitespaceToken) && x.GetType() != typeof(NewLineToken) && x.GetType() != typeof(CommentToken));

        }
        public bool TryParse(TokenCollection tokens)
        {
            tokens = new TokenCollection(Filter(tokens));
            return TryMatchProduction(tokens, out IProductionNode Node);
        }
        public bool TryMatchProduction(TokenCollection tokens, out IProductionNode node)
        {
            
            switch(CurrentProducitonGroup.ID)
            {
                case "E":
                    ECheck(tokens, out node);
                    break;
                case "EPrime":
                    EPrimeCheck(tokens, out node);
                    break;
            }
            node = default;
            return false;
        }
        //Dont want this
        
        public bool ECheck(TokenCollection tokens, out IProductionNode node)
        {
            TokenCollection LeftTokens;
            TokenCollection RightTokens;
            if (E.TryExactOperatorCheck(tokens, E.productions[0], out node, out LeftTokens, out RightTokens))
            {
                TryMatchProduction(LeftTokens, out IProductionNode LeftKid);
                node.Children.Add(LeftKid);
                TryMatchProduction(RightTokens, out IProductionNode RightKid);
                node.Children.Add(RightKid);
                return true;
            }
            else if (E.TryExactOperatorCheck(tokens, E.productions[1], out node, out LeftTokens, out RightTokens))
            {
                TryMatchProduction(LeftTokens, out IProductionNode LeftKid);
                node.Children.Add(LeftKid);
                TryMatchProduction(RightTokens, out IProductionNode RightKid);
                node.Children.Add(RightKid);
                return true;
            }
            else
            {
                CurrentProducitonGroup = EPrime;
                return TryMatchProduction(tokens, out node);

               
            }
        }

        public bool EPrimeCheck(TokenCollection tokens, out IProductionNode node)
        {

            TokenCollection LeftTokens;
            TokenCollection RightTokens;
            if (EPrime.TryExactOperatorCheck(tokens, E.productions[0], out node, out LeftTokens, out RightTokens))
            {
                CurrentProducitonGroup = E;
                TryMatchProduction(LeftTokens, out IProductionNode LeftKid);
                node.Children.Add(LeftKid);
                TryMatchProduction(RightTokens, out IProductionNode RightKid);
                node.Children.Add(RightKid);
                return true;
            }
            else if (EPrime.TryExactOperatorCheck(tokens, E.productions[1], out node, out LeftTokens, out RightTokens))
            {

                CurrentProducitonGroup = E;
                TryMatchProduction(LeftTokens, out IProductionNode LeftKid);
                node.Children.Add(LeftKid);
                TryMatchProduction(RightTokens, out IProductionNode RightKid);
                node.Children.Add(RightKid);
                return true;
            }
            else if(EPrime.TryParenthesisCheck(tokens, out TokenCollection ETokens))
            {
                CurrentProducitonGroup = E;
                node = default;
                return TryMatchProduction(ETokens, out IProductionNode a);
                
            }
            node = default;
            return false;
        }


    }
}
