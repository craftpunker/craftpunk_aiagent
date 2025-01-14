
using System.Collections.Generic;


namespace Battle
{
    public class Simulator : Singleton<Simulator>
    {
        private int id = 0;

        //private class Worker
        //{
        //    private ManualResetEvent doneEvent_;
        //    private volatile int end_;
        //    private volatile int start_;

        //    internal Worker(int start, int end, ManualResetEvent doneEvent)
        //    {
        //        start_ = start;
        //        end_ = end;
        //        doneEvent_ = doneEvent;
        //    }

        //    internal void config(int start, int end)
        //    {
        //        start_ = start;
        //        end_ = end;
        //    }

        //    internal void step(object obj)
        //    {
        //        for (int agentNo = start_; agentNo < end_; ++agentNo)
        //        {
        //            Simulator.instance.agents_[agentNo].computeNeighbors();
        //            Simulator.instance.agents_[agentNo].computeNewVelocity();
        //        }

        //        doneEvent_.Set();
        //    }

        //    internal void update(object obj)
        //    {
        //        for (int agentNo = start_; agentNo < end_; ++agentNo)
        //        {
        //            Simulator.instance.agents_[agentNo].update();
        //        }

        //        doneEvent_.Set();
        //    }
        //}

        internal IList<Agent> agents_;
        internal IList<ObstaclePolygon> obstaclePolygons_;
        internal IList<Obstacle> obstacles_;
        internal bool obstacleChange;
        //internal KdTree kdTree_;
        internal Fix64 timeStep_;

        //private static Simulator instance_ = new Simulator();

        private Agent defaultAgent_;
        //private ManualResetEvent[] doneEvents_;
        //private Worker[] workers_;
        private int numWorkers_;
        //private int workerAgentCount_;
        private Fix64 globalTime_;

        public KdTree kdTree_ { get; set; }
        //public SoldierFlagKDTree soldierFlagkdTree_ { get; set; }

        public Simulator()
        {
            agents_ = new List<Agent>();
            defaultAgent_ = null;
            kdTree_ = new KdTree();
            obstacles_ = new List<Obstacle>();
            obstaclePolygons_ = new List<ObstaclePolygon>();
            globalTime_ = Fix64.Zero;
            timeStep_ = (Fix64)0.1;
            obstacleChange = false;
            //soldierFlagkdTree_ = new SoldierFlagKDTree();
            //SetNumWorkers(0);
        }

        public void Release()
        {
            foreach (var item in agents_)
            {
                item.Release();
            }

            defaultAgent_ = null;
            agents_.Clear();
            obstaclePolygons_.Clear();
            obstacles_.Clear();
            obstacleChange = false;
            globalTime_ = Fix64.Zero;
            id = 0;
            kdTree_.Release();
            //soldierFlagkdTree_.Release();
        }

        public Agent addAgent(FixVector2 position, EntityBase entity)
        {
            if (defaultAgent_ == null)
            {
                return null;
            }

            Agent agent = ClassPool.instance.Pop<Agent>();
            agent.id_ = id++;
            agent.maxNeighbors_ = defaultAgent_.maxNeighbors_;
            agent.maxSpeed_ = defaultAgent_.maxSpeed_;
            agent.neighborDist_ = defaultAgent_.neighborDist_;
            agent.position_ = position;
            agent.radius_ = defaultAgent_.radius_;
            agent.timeHorizon_ = defaultAgent_.timeHorizon_;
            agent.timeHorizonObst_ = defaultAgent_.timeHorizonObst_;
            agent.velocity_ = defaultAgent_.velocity_;
            agent.entity = entity;
            agent.orcaType = OrcaType.CloseOrca;
            //agent.isSearchEnemy = false;
            agents_.Add(agent);

            OnAddAgent();

            return agent;
        }

        public Agent addAgent(FixVector2 position, Fix64 neighborDist, int maxNeighbors, Fix64 timeHorizon, Fix64 timeHorizonObst, Fix64 radius, Fix64 maxSpeed, FixVector2 velocity, EntityBase entity)
        {
            Agent agent = ClassPool.instance.Pop<Agent>();
            agent.id_ = id++;
            agent.maxNeighbors_ = maxNeighbors;
            agent.maxSpeed_ = maxSpeed;
            agent.neighborDist_ = neighborDist;
            agent.position_ = position;
            agent.radius_ = radius;
            agent.timeHorizon_ = timeHorizon;
            agent.timeHorizonObst_ = timeHorizonObst;
            agent.velocity_ = velocity;
            agent.entity = entity;
            agent.orcaType = OrcaType.CloseOrca;
            agent.needDelete_ = false;
            //agent.isSearchEnemy = false;
            agents_.Add(agent);

            //OnAddAgent();

            return agent;
        }

        public void delAgent(int agentNo)
        {
            agents_[agentNo].needDelete_ = true;
        }

        public void updateDeleteAgent()
        {
            bool isDelete = false;
            for (int i = agents_.Count - 1; i >= 0; i--)
            {
                if (agents_[i].needDelete_)
                {
                    agents_.RemoveAt(i);
                    isDelete = true;
                }
            }

            if (isDelete)
            {
                OnDelAgent();
                id = agents_.Count;
            }
        }

        public ObstaclePolygon addObstacle(IList<FixVector2> vertices)
        {
            if (vertices.Count < 2)
            {
                return null;
            }

            ObstaclePolygon polygon = new ObstaclePolygon();

            for (int i = 0; i < vertices.Count; i++)
            {
                polygon._vertices.Add(vertices[i]);
            }

            polygon._id = obstaclePolygons_.Count;
            obstaclePolygons_.Add(polygon);

            onAddObstacle();
            return polygon;
        }

        public void delObstacle(int obstacleNo)
        {
            obstaclePolygons_[obstacleNo]._isNeedDelete = true;
        }

        internal void onAddObstacle()
        {
            obstacleChange = true;
        }

        internal void onDelObstacle()
        {
            obstacleChange = true;
        }

        internal void updateDelObstacle()
        {
            bool isDelete = false;

            for (int i = obstaclePolygons_.Count - 1; i >= 0; i--)
            {
                if (obstaclePolygons_[i]._isNeedDelete)
                {
                    // IList<Obstacle> obstaclesPostion = obstaclePolygons_[i]._obstacles;
                    // for (int j = obstaclesPostion.Count - 1; j >= 0; j--)
                    // {
                    //     obstacles_.RemoveAt(j);
                    // }
                    obstaclePolygons_.RemoveAt(i);
                    isDelete = true;
                }
            }

            if (isDelete)
            {
                onDelObstacle();
            }
        }

        internal void reBuildObstacle()
        {
            if (obstacleChange)
            {
                obstacles_.Clear();

                for (int polygonIndex = 0; polygonIndex < obstaclePolygons_.Count; polygonIndex++)
                {
                    obstaclePolygons_[polygonIndex]._id = polygonIndex;

                    int obstacleNo = obstacles_.Count;

                    IList<FixVector2> vertices = obstaclePolygons_[polygonIndex]._vertices;
                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        Obstacle obstacle = new Obstacle();
                        obstacle.point_ = vertices[i];

                        if (i != 0)
                        {
                            //之前的
                            obstacle.previous_ = obstacles_[obstacles_.Count - 1];
                            obstacle.previous_.next_ = obstacle;
                        }

                        if (i == vertices.Count - 1)
                        {
                            //下一个
                            obstacle.next_ = obstacles_[obstacleNo];
                            obstacle.next_.previous_ = obstacle;
                        }

                        obstacle.direction_ = (vertices[(i == vertices.Count - 1 ? 0 : i + 1)] - vertices[i]).GetNormalized();

                        if (vertices.Count == 2)
                        {
                            obstacle.convex_ = true;
                        }
                        else
                        {
                            obstacle.convex_ = (FixMath.LeftOf(vertices[(i == 0 ? vertices.Count - 1 : i - 1)], vertices[i], vertices[(i == vertices.Count - 1 ? 0 : i + 1)]) >= Fix64.Zero);
                        }

                        obstacle.id_ = obstacles_.Count;
                        obstacles_.Add(obstacle);
                    }
                }

                obstacleChange = false;

                processObstacles();
            }
        }

        internal void updateObstacle()
        {
            updateDelObstacle();

            reBuildObstacle();
        }

        /**
         * <summary>Clears the simulation.</summary>
         */
        //public void Clear()
        //{
        //    agents_ = new List<Agent>();
        //    defaultAgent_ = null;
        //    kdTree_ = new KdTree();
        //    obstacles_ = new List<Obstacle>();
        //    obstaclePolygons_ = new List<ObstaclePolygon>();
        //    globalTime_ = Fix64.Zero;
        //    timeStep_ = (Fix64)0.1;
        //    obstacleChange = false;

        //    SetNumWorkers(0);
        //}

        public Fix64 doStep()
        {
            updateObstacle();
            updateDeleteAgent();

            //if (workers_ == null)
            //{
            //    workers_ = new Worker[numWorkers_];
            //    doneEvents_ = new ManualResetEvent[workers_.Length];
            //    workerAgentCount_ = getNumAgents();

            //    for (int block = 0; block < workers_.Length; ++block)
            //    {
            //        doneEvents_[block] = new ManualResetEvent(false);
            //        workers_[block] = new Worker(block * getNumAgents() / workers_.Length, (block + 1) * getNumAgents() / workers_.Length, doneEvents_[block]);
            //    }
            //}

            //if (workerAgentCount_ != getNumAgents())
            //{
            //    workerAgentCount_ = getNumAgents();
            //    for (int block = 0; block < workers_.Length; ++block)
            //    {
            //        workers_[block].config(block * getNumAgents() / workers_.Length, (block + 1) * getNumAgents() / workers_.Length);
            //    }
            //}

            kdTree_.buildAgentTree();
            //soldierFlagkdTree_.buildAgentTree();
            //for (int block = 0; block < workers_.Length; ++block)
            //{
            //    doneEvents_[block].Reset();
            //    //ThreadPool.QueueUserWorkItem(workers_[block].step);
            //    workers_[block].step(null);
            //}

            ////WaitHandle.WaitAll(doneEvents_);

            //for (int block = 0; block < workers_.Length; ++block)
            //{
            //    doneEvents_[block].Reset();
            //    //ThreadPool.QueueUserWorkItem(workers_[block].update);
            //    workers_[block].update(null);
            //}
            //WaitHandle.WaitAll(doneEvents_);

            int count = agents_.Count;
            for (int i = 0; i < count; i++)
            {
                //if (!agents_[i].isORCA)
                //    continue;

                agents_[i].computeNeighbors();
                agents_[i].computeNewVelocity();
                agents_[i].update();
            }

            //for (int i = 0; i < count; i++)
            //{
            //    agents_[i].update();
            //}

            globalTime_ += timeStep_;

            return globalTime_;
        }

        //public Fix64 doStep()
        //{
        //    updateObstacle();
        //    updateDeleteAgent();

        //    if (workers_ == null)
        //    {
        //        workers_ = new Worker[numWorkers_];
        //        doneEvents_ = new ManualResetEvent[workers_.Length];
        //        workerAgentCount_ = getNumAgents();

        //        for (int block = 0; block < workers_.Length; ++block)
        //        {
        //            doneEvents_[block] = new ManualResetEvent(false);
        //            workers_[block] = new Worker(block * getNumAgents() / workers_.Length, (block + 1) * getNumAgents() / workers_.Length, doneEvents_[block]);
        //        }
        //    }

        //    if (workerAgentCount_ != getNumAgents())
        //    {
        //        workerAgentCount_ = getNumAgents();
        //        for (int block = 0; block < workers_.Length; ++block)
        //        {
        //            workers_[block].config(block * getNumAgents() / workers_.Length, (block + 1) * getNumAgents() / workers_.Length);
        //        }
        //    }

        //    kdTree_.buildAgentTree();

        //    for (int block = 0; block < workers_.Length; ++block)
        //    {
        //        doneEvents_[block].Reset();
        //        //ThreadPool.QueueUserWorkItem(workers_[block].step);
        //        workers_[block].step(null);
        //    }

        //    //WaitHandle.WaitAll(doneEvents_);

        //    for (int block = 0; block < workers_.Length; ++block)
        //    {
        //        doneEvents_[block].Reset();
        //        //ThreadPool.QueueUserWorkItem(workers_[block].update);
        //        workers_[block].update(null);
        //    }
        //    //WaitHandle.WaitAll(doneEvents_);

        //    globalTime_ += timeStep_;

        //    return globalTime_;
        //}

        public int getAgentAgentNeighbor(int agentNo, int neighborNo)
        {
            return agents_[agentNo].agentNeighbors_[neighborNo].Value.id_;
        }

        public int getAgentMaxNeighbors(int agentNo)
        {
            return agents_[agentNo].maxNeighbors_;
        }

        public Fix64 getAgentMaxSpeed(int agentNo)
        {
            return agents_[agentNo].maxSpeed_;
        }

        public Fix64 getAgentNeighborDist(int agentNo)
        {
            return agents_[agentNo].neighborDist_;
        }

        public int getAgentNumAgentNeighbors(int agentNo)
        {
            return agents_[agentNo].agentNeighbors_.Count;
        }

        public int getAgentNumObstacleNeighbors(int agentNo)
        {
            return agents_[agentNo].obstacleNeighbors_.Count;
        }

        public int getAgentObstacleNeighbor(int agentNo, int neighborNo)
        {
            return agents_[agentNo].obstacleNeighbors_[neighborNo].Value.id_;
        }

        public IList<Line> getAgentOrcaLines(int agentNo)
        {
            return agents_[agentNo].orcaLines_;
        }

        public FixVector2 getAgentPosition(int agentNo)
        {
            return agents_[agentNo].position_;
        }

        public FixVector3 getAgentPositionV3(int agentNo)
        {
            return agents_[agentNo].position_.ToFixVector3();
        }

        public FixVector2 getAgentPrefVelocity(int agentNo)
        {
            return agents_[agentNo].prefVelocity_;
        }

        public Fix64 getAgentRadius(int agentNo)
        {
            return agents_[agentNo].radius_;
        }

        public Fix64 getAgentTimeHorizon(int agentNo)
        {
            return agents_[agentNo].timeHorizon_;
        }

        public Fix64 getAgentTimeHorizonObst(int agentNo)
        {
            return agents_[agentNo].timeHorizonObst_;
        }

        public FixVector2 getAgentVelocity(int agentNo)
        {
            return agents_[agentNo].velocity_;
        }

        public Fix64 getGlobalTime()
        {
            return globalTime_;
        }

        public int getNumAgents()
        {
            return agents_.Count;
        }

        public int getObstacleCount()
        {
            return obstaclePolygons_.Count;
        }

        public int getNumObstacleVertices()
        {
            return obstacles_.Count;
        }

        public int GetNumWorkers()
        {
            return numWorkers_;
        }

        public FixVector2 getObstacleVertex(int vertexNo)
        {
            return obstacles_[vertexNo].point_;
        }

        public int getNextObstacleVertexNo(int vertexNo)
        {
            return obstacles_[vertexNo].next_.id_;
        }

        public int getPrevObstacleVertexNo(int vertexNo)
        {
            return obstacles_[vertexNo].previous_.id_;
        }

        public Fix64 getTimeStep()
        {
            return timeStep_;
        }

        internal void processObstacles()
        {
            kdTree_.buildObstacleTree();
        }

        public bool queryVisibility(FixVector2 point1, FixVector2 point2, Fix64 radius)
        {
            return kdTree_.queryVisibility(point1, point2, radius);
        }

        public void setAgentDefaults(Fix64 neighborDist, int maxNeighbors, Fix64 timeHorizon, Fix64 timeHorizonObst, Fix64 radius, Fix64 maxSpeed, FixVector2 velocity)
        {
            if (defaultAgent_ == null)
            {
                defaultAgent_ = ClassPool.instance.Pop<Agent>();
            }

            defaultAgent_.maxNeighbors_ = maxNeighbors;
            defaultAgent_.maxSpeed_ = maxSpeed;
            defaultAgent_.neighborDist_ = neighborDist;
            defaultAgent_.radius_ = radius;
            defaultAgent_.timeHorizon_ = timeHorizon;
            defaultAgent_.timeHorizonObst_ = timeHorizonObst;
            defaultAgent_.velocity_ = velocity;
        }

        public void setAgentMaxNeighbors(int agentNo, int maxNeighbors)
        {
            agents_[agentNo].maxNeighbors_ = maxNeighbors;
        }

        public void setAgentMaxSpeed(int agentNo, Fix64 maxSpeed)
        {
            agents_[agentNo].maxSpeed_ = maxSpeed;
        }

        public void setAgentNeighborDist(int agentNo, Fix64 neighborDist)
        {
            agents_[agentNo].neighborDist_ = neighborDist;
        }

        public void setAgentPosition(int agentNo, FixVector2 position)
        {
            agents_[agentNo].position_ = position;
        }

        public void setAgentPosition(int agentNo, FixVector3 position)
        {
            agents_[agentNo].position_ = new FixVector2(position.x, position.y);
        }

        public void setAgentPrefVelocity(int agentNo, FixVector2 prefVelocity)
        {
            agents_[agentNo].prefVelocity_ = prefVelocity;
        }

        public void setAgentRadius(int agentNo, Fix64 radius)
        {
            agents_[agentNo].radius_ = radius;
        }

        public void setAgentTimeHorizon(int agentNo, Fix64 timeHorizon)
        {
            agents_[agentNo].timeHorizon_ = timeHorizon;
        }

        public void setAgentTimeHorizonObst(int agentNo, Fix64 timeHorizonObst)
        {
            agents_[agentNo].timeHorizonObst_ = timeHorizonObst;
        }

        public void setAgentVelocity(int agentNo, FixVector2 velocity)
        {
            agents_[agentNo].velocity_ = velocity;
        }

        public void setGlobalTime(Fix64 globalTime)
        {
            globalTime_ = globalTime;
        }

        //public void SetNumWorkers(int numWorkers)
        //{
        //    numWorkers_ = numWorkers;

        //    if (numWorkers_ <= 0)
        //    {
        //        int completionPorts;
        //        ThreadPool.GetMinThreads(out numWorkers_, out completionPorts);
        //    }
        //    workers_ = null;
        //    workerAgentCount_ = 0;
        //}

        public void setTimeStep(Fix64 timeStep)
        {
            timeStep_ = timeStep;
        }

        //private Simulator()
        //{
        //    Clear();
        //}

        public bool isNeedDelete(int agentNo)
        {
            return agents_[agentNo].needDelete_;
        }

        internal void OnAddAgent()
        {

        }

        internal void OnDelAgent()
        {
            for (int i = 0; i < agents_.Count; i++)
            {
                agents_[i].id_ = i;
            }
        }
    }
}
