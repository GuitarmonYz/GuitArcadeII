using System;
using System.Collections;
using System.Collections.Generic;
namespace FastEMD {
    public class EMDProcessor{
        static public double distance(Signiture signature1, Signiture signature2, double extraMassPenalty) {
            List<Double> P = new List<Double>();
            List<Double> Q = new List<Double>();
            for (int i = 0; i < signature1.getNumberOfFeatures() + signature2.getNumberOfFeatures(); i++) {
                P.Add(0.0);
                Q.Add(0.0);
            }
            for (int i = 0; i < signature1.getNumberOfFeatures(); i++) {
                P[i] = signature1.getWeights()[i];
            }
            for (int j = 0; j < signature2.getNumberOfFeatures(); j++) {
                Q[j + signature1.getNumberOfFeatures()] = signature2.getWeights()[j];
            }

            List<List<Double>> C = new List<List<Double>>();
            for (int i = 0; i < P.Count; i++) {
                List<Double> vec = new List<Double>();
                for (int j = 0; j < P.Count; j++) {
                    vec.Add(0.0f);
                }
                C.Add(vec);
            }
            for (int i = 0; i < signature1.getNumberOfFeatures(); i++) {
                for (int j = 0; j < signature2.getNumberOfFeatures(); j++) {
                    double dist = signature1.getFeatures()[i].groudDist(signature2.getFeatures()[j]);
                    C[i][j + signature1.getNumberOfFeatures()] = dist;
                    C[j + signature1.getNumberOfFeatures()][i] = dist;
                }
            }
            double totalCost_P = 0;
            double totalCost_Q = 0;
            for (int i = 0; i < signature1.getWeights().Length; i++) {
                totalCost_P += signature1.getWeights()[i];
            }
            for (int i = 0; i < signature2.getWeights().Length; i++) {
                totalCost_Q += signature2.getWeights()[i];
            }

            return emdHat(P, Q, C, extraMassPenalty) / Math.Max(totalCost_P, totalCost_Q);
        }
        static private double emdHat(List<Double> P, List<Double> Q, List<List<Double>> C, double extraMassPenalty) {
            // This condition should hold:
            // ( 2^(sizeof(CONVERT_TO_T*8)) >= ( MULT_FACTOR^2 )
            // Note that it can be problematic to check it because
            // of overflow problems. I simply checked it with Linux calc
            // which has arbitrary precision.
            double MULT_FACTOR = 1000000;

            // Constructing the input
            int N = P.Count;
            List<long> iP = new List<long>();
            List<long> iQ = new List<long>();
            List<List<long>> iC = new List<List<long>>();
            for (int i = 0; i < N; i++) {
                iP.Add(0L);
                iQ.Add(0L);
                List<long> vec = new List<long>();
                for (int j = 0; j < N; j++) {
                    vec.Add(0L);
                }
                iC.Add(vec);
            }

            // Converting to CONVERT_TO_T
            double sumP = 0.0;
            double sumQ = 0.0;
            double maxC = C[0][0];
            for (int i = 0; i < N; i++) {
                sumP += P[i];
                sumQ += Q[i];
                for (int j = 0; j < N; j++) {
                    if (C[i][j] > maxC)
                        maxC = C[i][j];
                }
            }
            double minSum = Math.Min(sumP, sumQ);
            double maxSum = Math.Max(sumP, sumQ);
            double PQnormFactor = MULT_FACTOR / maxSum;
            double CnormFactor = MULT_FACTOR / maxC;
            for (int i = 0; i < N; i++) {
                iP[i] = (long) (Math.Floor(P[i] * PQnormFactor + 0.5f));
                iQ[i] = (long) (Math.Floor(Q[i] * PQnormFactor + 0.5f));
                for (int j = 0; j < N; j++) {
                    iC[i][j] = (long) (Math.Floor(C[i][j] * CnormFactor + 0.5));
                }
            }

            // computing distance without extra mass penalty
            double dist = emdHatImplLongLongInt(iP, iQ, iC, 0);
            // unnormalize
            dist = dist / PQnormFactor;
            dist = dist / CnormFactor;

            // adding extra mass penalty
            if (extraMassPenalty == -1)
                extraMassPenalty = maxC;
            dist += (maxSum - minSum) * extraMassPenalty;
            
            return dist;
        }

        static private long emdHatImplLongLongInt(List<long> Pc, List<long> Qc,
                List<List<long>> C, long extraMassPenalty) {

            int N = Pc.Count;
            // Ensuring that the supplier - P, have more mass.
            // Note that we assume here that C is symmetric
            List<long> P;
            List<long> Q;
            long absDiffSumPSumQ;
            long sumP = 0;
            long sumQ = 0;
            for (int i = 0; i < N; i++)
                sumP += Pc[i];
            for (int i = 0; i < N; i++)
                sumQ += Qc[i];
            if (sumQ > sumP) {
                P = Qc;
                Q = Pc;
                absDiffSumPSumQ = sumQ - sumP;
            } else {
                P = Pc;
                Q = Qc;
                absDiffSumPSumQ = sumP - sumQ;
            }

            // creating the b vector that contains all vertexes
            List<long> b = new List<long>();
            for (int i = 0; i < 2 * N + 2; i++) {
                b.Add(0L);
            }
            int THRESHOLD_NODE = 2 * N;
            int ARTIFICIAL_NODE = 2 * N + 1; // need to be last !
            for (int i = 0; i < N; i++) {
                b[i] = P[i];
            }
            for (int i = N; i < 2 * N; i++) {
                b[i] = Q[i - N];
            }

            // remark*) I put here a deficit of the extra mass, as mass that flows
            // to the threshold node
            // can be absorbed from all sources with cost zero (this is in reverse
            // order from the paper,
            // where incoming edges to the threshold node had the cost of the
            // threshold and outgoing
            // edges had the cost of zero)
            // This also makes sum of b zero.
            b[THRESHOLD_NODE] = -absDiffSumPSumQ;
            b[ARTIFICIAL_NODE] = 0L;

            long maxC = 0;
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    if (C[i][j] > maxC)
                        maxC = C[i][j];
                }
            }
            if (extraMassPenalty == -1)
                extraMassPenalty = maxC;

            HashSet<int> sourcesThatFlowNotOnlyToThresh = new HashSet<int>();
            HashSet<int> sinksThatGetFlowNotOnlyFromThresh = new HashSet<int>();
            long preFlowCost = 0;

            // regular edges between sinks and sources without threshold edges
            List<List<Edge>> c = new List<List<Edge>>();
            for (int i = 0; i < b.Count; i++) {
                c.Add(new List<Edge>());
            }
            for (int i = 0; i < N; i++) {
                if (b[i] == 0)
                    continue;
                for (int j = 0; j < N; j++) {
                    if (b[j + N] == 0)
                        continue;
                    if (C[i][j] == maxC)
                        continue;
                    c[i].Add(new Edge(j + N, C[i][j]));
                }
            }

            // checking which are not isolated
            for (int i = 0; i < N; i++) {
                if (b[i] == 0)
                    continue;
                for (int j = 0; j < N; j++) {
                    if (b[j + N] == 0)
                        continue;
                    if (C[i][j] == maxC)
                        continue;
                    sourcesThatFlowNotOnlyToThresh.Add(i);
                    sinksThatGetFlowNotOnlyFromThresh.Add(j + N);
                }
            }

            // converting all sinks to negative
            for (int i = N; i < 2 * N; i++) {
                b[i] = -b[i];
            }

            // add edges from/to threshold node,
            // note that costs are reversed to the paper (see also remark* above)
            // It is important that it will be this way because of remark* above.
            for (int i = 0; i < N; ++i) {
                c[i].Add(new Edge(THRESHOLD_NODE, 0));
            }
            for (int j = 0; j < N; ++j) {
                c[THRESHOLD_NODE].Add(new Edge(j + N, maxC));
            }

            // artificial arcs - Note the restriction that only one edge i,j is
            // artificial so I ignore it...
            for (int i = 0; i < ARTIFICIAL_NODE; i++) {
                c[i].Add(new Edge(ARTIFICIAL_NODE, maxC + 1));
                c[ARTIFICIAL_NODE].Add(new Edge(i, maxC + 1));
            }

            // remove nodes with supply demand of 0
            // and vertexes that are connected only to the
            // threshold vertex
            int currentNodeName = 0;
            // Note here it should be vector<int> and not vector<int>
            // as I'm using -1 as a special flag !!!
            int REMOVE_NODE_FLAG = -1;
            List<int> nodesNewNames = new List<int>();
            List<int> nodesOldNames = new List<int>();
            for (int i = 0; i < b.Count; i++) {
                nodesNewNames.Add(REMOVE_NODE_FLAG);
                nodesOldNames.Add(0);
            }
            for (int i = 0; i < N * 2; i++) {
                if (b[i] != 0) {
                    if (sourcesThatFlowNotOnlyToThresh.Contains(i)
                            || sinksThatGetFlowNotOnlyFromThresh.Contains(i)) {
                        nodesNewNames[i] = currentNodeName;
                        nodesOldNames.Add(i);
                        currentNodeName++;
                    } else {
                        if (i >= N) {
                            preFlowCost -= (b[i] * maxC);
                        }
                        b[THRESHOLD_NODE] = b[THRESHOLD_NODE] + b[i]; // add mass(i<N) or deficit (i>=N)
                    }
                }
            }
            nodesNewNames[THRESHOLD_NODE] = currentNodeName;
            nodesOldNames.Add(THRESHOLD_NODE);
            currentNodeName++;
            nodesNewNames[ARTIFICIAL_NODE] = currentNodeName;
            nodesOldNames.Add(ARTIFICIAL_NODE);
            currentNodeName++;

            List<long> bb = new List<long>();
            for (int i = 0; i < currentNodeName; i++) {
                bb.Add(0L);
            }
            // int j = 0;
            for (int i = 0, j = 0; i < b.Count; i++) {
                if (nodesNewNames[i] != REMOVE_NODE_FLAG) {
                    bb[j] = b[i];
                    j++;
                }
            }

            List<List<Edge>> cc = new List<List<Edge>>();
            for (int i = 0; i < bb.Count; i++) {
                cc.Add(new List<Edge>());
            }
            for (int i = 0; i < c.Count; i++) {
                if (nodesNewNames[i] == REMOVE_NODE_FLAG)
                    continue;
                foreach (Edge it in c[i]) {
                    if (nodesNewNames[it._to] != REMOVE_NODE_FLAG) {
                        cc[nodesNewNames[i]].Add(
                                new Edge(nodesNewNames[it._to], it._cost));
                    }
                }
            }

            MinCostFlow mcf = new MinCostFlow();

            long myDist;

            List<List<Edge0>> flows = new List<List<Edge0>>(bb.Count);
            for (int i = 0; i < bb.Count; i++) {
                flows.Add(new List<Edge0>());
            }

            long mcfDist = mcf.compute(bb, cc, flows);

            myDist = preFlowCost + // pre-flowing on cases where it was possible
                    mcfDist + // solution of the transportation problem
                    (absDiffSumPSumQ * extraMassPenalty); // emd-hat extra mass penalty

            return myDist;
        }
    } 
}