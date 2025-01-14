

using System.Collections.Generic;

namespace Battle
{
    public enum OrcaType
    {
        CloseOrca = 0, //关闭避让
        AllOrca = 1, //避让所有
        NoTeamOrca = 2, //同队不避让
    }

    public class Agent
    {
        internal IList<KeyValuePair<Fix64, Agent>> agentNeighbors_ = new List<KeyValuePair<Fix64, Agent>>();
        internal IList<KeyValuePair<Fix64, Obstacle>> obstacleNeighbors_ = new List<KeyValuePair<Fix64, Obstacle>>();
        internal IList<Line> orcaLines_ = new List<Line>();
        internal FixVector2 position_;
        internal FixVector2 prefVelocity_;
        internal FixVector2 velocity_;
        internal int id_ = 0;
        internal int maxNeighbors_ = 0;
        internal Fix64 maxSpeed_ = Fix64.Zero;
        internal Fix64 neighborDist_ = Fix64.Zero;
        internal Fix64 radius_ = Fix64.Zero;
        internal Fix64 timeHorizon_ = Fix64.Zero;
        internal Fix64 timeHorizonObst_ = Fix64.Zero;
        internal bool needDelete_ = false;
        internal OrcaType orcaType;
        //public bool isORCA = false; //避让开关
        //public bool isSearchEnemy = false;
        public EntityBase entity;

        private FixVector2 newVelocity_;

        internal void Release()
        {
            agentNeighbors_.Clear();
            obstacleNeighbors_.Clear();
            orcaLines_.Clear();
            needDelete_ = false;
            orcaType = OrcaType.CloseOrca;
            entity = null;
            position_ = new FixVector2();
            prefVelocity_ = new FixVector2();
            velocity_ = new FixVector2();

            maxSpeed_ = Fix64.Zero;
            neighborDist_ = Fix64.Zero;
            radius_ = Fix64.Zero;
            timeHorizon_ = Fix64.Zero;
            timeHorizonObst_ = Fix64.Zero;
            newVelocity_ = new FixVector2();

            ClassPool.instance.Push(this);
        }

        internal void computeNeighbors()
        {
            obstacleNeighbors_.Clear();
            Fix64 rangeSq = Fix64.Square(timeHorizonObst_ * maxSpeed_ + radius_);
            Simulator.instance.kdTree_.computeObstacleNeighbors(this, rangeSq);

            agentNeighbors_.Clear();

            if (maxNeighbors_ > 0)
            {
                rangeSq = Fix64.Square(neighborDist_);
                Simulator.instance.kdTree_.computeAgentNeighbors(this, ref rangeSq);
            }
        }

        internal void computeNewVelocity()
        {
            orcaLines_.Clear();

            Fix64 invTimeHorizonObst = Fix64.One / timeHorizonObst_;

            /* Create obstacle ORCA lines. */
            for (int i = 0; i < obstacleNeighbors_.Count; ++i)
            {

                Obstacle obstacle1 = obstacleNeighbors_[i].Value;
                Obstacle obstacle2 = obstacle1.next_;

                FixVector2 relativePosition1 = obstacle1.point_ - position_;
                FixVector2 relativePosition2 = obstacle2.point_ - position_;

                /*
                 * Check if velocity obstacle of obstacle is already taken care
                 * of by previously constructed obstacle ORCA lines.
                 */
                bool alreadyCovered = false;

                for (int j = 0; j < orcaLines_.Count; ++j)
                {
                    if (FixVector2.Det(invTimeHorizonObst * relativePosition1 - orcaLines_[j].point, orcaLines_[j].direction) - invTimeHorizonObst * radius_ >= -Fix64.RVO_EPSILON && FixVector2.Det(invTimeHorizonObst * relativePosition2 - orcaLines_[j].point, orcaLines_[j].direction) - invTimeHorizonObst * radius_ >= -Fix64.RVO_EPSILON)
                    {
                        alreadyCovered = true;

                        break;
                    }
                }

                if (alreadyCovered)
                {
                    continue;
                }

                /* Not yet covered. Check for collisions. */
                Fix64 distSq1 = FixVector2.SqrMagnitude(relativePosition1);
                Fix64 distSq2 = FixVector2.SqrMagnitude(relativePosition2);

                Fix64 radiusSq = Fix64.Square(radius_);

                FixVector2 obstacleVector = obstacle2.point_ - obstacle1.point_;
                Fix64 s = FixVector2.Dot(-relativePosition1, obstacleVector) / FixVector2.SqrMagnitude(obstacleVector);
                Fix64 distSqLine = FixVector2.SqrMagnitude(-relativePosition1 - s * obstacleVector);

                Line line;

                if (s < 0.0f && distSq1 <= radiusSq)
                {
                    /* Collision with left vertex. Ignore if non-convex. */
                    if (obstacle1.convex_)
                    {
                        line.point = FixVector2.Zero;
                        line.direction = new FixVector2(-relativePosition1.y, relativePosition1.x).GetNormalized();
                        orcaLines_.Add(line);
                    }

                    continue;
                }
                else if (s > Fix64.One && distSq2 <= radiusSq)
                {
                    /*
                     * Collision with right vertex. Ignore if non-convex or if
                     * it will be taken care of by neighboring obstacle.
                     */
                    if (obstacle2.convex_ && FixVector2.Det(relativePosition2, obstacle2.direction_) >= Fix64.Zero)
                    {
                        line.point = FixVector2.Zero;
                        line.direction = new FixVector2(-relativePosition2.y, relativePosition2.x).GetNormalized();
                        orcaLines_.Add(line);
                    }

                    continue;
                }
                else if (s >= Fix64.Zero && s < Fix64.One && distSqLine <= radiusSq)
                {
                    /* Collision with obstacle segment. */
                    line.point = FixVector2.Zero;
                    line.direction = -obstacle1.direction_;
                    orcaLines_.Add(line);

                    continue;
                }

                /*
                 * No collision. Compute legs. When obliquely viewed, both legs
                 * can come from a single vertex. Legs extend cut-off line when
                 * non-convex vertex.
                 */

                FixVector2 leftLegDirection, rightLegDirection;

                if (s < Fix64.Zero && distSqLine <= radiusSq)
                {
                    /*
                     * Obstacle viewed obliquely so that left vertex
                     * defines velocity obstacle.
                     */
                    if (!obstacle1.convex_)
                    {
                        /* Ignore obstacle. */
                        continue;
                    }

                    obstacle2 = obstacle1;

                    Fix64 leg1 = Fix64.Sqrt(distSq1 - radiusSq);
                    leftLegDirection = new FixVector2(relativePosition1.x * leg1 - relativePosition1.y * radius_, relativePosition1.x * radius_ + relativePosition1.y * leg1) / distSq1;
                    rightLegDirection = new FixVector2(relativePosition1.x * leg1 + relativePosition1.y * radius_, -relativePosition1.x * radius_ + relativePosition1.y * leg1) / distSq1;
                }
                else if (s > Fix64.One && distSqLine <= radiusSq)
                {
                    /*
                     * Obstacle viewed obliquely so that
                     * right vertex defines velocity obstacle.
                     */
                    if (!obstacle2.convex_)
                    {
                        /* Ignore obstacle. */
                        continue;
                    }

                    obstacle1 = obstacle2;

                    Fix64 leg2 = Fix64.Sqrt(distSq2 - radiusSq);
                    leftLegDirection = new FixVector2(relativePosition2.x * leg2 - relativePosition2.y * radius_, relativePosition2.x * radius_ + relativePosition2.y * leg2) / distSq2;
                    rightLegDirection = new FixVector2(relativePosition2.x * leg2 + relativePosition2.y * radius_, -relativePosition2.x * radius_ + relativePosition2.y * leg2) / distSq2;
                }
                else
                {
                    /* Usual situation. */
                    if (obstacle1.convex_)
                    {
                        Fix64 leg1 = Fix64.Sqrt(distSq1 - radiusSq);
                        leftLegDirection = new FixVector2(relativePosition1.x * leg1 - relativePosition1.y * radius_, relativePosition1.x * radius_ + relativePosition1.y * leg1) / distSq1;
                    }
                    else
                    {
                        /* Left vertex non-convex; left leg extends cut-off line. */
                        leftLegDirection = -obstacle1.direction_;
                    }

                    if (obstacle2.convex_)
                    {
                        Fix64 leg2 = Fix64.Sqrt(distSq2 - radiusSq);
                        rightLegDirection = new FixVector2(relativePosition2.x * leg2 + relativePosition2.y * radius_, -relativePosition2.x * radius_ + relativePosition2.y * leg2) / distSq2;
                    }
                    else
                    {
                        /* Right vertex non-convex; right leg extends cut-off line. */
                        rightLegDirection = obstacle1.direction_;
                    }
                }

                /*
                 * Legs can never point into neighboring edge when convex
                 * vertex, take cutoff-line of neighboring edge instead. If
                 * velocity projected on "foreign" leg, no constraint is added.
                 */

                Obstacle leftNeighbor = obstacle1.previous_;

                bool isLeftLegForeign = false;
                bool isRightLegForeign = false;

                if (obstacle1.convex_ && FixVector2.Det(leftLegDirection, -leftNeighbor.direction_) >= Fix64.Zero)
                {
                    /* Left leg points into obstacle. */
                    leftLegDirection = -leftNeighbor.direction_;
                    isLeftLegForeign = true;
                }

                if (obstacle2.convex_ && FixVector2.Det(rightLegDirection, obstacle2.direction_) <= Fix64.Zero)
                {
                    /* Right leg points into obstacle. */
                    rightLegDirection = obstacle2.direction_;
                    isRightLegForeign = true;
                }

                /* Compute cut-off centers. */
                FixVector2 leftCutOff = invTimeHorizonObst * (obstacle1.point_ - position_);
                FixVector2 rightCutOff = invTimeHorizonObst * (obstacle2.point_ - position_);
                FixVector2 cutOffVector = rightCutOff - leftCutOff;

                /* Project current velocity on velocity obstacle. */

                /* Check if current velocity is projected on cutoff circles. */
                Fix64 t = obstacle1 == obstacle2 ? (Fix64)0.5 : FixVector2.Dot((velocity_ - leftCutOff), cutOffVector) / FixVector2.SqrMagnitude(cutOffVector);
                Fix64 tLeft = FixVector2.Dot((velocity_ - leftCutOff), leftLegDirection);
                Fix64 tRight = FixVector2.Dot((velocity_ - rightCutOff), rightLegDirection);

                if ((t < Fix64.Zero && tLeft < Fix64.Zero) || (obstacle1 == obstacle2 && tLeft < Fix64.Zero && tRight < Fix64.Zero))
                {
                    /* Project on left cut-off circle. */
                    FixVector2 unitW = (velocity_ - leftCutOff).GetNormalized();

                    line.direction = new FixVector2(unitW.y, -unitW.x);
                    line.point = leftCutOff + radius_ * invTimeHorizonObst * unitW;
                    orcaLines_.Add(line);

                    continue;
                }
                else if (t > Fix64.One && tRight < Fix64.Zero)
                {
                    /* Project on right cut-off circle. */
                    FixVector2 unitW = (velocity_ - rightCutOff).GetNormalized();

                    line.direction = new FixVector2(unitW.y, -unitW.x);
                    line.point = rightCutOff + radius_ * invTimeHorizonObst * unitW;
                    orcaLines_.Add(line);

                    continue;
                }

                /*
                 * Project on left leg, right leg, or cut-off line, whichever is
                 * closest to velocity.
                 */
                Fix64 distSqCutoff = (t < Fix64.Zero || t > Fix64.One || obstacle1 == obstacle2) ? Fix64.MaxValue : FixVector2.SqrMagnitude(velocity_ - (leftCutOff + t * cutOffVector));
                Fix64 distSqLeft = tLeft < Fix64.Zero ? Fix64.MaxValue : FixVector2.SqrMagnitude(velocity_ - (leftCutOff + tLeft * leftLegDirection));
                Fix64 distSqRight = tRight <Fix64.Zero ? Fix64.MaxValue : FixVector2.SqrMagnitude(velocity_ - (rightCutOff + tRight * rightLegDirection));

                if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight)
                {
                    /* Project on cut-off line. */
                    line.direction = -obstacle1.direction_;
                    line.point = leftCutOff + radius_ * invTimeHorizonObst * new FixVector2(-line.direction.y, line.direction.x);
                    orcaLines_.Add(line);

                    continue;
                }

                if (distSqLeft <= distSqRight)
                {
                    /* Project on left leg. */
                    if (isLeftLegForeign)
                    {
                        continue;
                    }

                    line.direction = leftLegDirection;
                    line.point = leftCutOff + radius_ * invTimeHorizonObst * new FixVector2(-line.direction.y, line.direction.x);
                    orcaLines_.Add(line);

                    continue;
                }

                /* Project on right leg. */
                if (isRightLegForeign)
                {
                    continue;
                }

                line.direction = -rightLegDirection;
                line.point = rightCutOff + radius_ * invTimeHorizonObst * new FixVector2(-line.direction.y, line.direction.x);
                orcaLines_.Add(line);
            }

            int numObstLines = orcaLines_.Count;

            Fix64 invTimeHorizon = Fix64.One / timeHorizon_;

            /* Create agent ORCA lines. */
            for (int i = 0; i < agentNeighbors_.Count; ++i)
            {
                Agent other = agentNeighbors_[i].Value;

                if (other.entity.PlayerGroup != entity.PlayerGroup)
                    continue;

                if (other.entity.Radius < entity.Radius)
                    continue;

                FixVector2 relativePosition = other.position_ - position_;
                FixVector2 relativeVelocity = velocity_ - other.velocity_;
                Fix64 distSq = FixVector2.SqrMagnitude(relativePosition);
                Fix64 combinedRadius = radius_ + other.radius_;
                Fix64 combinedRadiusSq = Fix64.Square(combinedRadius);

                Line line;
                FixVector2 u;

                if (distSq > combinedRadiusSq)
                {
                    /* No collision. */
                    FixVector2 w = relativeVelocity - invTimeHorizon * relativePosition;

                    /* Vector from cutoff center to relative velocity. */
                    Fix64 wLengthSq = FixVector2.SqrMagnitude(w);
                    Fix64 dotProduct1 = FixVector2.Dot(w, relativePosition);

                    if (dotProduct1 < Fix64.Zero && Fix64.Square(dotProduct1) > combinedRadiusSq * wLengthSq)
                    {
                        /* Project on cut-off circle. */
                        Fix64 wLength = Fix64.Sqrt(wLengthSq);
                        if (wLength == Fix64.Zero)
                        {
                            wLength = Fix64.RVO_EPSILON;
                        }

                        FixVector2 unitW = w / wLength;

                        line.direction = new FixVector2(unitW.y, -unitW.x);
                        u = (combinedRadius * invTimeHorizon - wLength) * unitW;
                    }
                    else
                    {
                        /* Project on legs. */
                        Fix64 leg = Fix64.Sqrt(distSq - combinedRadiusSq);

                        if (FixVector2.Det(relativePosition, w) > Fix64.Zero)
                        {
                            /* Project on left leg. */
                            line.direction = new FixVector2(relativePosition.x * leg - relativePosition.y * combinedRadius, relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
                        }
                        else
                        {
                            /* Project on right leg. */
                            line.direction = new FixVector2(relativePosition.x * leg + relativePosition.y * combinedRadius, -relativePosition.x * combinedRadius + relativePosition.y * leg) / -distSq;
                        }

                        Fix64 dotProduct2 = FixVector2.Dot(relativeVelocity, line.direction);
                        u = dotProduct2 * line.direction - relativeVelocity;
                    }
                }
                else
                {
                    /* Collision. Project on cut-off circle of time timeStep. */
                    //Fix64 invTimeStep = Fix64.One / PlayerSimulator.instance.timeStep_;
                    Fix64 invTimeStep =  Fix64.One / GameData.instance._FixFrameLen;

                    /* Vector from cutoff center to relative velocity. */
                    FixVector2 w = relativeVelocity - invTimeStep * relativePosition;

                    Fix64 wLength = FixVector2.Magnitude(w);
                    if (wLength == Fix64.Zero)
                        wLength = Fix64.RVO_EPSILON;

                    FixVector2 unitW = w / wLength; 

                    line.direction = new FixVector2(unitW.y, -unitW.x);
                    u = (combinedRadius * invTimeStep - wLength) * unitW;
                }

                line.point = velocity_ + (Fix64)0.5 * u ;
                orcaLines_.Add(line);
            }

            int lineFail = linearProgram2(orcaLines_, maxSpeed_, prefVelocity_, false, ref newVelocity_);

            if (lineFail < orcaLines_.Count)
            {
                linearProgram3(orcaLines_, numObstLines, lineFail, maxSpeed_, ref newVelocity_);
            }
        }

        internal void insertAgentNeighbor(Agent agent, ref Fix64 rangeSq, bool isFindEnemy)
        {
            if (this != agent)
            {
                Fix64 distSq = FixVector2.SqrMagnitude(position_ - agent.position_);

                if (distSq < rangeSq)
                {
                    if (agentNeighbors_.Count < maxNeighbors_)
                    {
                        agentNeighbors_.Add(new KeyValuePair<Fix64, Agent>(distSq, agent));
                    }

                    int i = agentNeighbors_.Count - 1;

                    while (i != 0 && distSq < agentNeighbors_[i - 1].Key)
                    {
                        agentNeighbors_[i] = agentNeighbors_[i - 1];
                        --i;
                    }

                    agentNeighbors_[i] = new KeyValuePair<Fix64, Agent>(distSq, agent);

                    rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
                    //if (agentNeighbors_.Count == maxNeighbors_)
                    //{
                    //    rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
                    //}
                }
            }
        }

        internal void insertObstacleNeighbor(Obstacle obstacle, Fix64 rangeSq)
        {
            Obstacle nextObstacle = obstacle.next_;

            Fix64 distSq = FixVector2.distSqPointLineSegment(obstacle.point_, nextObstacle.point_, position_);

            if (distSq < rangeSq)
            {
                obstacleNeighbors_.Add(new KeyValuePair<Fix64, Obstacle>(distSq, obstacle));

                int i = obstacleNeighbors_.Count - 1;

                while (i != 0 && distSq < obstacleNeighbors_[i - 1].Key)
                {
                    obstacleNeighbors_[i] = obstacleNeighbors_[i - 1];
                    --i;
                }
                obstacleNeighbors_[i] = new KeyValuePair<Fix64, Obstacle>(distSq, obstacle);
            }
        }

        internal void update()
        {
            velocity_ = newVelocity_;
            position_ += velocity_ * (GameData.instance._FixFrameLen * entity.CurrAttrValue.MoveSpeed);
            position_ = GameUtils.CheckMapBorder(position_);
        }

        private bool linearProgram1(IList<Line> lines, int lineNo, Fix64 radius, FixVector2 optVelocity, bool directionOpt, ref FixVector2 result)
        {
            Fix64 dotProduct = FixVector2.Dot(lines[lineNo].point, lines[lineNo].direction);
            Fix64 discriminant = Fix64.Square(dotProduct) + Fix64.Square(radius) - FixVector2.SqrMagnitude(lines[lineNo].point);

            if (discriminant < Fix64.Zero)
            {
                /* Max speed circle fully invalidates line lineNo. */
                return false;
            }

            Fix64 sqrtDiscriminant = Fix64.Sqrt(discriminant);
            Fix64 tLeft = -dotProduct - sqrtDiscriminant;
            Fix64 tRight = -dotProduct + sqrtDiscriminant;

            for (int i = 0; i < lineNo; ++i)
            {
                Fix64 denominator = FixVector2.Det(lines[lineNo].direction, lines[i].direction);
                Fix64 numerator = FixVector2.Det(lines[i].direction, lines[lineNo].point - lines[i].point);

                if (Fix64.Abs(denominator) <= Fix64.RVO_EPSILON)
                {
                    /* Lines lineNo and i are (almost) parallel. */
                    if (numerator < Fix64.Zero)
                    {
                        return false;
                    }

                    continue;
                }

                Fix64 t = numerator / denominator;

                if (denominator >= Fix64.Zero)
                {
                    /* Line i bounds line lineNo on the right. */
                    tRight = Fix64.Min(tRight, t);
                }
                else
                {
                    /* Line i bounds line lineNo on the left. */
                    tLeft = Fix64.Max(tLeft, t);
                }

                if (tLeft > tRight)
                {
                    return false;
                }
            }

            if (directionOpt)
            {
                /* Optimize direction. */
                if (FixVector2.Dot(optVelocity, lines[lineNo].direction) > Fix64.Zero)
                {
                    /* Take right extreme. */
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                }
                else
                {
                    /* Take left extreme. */
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
            }
            else
            {
                /* Optimize closest point. */
                Fix64 t = FixVector2.Dot(lines[lineNo].direction, (optVelocity - lines[lineNo].point));

                if (t < tLeft)
                {
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
                else if (t > tRight)
                {
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                }
                else
                {
                    result = lines[lineNo].point + t * lines[lineNo].direction;
                }
            }

            return true;
        }

        private int linearProgram2(IList<Line> lines, Fix64 radius, FixVector2 optVelocity, bool directionOpt, ref FixVector2 result)
        {
            if (directionOpt)
            {
                /*
                 * Optimize direction. Note that the optimization velocity is of
                 * unit length in this case.
                 */
                result = optVelocity * radius;
            }
            else if (FixVector2.SqrMagnitude(optVelocity) > Fix64.Square(radius))
            {
                /* Optimize closest point and outside circle. */
                result = optVelocity.GetNormalized() * radius;
            }
            else
            {
                /* Optimize closest point and inside circle. */
                result = optVelocity;
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                if (FixVector2.Det(lines[i].direction, lines[i].point - result) > Fix64.Zero)
                {
                    /* Result does not satisfy constraint i. Compute new optimal result. */
                    FixVector2 tempResult = result;
                    if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
                    {
                        result = tempResult;

                        return i;
                    }
                }
            }

            return lines.Count;
        }

        private void linearProgram3(IList<Line> lines, int numObstLines, int beginLine, Fix64 radius, ref FixVector2 result)
        {
            Fix64 distance = Fix64.Zero;

            for (int i = beginLine; i < lines.Count; ++i)
            {
                if (FixVector2.Det(lines[i].direction, lines[i].point - result) > distance)
                {
                    /* Result does not satisfy constraint of line i. */
                    IList<Line> projLines = new List<Line>();
                    for (int ii = 0; ii < numObstLines; ++ii)
                    {
                        projLines.Add(lines[ii]);
                    }

                    for (int j = numObstLines; j < i; ++j)
                    {
                        Line line;

                        Fix64 determinant = FixVector2.Det(lines[i].direction, lines[j].direction);

                        if (Fix64.Abs(determinant) <= Fix64.RVO_EPSILON)
                        {
                            /* Line i and line j are parallel. */
                            if (FixVector2.Dot(lines[i].direction, lines[j].direction) > Fix64.Zero)
                            {
                                /* Line i and line j point in the same direction. */
                                continue;
                            }
                            else
                            {
                                /* Line i and line j point in opposite direction. */
                                line.point = (Fix64)0.5 * (lines[i].point + lines[j].point);
                            }
                        }
                        else
                        {
                            line.point = lines[i].point + (FixVector2.Det(lines[j].direction, lines[i].point - lines[j].point) / determinant) * lines[i].direction;
                        }

                        line.direction = (lines[j].direction - lines[i].direction).GetNormalized();
                        projLines.Add(line);
                    }

                    FixVector2 tempResult = result;
                    if (linearProgram2(projLines, radius, new FixVector2(-lines[i].direction.y, lines[i].direction.x), true, ref result) < projLines.Count)
                    {
                        /*
                         * This should in principle not happen. The result is by
                         * definition already in the feasible region of this
                         * linear program. If it fails, it is due to small
                         * floating point error, and the current result is kept.
                         */
                        result = tempResult;
                    }

                    distance = FixVector2.Det(lines[i].direction, lines[i].point - result);
                }
            }
        }
    }
}
