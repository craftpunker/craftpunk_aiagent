
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle
{
    public class AgentTreeNode
    {
        internal int begin_;
        internal int end_;
        internal int left_;
        internal int right_;
        internal Fix64 maxX_;
        internal Fix64 maxY_;
        internal Fix64 minX_;
        internal Fix64 minY_;
    }

    public struct FloatPair
    {
        private Fix64 a_;
        private Fix64 b_;

        internal FloatPair(Fix64 a, Fix64 b)
        {
            a_ = a;
            b_ = b;
        }

        public static bool operator <(FloatPair pair1, FloatPair pair2)
        {
            return pair1.a_ < pair2.a_ || !(pair2.a_ < pair1.a_) && pair1.b_ < pair2.b_;
        }

        public static bool operator <=(FloatPair pair1, FloatPair pair2)
        {
            return (pair1.a_ == pair2.a_ && pair1.b_ == pair2.b_) || pair1 < pair2;
        }

        public static bool operator >(FloatPair pair1, FloatPair pair2)
        {
            return !(pair1 <= pair2);
        }

        public static bool operator >=(FloatPair pair1, FloatPair pair2)
        {
            return !(pair1 < pair2);
        }
    }

    public class KdTree
    {

        private class ObstacleTreeNode
        {
            internal Obstacle obstacle_;
            internal ObstacleTreeNode left_;
            internal ObstacleTreeNode right_;
        };

        private const int MAX_LEAF_SIZE = 10;

        //private Agent[] agents_;
        //private AgentTreeNode[] agentTree_;

        private List<Agent> agents_;
        private List<AgentTreeNode> agentTree_;
        private ObstacleTreeNode obstacleTree_;

        public void Release()
        {
            agents_?.Clear();
            agentTree_?.Clear();
            //agents_ = null;
            //agentTree_ = null;
            obstacleTree_ = null;
        }

        internal void buildAgentTree()
        {
            if (agents_ == null)
            {
                agents_ = new List<Agent>(Simulator.instance.agents_.Count);
            }

            if (agentTree_ == null)
            {
                agentTree_ = new List<AgentTreeNode>(2 * agents_.Count);
            }

            if (agents_.Count != Simulator.instance.agents_.Count)
            {
                var count = Simulator.instance.agents_.Count;
                agents_.Clear();
                //for (int i = 0; i < count; i++)
                //{
                //    agents_[i] = Simulator.instance.agents_[i];
                //}

                agents_.AddRange(Simulator.instance.agents_);

                //agentTree_ = new AgentTreeNode[2 * agents_.Length];
                agentTree_.Clear();
                var agentCount = count * 2;

                for (int i = 0; i < agentCount; ++i)
                {
                    agentTree_.Add(ClassPool.instance.Pop<AgentTreeNode>());
                }
            }

            if (agents_.Count != 0)
            {
                buildAgentTreeRecursive(0, agents_.Count, 0);
            }
        }

        //internal void buildAgentTree()
        //{
        //    if (agents_ == null || agents_.Length != Simulator.instance.agents_.Count)
        //    {
        //        agents_ = new Agent[Simulator.instance.agents_.Count];

        //        for (int i = 0; i < agents_.Length; ++i)
        //        {
        //            agents_[i] = Simulator.instance.agents_[i];
        //        }

        //        agentTree_ = new AgentTreeNode[2 * agents_.Length];

        //        for (int i = 0; i < agentTree_.Length; ++i)
        //        {
        //            agentTree_[i] = new AgentTreeNode();
        //        }
        //    }

        //    if (agents_.Length != 0)
        //    {
        //        buildAgentTreeRecursive(0, agents_.Length, 0);
        //    }
        //}

        public void buildObstacleTree()
        {
            obstacleTree_ = new ObstacleTreeNode();

            IList<Obstacle> obstacles = new List<Obstacle>(Simulator.instance.obstacles_.Count);

            for (int i = 0; i < Simulator.instance.obstacles_.Count; ++i)
            {
                obstacles.Add(Simulator.instance.obstacles_[i]);
            }

            obstacleTree_ = buildObstacleTreeRecursive(obstacles);
        }

        internal void computeAgentNeighbors(Agent agent, ref Fix64 rangeSq, bool isFindEnemy = false)
        {
            agent.agentNeighbors_.Clear();
            queryAgentTreeRecursive(agent, ref rangeSq, 0, isFindEnemy);
        }

        internal void computeAgentNeighborsByPos(FixVector2 pos, ref Fix64 rangeSq, PlayerGroup targetGroup, ref List<Agent> agents, int count)
        {
            queryAgentTreeRecursiveByPos(pos, ref rangeSq, 0, targetGroup, ref agents, count);
        }

        internal void computeObstacleNeighbors(Agent agent, Fix64 rangeSq)
        {
            queryObstacleTreeRecursive(agent, rangeSq, obstacleTree_);
        }

        internal bool queryVisibility(FixVector2 q1, FixVector2 q2, Fix64 radius)
        {
            return queryVisibilityRecursive(q1, q2, radius, obstacleTree_);
        }

        private void buildAgentTreeRecursive(int begin, int end, int node)
        {
            agentTree_[node].begin_ = begin;
            agentTree_[node].end_ = end;
            agentTree_[node].minX_ = agentTree_[node].maxX_ = agents_[begin].position_.x;
            agentTree_[node].minY_ = agentTree_[node].maxY_ = agents_[begin].position_.y;

            for (int i = begin + 1; i < end; ++i)
            {
                agentTree_[node].maxX_ = Fix64.Max(agentTree_[node].maxX_, agents_[i].position_.x);
                agentTree_[node].minX_ = Fix64.Min(agentTree_[node].minX_, agents_[i].position_.x);
                agentTree_[node].maxY_ = Fix64.Max(agentTree_[node].maxY_, agents_[i].position_.y);
                agentTree_[node].minY_ = Fix64.Min(agentTree_[node].minY_, agents_[i].position_.y);
            }

            if (end - begin > MAX_LEAF_SIZE)
            {
                /* No leaf node. */
                bool isVertical = agentTree_[node].maxX_ - agentTree_[node].minX_ > agentTree_[node].maxY_ - agentTree_[node].minY_;
                Fix64 splitValue = (Fix64)0.5 * (isVertical ? agentTree_[node].maxX_ + agentTree_[node].minX_ : agentTree_[node].maxY_ + agentTree_[node].minY_);

                int left = begin;
                int right = end;

                while (left < right)
                {
                    while (left < right && (isVertical ? agents_[left].position_.x : agents_[left].position_.y) < splitValue)
                    {
                        ++left;
                    }

                    while (right > left && (isVertical ? agents_[right - 1].position_.x : agents_[right - 1].position_.y) >= splitValue)
                    {
                        --right;
                    }

                    if (left < right)
                    {
                        Agent tempAgent = agents_[left];
                        agents_[left] = agents_[right - 1];
                        agents_[right - 1] = tempAgent;
                        ++left;
                        --right;
                    }
                }

                int leftSize = left - begin;

                if (leftSize == 0)
                {
                    ++leftSize;
                    ++left;
                    ++right;
                }

                agentTree_[node].left_ = node + 1;
                agentTree_[node].right_ = node + 2 * leftSize;

                buildAgentTreeRecursive(begin, left, agentTree_[node].left_);
                buildAgentTreeRecursive(left, end, agentTree_[node].right_);
            }


        }

        private ObstacleTreeNode buildObstacleTreeRecursive(IList<Obstacle> obstacles)
        {
            if (obstacles.Count == 0)
            {
                return null;
            }

            ObstacleTreeNode node = new ObstacleTreeNode();

            int optimalSplit = 0;
            int minLeft = obstacles.Count;
            int minRight = obstacles.Count;

            for (int i = 0; i < obstacles.Count; ++i)
            {
                int leftSize = 0;
                int rightSize = 0;

                Obstacle obstacleI1 = obstacles[i];
                Obstacle obstacleI2 = obstacleI1.next_;

                /* Compute optimal split node. */
                for (int j = 0; j < obstacles.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Obstacle obstacleJ1 = obstacles[j];
                    Obstacle obstacleJ2 = obstacleJ1.next_;

                    Fix64 j1LeftOfI = FixMath.LeftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ1.point_);
                    Fix64 j2LeftOfI = FixMath.LeftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ2.point_);

                    if (j1LeftOfI >= -Fix64.RVO_EPSILON && j2LeftOfI >= -Fix64.RVO_EPSILON)
                    {
                        ++leftSize;
                    }
                    else if (j1LeftOfI <= Fix64.RVO_EPSILON && j2LeftOfI <= Fix64.RVO_EPSILON)
                    {
                        ++rightSize;
                    }
                    else
                    {
                        ++leftSize;
                        ++rightSize;
                    }

                    if (new FloatPair(Fix64.Max(leftSize, rightSize), Fix64.Min(leftSize, rightSize)) >= new FloatPair(Fix64.Max(minLeft, minRight), Fix64.Min(minLeft, minRight)))
                    {
                        break;
                    }
                }

                if (new FloatPair(Fix64.Max(leftSize, rightSize), Fix64.Min(leftSize, rightSize)) < new FloatPair(Fix64.Max(minLeft, minRight), Fix64.Min(minLeft, minRight)))
                {
                    minLeft = leftSize;
                    minRight = rightSize;
                    optimalSplit = i;
                }
            }

            {
                /* Build split node. */
                IList<Obstacle> leftObstacles = new List<Obstacle>(minLeft);

                for (int n = 0; n < minLeft; ++n)
                {
                    leftObstacles.Add(null);
                }

                IList<Obstacle> rightObstacles = new List<Obstacle>(minRight);

                for (int n = 0; n < minRight; ++n)
                {
                    rightObstacles.Add(null);
                }

                int leftCounter = 0;
                int rightCounter = 0;
                int i = optimalSplit;

                Obstacle obstacleI1 = obstacles[i];
                Obstacle obstacleI2 = obstacleI1.next_;

                for (int j = 0; j < obstacles.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Obstacle obstacleJ1 = obstacles[j];
                    Obstacle obstacleJ2 = obstacleJ1.next_;

                    Fix64 j1LeftOfI = FixMath.LeftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ1.point_);
                    Fix64 j2LeftOfI = FixMath.LeftOf(obstacleI1.point_, obstacleI2.point_, obstacleJ2.point_);

                    if (j1LeftOfI >= -Fix64.RVO_EPSILON && j2LeftOfI >= -Fix64.RVO_EPSILON)
                    {
                        leftObstacles[leftCounter++] = obstacles[j];
                    }
                    else if (j1LeftOfI <= Fix64.RVO_EPSILON && j2LeftOfI <= Fix64.RVO_EPSILON)
                    {
                        rightObstacles[rightCounter++] = obstacles[j];
                    }
                    else
                    {
                        /* Split obstacle j. */
                        Fix64 t = FixVector2.Det(obstacleI2.point_ - obstacleI1.point_, obstacleJ1.point_ - obstacleI1.point_) / FixVector2.Det(obstacleI2.point_ - obstacleI1.point_, obstacleJ1.point_ - obstacleJ2.point_);

                        FixVector2 splitPoint = obstacleJ1.point_ + t * (obstacleJ2.point_ - obstacleJ1.point_);

                        Obstacle newObstacle = new Obstacle();
                        newObstacle.point_ = splitPoint;
                        newObstacle.previous_ = obstacleJ1;
                        newObstacle.next_ = obstacleJ2;
                        newObstacle.convex_ = true;
                        newObstacle.direction_ = obstacleJ1.direction_;

                        newObstacle.id_ = Simulator.instance.obstacles_.Count;

                        Simulator.instance.obstacles_.Add(newObstacle);

                        obstacleJ1.next_ = newObstacle;
                        obstacleJ2.previous_ = newObstacle;

                        if (j1LeftOfI > Fix64.Zero)
                        {
                            leftObstacles[leftCounter++] = obstacleJ1;
                            rightObstacles[rightCounter++] = newObstacle;
                        }
                        else
                        {
                            rightObstacles[rightCounter++] = obstacleJ1;
                            leftObstacles[leftCounter++] = newObstacle;
                        }
                    }
                }

                node.obstacle_ = obstacleI1;
                node.left_ = buildObstacleTreeRecursive(leftObstacles);
                node.right_ = buildObstacleTreeRecursive(rightObstacles);

                return node;
            }
        }

        private void queryAgentTreeRecursive(Agent agent, ref Fix64 rangeSq, int node, bool isFindEnemy)
        {
            if (agentTree_[node].end_ - agentTree_[node].begin_ <= MAX_LEAF_SIZE)
            {
                for (int i = agentTree_[node].begin_; i < agentTree_[node].end_; ++i)
                {
                    var otherAgent = agents_[i];
                    if (isFindEnemy)
                    {
                        if (otherAgent.entity.BKilled || agent.entity.PlayerGroup == otherAgent.entity.PlayerGroup || !otherAgent.entity.IsSoldier || BattleMgr.instance.CheckStage(otherAgent.entity, StageConst.Stealth))
                            continue;
                    }

                    if (agent.orcaType == OrcaType.CloseOrca)
                    {
                        continue;
                    }

                    if (agent.orcaType == OrcaType.NoTeamOrca)
                    {
                        if (otherAgent.entity.SoldierFlag == agent.entity.SoldierFlag)
                            continue;
                    }

                    agent.insertAgentNeighbor(otherAgent, ref rangeSq, isFindEnemy);
                }
            }
            else
            {
                Fix64 distSqLeft = Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].left_].minX_ - agent.position_.x)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.position_.x - agentTree_[agentTree_[node].left_].maxX_)) + Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].left_].minY_ - agent.position_.y)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.position_.y - agentTree_[agentTree_[node].left_].maxY_));
                Fix64 distSqRight = Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].right_].minX_ - agent.position_.x)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.position_.x - agentTree_[agentTree_[node].right_].maxX_)) + Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].right_].minY_ - agent.position_.y)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.position_.y - agentTree_[agentTree_[node].right_].maxY_));

                if (distSqLeft < distSqRight)
                {
                    if (distSqLeft < rangeSq)
                    {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left_, isFindEnemy);

                        if (distSqRight < rangeSq)
                        {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right_, isFindEnemy);
                        }
                    }
                }
                else
                {
                    if (distSqRight < rangeSq)
                    {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right_, isFindEnemy);

                        if (distSqLeft < rangeSq)
                        {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left_, isFindEnemy);
                        }
                    }
                }

            }
        }

        //返回范围内agent集合,count=-1是无穷
        public void queryAgentTreeRecursiveByPos(FixVector2 pos, ref Fix64 rangeSq, int node, PlayerGroup targetGroup, ref List<Agent> agents, int count)
        {
            if (agentTree_[node].end_ - agentTree_[node].begin_ <= MAX_LEAF_SIZE)
            {
                for (int i = agentTree_[node].begin_; i < agentTree_[node].end_; ++i)
                {
                    var otherAgent = agents_[i];
                    if (otherAgent.entity.BKilled || targetGroup != otherAgent.entity.PlayerGroup || !otherAgent.entity.IsSoldier)
                        continue;

                    //agent.insertAgentNeighbor(agents_[i], ref rangeSq, isFindEnemy);

                    Fix64 distSq = FixVector2.SqrMagnitude(pos - otherAgent.position_);

                    if (distSq < rangeSq + otherAgent.entity.RadiusSq)
                    {
                        if (agents.Count < count || count == -1)
                            agents.Add(otherAgent);
                        else
                            return;
                    }

                }
            }
            else
            {
                Fix64 distSqLeft = Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].left_].minX_ - pos.x)) + Fix64.Square(Fix64.Max(Fix64.Zero, pos.x - agentTree_[agentTree_[node].left_].maxX_)) + Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].left_].minY_ - pos.y)) + Fix64.Square(Fix64.Max(Fix64.Zero, pos.y - agentTree_[agentTree_[node].left_].maxY_));
                Fix64 distSqRight = Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].right_].minX_ - pos.x)) + Fix64.Square(Fix64.Max(Fix64.Zero, pos.x - agentTree_[agentTree_[node].right_].maxX_)) + Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].right_].minY_ - pos.y)) + Fix64.Square(Fix64.Max(Fix64.Zero, pos.y - agentTree_[agentTree_[node].right_].maxY_));

                if (distSqLeft < distSqRight)
                {
                    if (distSqLeft < rangeSq)
                    {
                        queryAgentTreeRecursiveByPos(pos, ref rangeSq, agentTree_[node].left_, targetGroup, ref agents, count);

                        if (distSqRight < rangeSq)
                        {
                            queryAgentTreeRecursiveByPos(pos, ref rangeSq, agentTree_[node].right_, targetGroup, ref agents, count);
                        }
                    }
                }
                else
                {
                    if (distSqRight < rangeSq)
                    {
                        queryAgentTreeRecursiveByPos(pos, ref rangeSq, agentTree_[node].right_, targetGroup, ref agents, count);

                        if (distSqLeft < rangeSq)
                        {
                            queryAgentTreeRecursiveByPos(pos, ref rangeSq, agentTree_[node].left_, targetGroup, ref agents, count);
                        }
                    }
                }

            }
        }

        private void queryObstacleTreeRecursive(Agent agent, Fix64 rangeSq, ObstacleTreeNode node)
        {
            if (node != null)
            {
                Obstacle obstacle1 = node.obstacle_;
                Obstacle obstacle2 = obstacle1.next_;

                Fix64 agentLeftOfLine = FixMath.LeftOf(obstacle1.point_, obstacle2.point_, agent.position_);

                queryObstacleTreeRecursive(agent, rangeSq, agentLeftOfLine >= Fix64.Zero ? node.left_ : node.right_);

                Fix64 distSqLine = Fix64.Square(agentLeftOfLine) / FixVector2.SqrMagnitude(obstacle2.point_ - obstacle1.point_);

                if (distSqLine < rangeSq)
                {
                    if (agentLeftOfLine < 0.0f)
                    {
                        /*
                            * Try obstacle at this node only if agent is on right side of
                            * obstacle (and can see obstacle).
                            */
                        agent.insertObstacleNeighbor(node.obstacle_, rangeSq);
                    }

                    /* Try other side of line. */
                    queryObstacleTreeRecursive(agent, rangeSq, agentLeftOfLine >= Fix64.Zero ? node.right_ : node.left_);
                }
            }
        }

        private bool queryVisibilityRecursive(FixVector2 q1, FixVector2 q2, Fix64 radius, ObstacleTreeNode node)
        {
            if (node == null)
            {
                return true;
            }

            Obstacle obstacle1 = node.obstacle_;
            Obstacle obstacle2 = obstacle1.next_;

            Fix64 q1LeftOfI = FixMath.LeftOf(obstacle1.point_, obstacle2.point_, q1);
            Fix64 q2LeftOfI = FixMath.LeftOf(obstacle1.point_, obstacle2.point_, q2);
            Fix64 invLengthI = Fix64.One / FixVector2.SqrMagnitude(obstacle2.point_ - obstacle1.point_);

            if (q1LeftOfI >= Fix64.Zero && q2LeftOfI >= Fix64.Zero)
            {
                return queryVisibilityRecursive(q1, q2, radius, node.left_) && ((Fix64.Square(q1LeftOfI) * invLengthI >= Fix64.Square(radius) && Fix64.Square(q2LeftOfI) * invLengthI >= Fix64.Square(radius)) || queryVisibilityRecursive(q1, q2, radius, node.right_));
            }

            if (q1LeftOfI <= Fix64.Zero && q2LeftOfI <= Fix64.Zero)
            {
                return queryVisibilityRecursive(q1, q2, radius, node.right_) && ((Fix64.Square(q1LeftOfI) * invLengthI >= Fix64.Square(radius) && Fix64.Square(q2LeftOfI) * invLengthI >= Fix64.Square(radius)) || queryVisibilityRecursive(q1, q2, radius, node.left_));
            }

            if (q1LeftOfI >= Fix64.Zero && q2LeftOfI <= Fix64.Zero)
            {
                /* One can see through obstacle from left to right. */
                return queryVisibilityRecursive(q1, q2, radius, node.left_) && queryVisibilityRecursive(q1, q2, radius, node.right_);
            }

            Fix64 point1LeftOfQ = FixMath.LeftOf(q1, q2, obstacle1.point_);
            Fix64 point2LeftOfQ = FixMath.LeftOf(q1, q2, obstacle2.point_);
            Fix64 invLengthQ = Fix64.One / FixVector2.SqrMagnitude(q2 - q1);

            return point1LeftOfQ * point2LeftOfQ >= Fix64.Zero && Fix64.Square(point1LeftOfQ) * invLengthQ > Fix64.Square(radius) && Fix64.Square(point2LeftOfQ) * invLengthQ > Fix64.Square(radius) && queryVisibilityRecursive(q1, q2, radius, node.left_) && queryVisibilityRecursive(q1, q2, radius, node.right_);
        }
    }
}

