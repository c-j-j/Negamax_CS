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
        readonly Func<int, char, int> EchoIntFunction = (i, c) => i;
        const char PLAYER = 'p';
        const char OPPONENT = 'o';

        /*
         * 0("NodeDatum") - no children
         * Score of node is equal to node state, in this case that is 0
         */
        [Test]
        public void ScoreOfBestNodeIsCalculatedUsingPassedInFunction()
        {
            const int score = 10;
            Func<int, char,  int> scoreCalculator = (i, c) => score;
            var bestNode = new NegamaxCalculator<int, char, string>(alwaysTruePredicate, scoreCalculator, null, PLAYER, OPPONENT)
                .Negamax(new Node<int, string>(0, "NodeDatum"));
            Assert.AreEqual(score, bestNode.Score);
        }

        /*
         * 0("NodeDatum") - no children
         */
        [Test]
        public void ValueOfBestNodeIsValueOfTerminalNode()
        {
            const int node = 0;
            const string childNodeValue = "NodeDatum";
            var bestNode = new NegamaxCalculator<int, char, string>(alwaysTruePredicate, EchoIntFunction, null, PLAYER, OPPONENT)
                .Negamax(new Node<int, string>(node, childNodeValue));
            Assert.AreEqual(childNodeValue, bestNode.Node.Datum);
        }

        /* 0("ParentNode") parent node
         * \ 1("A") child node
         * best node = 1
         */
        [Test]
        public void BestNodeValueWillBeTheSingleChildOfParentNode()
        {
            const string childNodeValue = "A";
            var bestNode = RunNegamaxWithChild(new Node<int, string>(1, childNodeValue));
            Assert.AreEqual(bestNode.Node.Datum, childNodeValue);
        }

        /* 0("ParentNode") parent node
         * \ 1("A") child node
         * score of best child node = -1
         */
        [Test]
        public void ScoreOfBestNodeWillBeNegatedScoreOfOnlyChildNode()
        {
            const int childNode = 1;
            var bestNode = RunNegamaxWithChild(new Node<int, string>(childNode, "A"));
            Assert.AreEqual(bestNode.Score, -childNode);
        }

        /* 0("ParentNode")    parent node
         * \ 1("A"), 2("B")   children nodes
         * child node with highest score = 2
         */
        [Test]
        public void BestNodeWillBeTheChildWithHighestScore()
        {
            const string childNodeValueB = "B";
            var bestNode = RunNegamaxWithChildren(BuildListOfChildren(NewTrackingNode(1, "A"),
                                   NewTrackingNode(2, childNodeValueB)));
            Assert.AreEqual(childNodeValueB, bestNode.Node.Datum);
        }

        static NegamaxCalculator<int, char, string>.ScoredNode RunNegamaxWithChildren(IEnumerable<Node<int, string>> children)
        {
            const int parentNode = 0;
            var childExtractor = ParentToChildExtractorFunction(parentNode, children);
            var bestNode = new NegamaxCalculator<int, char, string>(TerminateAtChildNode(parentNode),
                               ScoreCalculator(), childExtractor, PLAYER, OPPONENT)
                .Negamax(new Node<int, string>(parentNode, "0"));
            return bestNode;
        }

        static Node<int, string> NewTrackingNode(int node, string tracker)
        {
            return new Node<int, string>(node, tracker);
        }

        static Func<int, char, int> ScoreCalculator()
        {
            return (x, c) => c == OPPONENT ? -x : x;
        }

        static Predicate<int> TerminateAtChildNode(int parentNode)
        {
            return x => x != parentNode;
        }

        static Func<int, char, IEnumerable<Node<int, string>>> ParentToChildExtractorFunction(int parentNode,
                IEnumerable<Node<int, string>> children)
        {
            return (x, c) => x == parentNode ? children : Enumerable.Empty<Node<int, string>>();
        }

        static IList<Node<int, string>> BuildListOfChildren(params Node<int, string>[] children)
        {
            return children.ToList();
        }

        private NegamaxCalculator<int, char, string>.ScoredNode RunNegamaxWithChild(Node<int, string> childNodeTracker)
        {
            const int parentNode = 0;
            var childExtractor = ParentToChildExtractorFunction(parentNode, BuildListOfChildren(childNodeTracker));
            var parentNodeTracker = new Node<int, string>(parentNode, "ParentNodeValue");
            var bestNode = new NegamaxCalculator<int, char, string>(TerminateAtChildNode(parentNode),
                    EchoIntFunction, childExtractor, PLAYER, OPPONENT).Negamax(parentNodeTracker);
            return bestNode;
        }

    }
}
