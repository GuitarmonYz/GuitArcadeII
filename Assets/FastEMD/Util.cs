using System.Collections.Generic;
using System;
namespace FastEMD {
    public class Edge {
        public int _to;
        public long _cost;
        public Edge (int to, long cost) {
            _to = to;
            _cost = cost;
        }
    }
    public class Edge0 {
        public Edge0(int to, long cost, long flow) {
            _to = to;
            _cost = cost;
            _flow = flow;
        }

        public int _to;
        public long _cost;
        public long _flow;
    }

    public class Edge1 {
        public Edge1(int to, long reduced_cost) {
            _to = to;
            _reduced_cost = reduced_cost;
        }

        public int _to;
        public long _reduced_cost;
    }

    class Edge2 {
        public Edge2(int to, long reduced_cost, long residual_capacity) {
            _to = to;
            _reduced_cost = reduced_cost;
            _residual_capacity = residual_capacity;
        }

        public int _to;
        public long _reduced_cost;
        public long _residual_capacity;
    }

    class Edge3 {
        public Edge3() {
            _to = 0;
            _dist = 0;
        }

        public Edge3(int to, long dist) {
            _to = to;
            _dist = dist;
        }

        public int _to;
        public long _dist;
    }

    class MinCostFlow {

        int numNodes;
        List<int> nodesToQ;

        // e - supply(positive) and demand(negative).
        // c[i] - edges that goes from node i. first is the second nod
        // x - the flow is returned in it
        public long compute(List<long> e, List<List<Edge>> c, List<List<Edge0>> x) {
            numNodes = e.Count;
            nodesToQ = new List<int>();
            for (int i = 0; i < numNodes; i++) {
                nodesToQ.Add(0);
            }

            // init flow
            for (int from = 0; from < numNodes; ++from) {
                foreach (Edge it in c[from]) {
                    x[from].Add(new Edge0(it._to, it._cost, 0));
                    x[it._to].Add(new Edge0(from, -it._cost, 0));
                }
            }

            // reduced costs for forward edges (c[i,j]-pi[i]+pi[j])
            // Note that for forward edges the residual capacity is infinity
            List<List<Edge1>> rCostForward = new List<List<Edge1>>();
            for (int i = 0; i < numNodes; i++) {
                rCostForward.Add(new List<Edge1>());
            }
            for (int from = 0; from < numNodes; ++from) {
                foreach (Edge it in c[from]) {
                    rCostForward[from].Add(new Edge1(it._to, it._cost));
                }
            }

            // reduced costs and capacity for backward edges
            // (c[j,i]-pi[j]+pi[i])
            // Since the flow at the beginning is 0, the residual capacity is
            // also zero
            List<List<Edge2>> rCostCapBackward = new List<List<Edge2>>();
            for (int i = 0; i < numNodes; i++) {
                rCostCapBackward.Add(new List<Edge2>());
            }
            for (int from = 0; from < numNodes; ++from) {
                foreach (Edge it in c[from]) {
                    rCostCapBackward[it._to].Add(
                            new Edge2(from, -it._cost, 0));
                }
            }

            // Max supply TODO:demand?, given U?, optimization-> min out of
            // demand,supply
            long U = 0;
            for (int i = 0; i < numNodes; i++) {
                if (e[i] > U)
                    U = e[i];
            }
            long delta = (long) (Math.Pow(2.0,
                    Math.Ceiling(Math.Log((double) (U)) / Math.Log(2.0))));

            List<long> d = new List<long>();
            List<int> prev = new List<int>();
            for (int i = 0; i < numNodes; i++) {
                d.Add(0L);
                prev.Add(0);
            }
            delta = 1;
            while (true) { // until we break when S or T is empty
                long maxSupply = 0;
                int k = 0;
                for (int i = 0; i < numNodes; i++) {
                    if (e[i] > 0) {
                        if (maxSupply < e[i]) {
                            maxSupply = e[i];
                            k = i;
                        }
                    }
                }
                if (maxSupply == 0)
                    break;
                delta = maxSupply;

                int[] l = new int[1];
                computeShortestPath(d, prev, k, rCostForward, rCostCapBackward,
                        e, l);

                // find delta (minimum on the path from k to l)
                // delta= e[k];
                // if (-e[l]<delta) delta= e[k];
                int to = l[0];
                do {
                    int from = prev[to];
                    // residual
                    int itccb = 0;
                    while ((itccb < rCostCapBackward[from].Count)
                            && (rCostCapBackward[from][itccb]._to != to)) {
                        itccb++;
                    }
                    if (itccb < rCostCapBackward[from].Count) {
                        if (rCostCapBackward[from][itccb]._residual_capacity < delta)
                            delta = rCostCapBackward[from][itccb]._residual_capacity;
                    }

                    to = from;
                } while (to != k);

                // augment delta flow from k to l (backwards actually...)
                to = l[0];
                do {
                    int from = prev[to];
                    // TODO - might do here O(n) can be done in O(1)
                    int itx = 0;
                    while (x[from][itx]._to != to) {
                        itx++;
                    }
                    x[from][itx]._flow += delta;

                    // update residual for backward edges
                    int itccb = 0;
                    while ((itccb < rCostCapBackward[to].Count)
                            && (rCostCapBackward[to][itccb]._to != from)) {
                        itccb++;
                    }
                    if (itccb < rCostCapBackward[to].Count) {
                        rCostCapBackward[to][itccb]._residual_capacity += delta;
                    }
                    itccb = 0;
                    while ((itccb < rCostCapBackward[from].Count)
                            && (rCostCapBackward[from][itccb]._to != to)) {
                        itccb++;
                    }
                    if (itccb < rCostCapBackward[from].Count) {
                        rCostCapBackward[from][itccb]._residual_capacity -= delta;
                    }

                    // update e
                    e[to] += delta;
                    e[from] -= delta;

                    to = from;
                } while (to != k);
            }

            // compute distance from x
            long dist = 0;
            for (int from = 0; from < numNodes; from++) {
                foreach (Edge0 it in x[from]) {
                    dist += (it._cost * it._flow);
                }
            }
            return dist;
        }

        void computeShortestPath(List<long> d, List<int> prev,
                int from, List<List<Edge1>> costForward,
                List<List<Edge2>> costBackward, List<long> e, int[] l) {
            // Making heap (all inf except 0, so we are saving comparisons...)
            List<Edge3> Q = new List<Edge3>();
            for (int i = 0; i < numNodes; i++) {
                Q.Add(new Edge3());
            }

            Q[0]._to = from;
            nodesToQ[from] = 0;
            Q[0]._dist = 0;

            int j = 1;
            // TODO: both of these into a function?
            for (int i = 0; i < from; ++i) {
                Q[j]._to = i;
                nodesToQ[i] = j;
                Q[j]._dist = long.MaxValue;
                j++;
            }

            for (int i = from + 1; i < numNodes; i++) {
                Q[j]._to = i;
                nodesToQ[i] = j;
                Q[j]._dist = long.MaxValue;
                j++;
            }

            List<Boolean> finalNodesFlg = new List<Boolean>();
            for (int i = 0; i < numNodes; i++) {
                finalNodesFlg.Add(false);
            }
            do {
                int u = Q[0]._to;

                d[u] = Q[0]._dist; // final distance
                finalNodesFlg[u] = true;
                if (e[u] < 0) {
                    l[0] = u;
                    break;
                }

                heapRemoveFirst(Q, nodesToQ);

                // neighbors of u
                foreach (Edge1 it in costForward[u]) {
                    long alt = d[u] + it._reduced_cost;
                    int v = it._to;
                    if ((nodesToQ[v] < Q.Count)
                            && (alt < Q[nodesToQ[v]]._dist)) {
                        heapDecreaseKey(Q, nodesToQ, v, alt);
                        prev[v] = u;
                    }
                }
                foreach (Edge2 it in costBackward[u]) {
                    if (it._residual_capacity > 0) {
                        long alt = d[u] + it._reduced_cost;
                        int v = it._to;
                        if ((nodesToQ[v] < Q.Count)
                                && (alt < Q[nodesToQ[v]]._dist)) {
                            heapDecreaseKey(Q, nodesToQ, v, alt);
                            prev[v] = u;
                        }
                    }
                }

            } while (Q.Count > 0);

            for (int _from = 0; _from < numNodes; ++_from) {
                foreach (Edge1 it in costForward[_from]) {
                    if (finalNodesFlg[_from]) {
                        it._reduced_cost += d[_from] - d[l[0]];
                    }
                    if (finalNodesFlg[it._to]) {
                        it._reduced_cost -= d[it._to] - d[l[0]];
                    }
                }
            }

            // reduced costs and capacity for backward edges
            // (c[j,i]-pi[j]+pi[i])
            for (int _from = 0; _from < numNodes; ++_from) {
                foreach (Edge2 it in costBackward[_from]) {
                    if (finalNodesFlg[_from]) {
                        it._reduced_cost += d[_from] - d[l[0]];
                    }
                    if (finalNodesFlg[it._to]) {
                        it._reduced_cost -= d[it._to] - d[l[0]];
                    }
                }
            }
        }

        void heapDecreaseKey(List<Edge3> Q, List<int> nodes_to_Q,
                int v, long alt) {
            int i = nodes_to_Q[v];
            Q[i]._dist = alt;
            while (i > 0 && Q[PARENT(i)]._dist > Q[i]._dist) {
                swapHeap(Q, nodes_to_Q, i, PARENT(i));
                i = PARENT(i);
            }
        }

        void heapRemoveFirst(List<Edge3> Q, List<int> nodes_to_Q) {
            swapHeap(Q, nodes_to_Q, 0, Q.Count - 1);
            Q.RemoveAt(Q.Count - 1);
            heapify(Q, nodes_to_Q, 0);
        }

        void heapify(List<Edge3> Q, List<int> nodes_to_Q, int i) {
            do {
                // TODO: change to loop
                int l = LEFT(i);
                int r = RIGHT(i);
                int smallest;
                if ((l < Q.Count) && (Q[l]._dist < Q[i]._dist)) {
                    smallest = l;
                } else {
                    smallest = i;
                }
                if ((r < Q.Count) && (Q[r]._dist < Q[smallest]._dist)) {
                    smallest = r;
                }

                if (smallest == i)
                    return;

                swapHeap(Q, nodes_to_Q, i, smallest);
                i = smallest;

            } while (true);
        }

        void swapHeap(List<Edge3> Q, List<int> nodesToQ, int i, int j) {
            Edge3 tmp = Q[i];
            Q[i] = Q[j];
            Q[j] = tmp;
            nodesToQ[Q[j]._to] = j;
            nodesToQ[Q[i]._to] = i;
        }

        int LEFT(int i) {
            return 2 * (i + 1) - 1;
        }

        int RIGHT(int i) {
            return 2 * (i + 1); // 2 * (i + 1) + 1 - 1
        }

        int PARENT(int i) {
            return (i - 1) / 2;
        }
    }
}