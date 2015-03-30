namespace Negamax
{
    public class Node<N, V>
    {
        public N State { get; set; }
        public V Datum { get; set; }

        public Node(N state, V datum){
            State = state;
            Datum = datum;
        }
    }
}
