using System;
using System.Collections.Generic;

namespace Negamax
{
    public class NegamaxCalculator<TNode>
    {
        private readonly Func<TNode, int> scoreCalculator;
        private readonly Predicate<TNode> terminalNodePredicate;
        private readonly Func<TNode, IEnumerable<TNode>> childNodeExtractor;

        public NegamaxCalculator(Predicate<TNode> terminalNodePredicate,
            Func<TNode, int> scoreCalculator,
            Func<TNode, IEnumerable<TNode>> childNodeExtractor)
        {
            this.childNodeExtractor = childNodeExtractor;
            this.terminalNodePredicate = terminalNodePredicate;
            this.scoreCalculator = scoreCalculator;
        }

        public TNode FindBestNode(TNode node)
        {
            return Negamax(node).Node;
        }

        public ScoredNode Negamax(TNode node, int alpha = -1000, int beta = 1000)
        {
            if (NodeIsTerminal(node))
            {
                return new ScoredNode(ScoreOfNode(node), node);
            }

            var bestNode = new ScoredNode(-1000, node);
            foreach (var childNode in GetChildren(node))
            {
                var score = -Negamax(childNode, -beta, -alpha).Score;
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

        private IEnumerable<TNode> GetChildren(TNode node)
        {
            return childNodeExtractor(node);
        }

        private int ScoreOfNode(TNode node)
        {
            return scoreCalculator(node);
        }

        private bool NodeIsTerminal(TNode node)
        {
            return terminalNodePredicate(node);
        }

        public class ScoredNode
        {
            public ScoredNode(int score, TNode node)
            {
                Score = score;
                Node = node;
            }

            public int Score{ get; set; }
            public TNode Node { get; set; }
        }

    }
}
