using Battle;
using System.Collections.Generic;
using System.Linq;

public class SoldierFlagKDTree
{
    private const int MAX_LEAF_SIZE = 10;

    private List<SoldierFlagBase> agents_;
    private List<AgentTreeNode> agentTree_;

    public void Delete(int id)
    {

    }

    int soldierFlagagentTreeCount = 0;
    internal void buildAgentTree()
    {
        if (agents_ == null)
        {
            agents_ = BattleMgr.instance.SoldierFlagDict.Values.ToList();
        }

        if (agentTree_ == null)
        {
            agentTree_ = new List<AgentTreeNode>(2 * agents_.Count);
        }

        if (agents_.Count != soldierFlagagentTreeCount)
        {
            soldierFlagagentTreeCount = agents_.Count;
            //agents_.Clear();
            //for (int i = 0; i < count; i++)
            //{
            //    agents_[i] = Simulator.instance.agents_[i];
            //}

            //agents_.AddRange(Simulator.instance.agents_);

            //agentTree_ = new AgentTreeNode[2 * agents_.Length];
            agentTree_.Clear();
            var agentCount = soldierFlagagentTreeCount * 2;

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

    private void buildAgentTreeRecursive(int begin, int end, int node)
    {
        agentTree_[node].begin_ = begin;
        agentTree_[node].end_ = end;
        agentTree_[node].minX_ = agentTree_[node].maxX_ = agents_[begin].Pos.x;
        agentTree_[node].minY_ = agentTree_[node].maxY_ = agents_[begin].Pos.y;

        for (int i = begin + 1; i < end; ++i)
        {
            agentTree_[node].maxX_ = Fix64.Max(agentTree_[node].maxX_, agents_[i].Pos.x);
            agentTree_[node].minX_ = Fix64.Min(agentTree_[node].minX_, agents_[i].Pos.x);
            agentTree_[node].maxY_ = Fix64.Max(agentTree_[node].maxY_, agents_[i].Pos.y);
            agentTree_[node].minY_ = Fix64.Min(agentTree_[node].minY_, agents_[i].Pos.y);
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
                while (left < right && (isVertical ? agents_[left].Pos.x : agents_[left].Pos.y) < splitValue)
                {
                    ++left;
                }

                while (right > left && (isVertical ? agents_[right - 1].Pos.x : agents_[right - 1].Pos.y) >= splitValue)
                {
                    --right;
                }

                if (left < right)
                {
                    SoldierFlagBase tempAgent = agents_[left];
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

    public void queryAgentTreeRecursive(SoldierFlagBase agent, ref Fix64 rangeSq, int node, bool isFindEnemy)
    {
        if (agentTree_[node].end_ - agentTree_[node].begin_ <= MAX_LEAF_SIZE)
        {
            for (int i = agentTree_[node].begin_; i < agentTree_[node].end_; ++i)
            {
                if (isFindEnemy)
                {
                    if (agents_[i].BKilled || agent.PlayerGroup == agents_[i].PlayerGroup)
                        continue;
                }

                agent.insertAgentNeighbor(agents_[i], ref rangeSq, isFindEnemy);
            }
        }
        else
        {
            Fix64 distSqLeft = Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].left_].minX_ - agent.Pos.x)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.Pos.x - agentTree_[agentTree_[node].left_].maxX_)) + Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].left_].minY_ - agent.Pos.y)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.Pos.y - agentTree_[agentTree_[node].left_].maxY_));
            Fix64 distSqRight = Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].right_].minX_ - agent.Pos.x)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.Pos.x - agentTree_[agentTree_[node].right_].maxX_)) + Fix64.Square(Fix64.Max(Fix64.Zero, agentTree_[agentTree_[node].right_].minY_ - agent.Pos.y)) + Fix64.Square(Fix64.Max(Fix64.Zero, agent.Pos.y - agentTree_[agentTree_[node].right_].maxY_));

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

    public void Release()
    {
        agents_ = null;
        agentTree_.Clear();
        soldierFlagagentTreeCount = 0;
    }

}