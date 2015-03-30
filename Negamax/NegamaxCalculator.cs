using System;
using System.Collections.Generic;

namespace Negamax
{
    public class NegamaxCalculator<TState, TPlayer, TDatum>
    {
        //using ScoredNode = Tuple<int, TScore, TDatum>;
        private readonly Func<TState, TPlayer, int> scoreCalculator;
        private readonly Predicate<TState> terminalNodePredicate;
        private readonly Func<TState, TPlayer, IEnumerable<Node<TState, TDatum>>> childNodeExtractor;
        private readonly TPlayer player;
        private readonly TPlayer opponent;

        public NegamaxCalculator(Predicate<TState> terminalNodePredicate,
            Func<TState, TPlayer, int> scoreCalculator,
            Func<TState, TPlayer, IEnumerable<Node<TState, TDatum>>> childNodeExtractor,
            TPlayer player, TPlayer opponent)
        {
            this.childNodeExtractor = childNodeExtractor;
            this.terminalNodePredicate = terminalNodePredicate;
            this.scoreCalculator = scoreCalculator;
            this.player = player;
            this.opponent = opponent;
        }

        public ScoredNode Negamax(Node<TState, TDatum> node)
        {
            return Negamax(node, player);
        }

        //Potential changes
        //Score is calculated by working out who the current player is, not rely on it being parsed in
        //TDatum maybe not necessary, can calculate the best move from the best possible next game state
        //Using tuple instead of ScoredNode
        public ScoredNode Negamax(Node<TState, TDatum> node, TPlayer currentPlayer,
            int alpha = -1000, int beta = 1000)
        {
            if (NodeIsTerminal(node))
            {
                return new ScoredNode(ScoreOfNode(node, currentPlayer), node);
            }

            var bestNode = new ScoredNode(-1000, null);
            foreach (var childNode in GetChildren(node, currentPlayer))
            {
                var score = -Negamax(childNode, SwapPlayer(currentPlayer), -beta, -alpha).Score;
                if (score > bestNode.Score)
                {
                    bestNode = new ScoredNode(score, childNode);
                }

                if ((alpha = Math.Max(alpha, score)) >= beta)
                {
                    break;
                }
            }

            return bestNode;
        }

        private IEnumerable<Node<TState,TDatum>> GetChildren(Node<TState, TDatum> node, TPlayer currentPlayer)
        {
            return childNodeExtractor(node.State, currentPlayer);
        }

        private int ScoreOfNode(Node<TState, TDatum> node, TPlayer currentPlayer)
        {
            return scoreCalculator(node.State, currentPlayer);
        }

        private bool NodeIsTerminal(Node<TState, TDatum> node)
        {
            return terminalNodePredicate(node.State);
        }

        private TPlayer SwapPlayer(TPlayer currentPlayer)
        {
            return currentPlayer.Equals(opponent) ? player : opponent;
        }

        public class ScoredNode
        {
            public ScoredNode(int score, Node<TState, TDatum>  node)
            {
                Score = score;
                Node = node;
            }

            public int Score{ get; set; }
            public Node<TState, TDatum> Node { get; set; }
        }

    }
}
