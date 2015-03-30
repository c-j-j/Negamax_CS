using NUnit.Framework;

namespace Negamax.Test
{
    [TestFixture]
    public class NodeTest
    {
        static void NewMethod(Node<int, string> trackingNode)
        {
            Assert.AreEqual(trackingNode.State, 0);
        }

        [Test]
        public void CreatesTrackingChildNode()
        {
            const int node = 0;
            const string tracker = "Tracker";
            var trackingNode = new Node<int, string>(node, tracker);
            NewMethod(trackingNode);
            Assert.AreEqual(trackingNode.Datum, tracker);
        }
    }

}
