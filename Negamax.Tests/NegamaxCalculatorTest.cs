using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Negamax.Tests
{
    [TestFixture]
    public class NegamaxCalculatorTestulatorTest
    {
        Predicate<int> alwaysTruePredicate = _ => true;
        readonly Func<int, int> EchoIntFunction = i => i;

        /*
         * 0("NodeDatum") - no children
         * Score of node is equal to node state, in this case that is 0
         */
        [Test]
        public void ScoreOfBestNodeIsCalculatedUsingPassedInFunction()
        {
            const int score = 10;
            Func<int, int> scoreCalculator = i => score;
            var bestNode = new NegamaxCalculator<int>(alwaysTruePredicate, scoreCalculator, null)
                .Negamax(0);
            Assert.AreEqual(score, bestNode.Score);
        }

        /*
         * 0 - no children
         */
        [Test]
        public void ValueOfBestNodeIsValueOfTerminalNode()
        {
            const int node = 0;
            var bestNode = new NegamaxCalculator<int>(alwaysTruePredicate, EchoIntFunction, null)
                .Negamax(node);
            Assert.AreEqual(node, bestNode.Node);
        }

        /* 0 parent node
         * \ 1 child node
         * best node = 1
         */
        [Test]
        public void BestNodeValueWillBeTheSingleChildOfParentNode()
        {
            var childNode = 1;
            var bestNode = RunNegamaxWithChild(childNode);
            Assert.AreEqual(bestNode.Node, childNode);
        }

        /* 0 parent node
         * \ 1 child node
         * score of best child node = -1
         */
        [Test]
        public void ScoreOfBestNodeWillBeNegatedScoreOfOnlyChildNode()
        {
            const int childNode = 1;
            var bestNode = RunNegamaxWithChild(childNode);
            Assert.AreEqual(bestNode.Score, -childNode);
        }

        /* 0("ParentNode")    parent node
         * \ 1("A"), 2("B")   children nodes
         * child node with highest score = 2
         */
        [Test]
        public void BestNodeWillBeTheChildWithHighestScore()
        {
            var bestNode = RunNegamaxWithChildren(BuildListOfChildren(1, 2));
            Assert.AreEqual(2, bestNode.Node);
        }

        static NegamaxCalculator<int>.ScoredNode RunNegamaxWithChildren(IEnumerable<int> children)
        {
            const int parentNode = 0;
            var childExtractor = ParentToChildExtractorFunction(parentNode, children);
            var bestNode = new NegamaxCalculator<int>(TerminateAtChildNode(parentNode),
                               ScoreCalculatingFunction(), childExtractor)
                .Negamax(0);
            return bestNode;
        }

        static Node<int, string> NewTrackingNode(int node, string tracker)
        {
            return new Node<int, string>(node, tracker);
        }

        static Func<int, int> ScoreCalculatingFunction()
        {
            return x => -x;
        }

        static Predicate<int> TerminateAtChildNode(int parentNode)
        {
            return x => x != parentNode;
        }

        static Func<int, IEnumerable<int>> ParentToChildExtractorFunction(int parentNode,
                IEnumerable<int> children)
        {
            return x => x == parentNode ? children : Enumerable.Empty<int>();
        }

        static IList<int> BuildListOfChildren(params int[] children)
        {
            return children.ToList();
        }

        private NegamaxCalculator<int>.ScoredNode RunNegamaxWithChild(int childNode)
        {
            const int parentNode = 0;
            var childExtractor = ParentToChildExtractorFunction(parentNode, BuildListOfChildren(childNode));
            var bestNode = new NegamaxCalculator<int>(TerminateAtChildNode(parentNode),
                    EchoIntFunction, childExtractor).Negamax(parentNode);
            return bestNode;
        }

    }
}
