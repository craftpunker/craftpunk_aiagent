//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Random = System.Random;
//using Battle;

//public class World : MonoBehaviour
//{
//    /***
//     * addObstacle ，
//     * setTimeStep  0.25f
//     * setAgentDefaults 
//     * processObstacles 
//     * setAgentPrefVelocity 
//     *
//     */

//    private const int guiWidth = 150;
//    private const int guiHeight = 50;
    
//    [HideInInspector]
//    public List<Agent> itemAgents;
//    //[HideInInspector]
//    //public List<ItemAgent> EnemyitemAgents;

//    SRandom sRandom = new SRandom(123);


//    //[HideInInspector]
//    //public Collider2D mapCollider;

//    public GameObject itemAgentGo;
//    public GameObject itemAgentGo1;

//    //[HideInInspector]
//    //public Transform map;

//    [HideInInspector]
//    public Transform itemRoot;

//    /// <summary>
//    /// 
//    /// </summary>
//    [HideInInspector] public Transform obstacleRoot;
    
//    [HideInInspector]
//    private int agentCnt = 3000;
    
//    public Random random;
    
//    /// <summary>
//    /// 
//    /// </summary>
//    private FixVector3 starPos1 = new FixVector3(-(Fix64)10, Fix64.Zero, Fix64.Zero);
//    private FixVector3 starPos2 = new FixVector3((Fix64)10, Fix64.Zero, Fix64.Zero);

//    //[NonSerialized]
//    //public List<ObstacleAgent> obstacleAgents = new List<ObstacleAgent>();

//    public void Start()
//    {
//        random = new Random();
        
//        //this.map = transform.Find("Map");
//       // mapCollider = map.GetComponent<Collider2D>();

//        this.obstacleRoot = transform.Find("obstacle");

//        //for (int i = 0; i < this.obstacleRoot.childCount; i++)
//        //{
//        //    var child = this.obstacleRoot.GetChild(i);
//        //    SpriteRenderer spriteRen = child.GetComponent<SpriteRenderer>();

//        //    ObstacleAgent obstacleAgent = new ObstacleAgent();
            
//        //    IList<RVO.Vector2> obstacle = new List<RVO.Vector2>();
//        //    obstacle.Add(new RVO.Vector2(spriteRen.bounds.max.x,spriteRen.bounds.max.y));
//        //    obstacle.Add(new RVO.Vector2(spriteRen.bounds.min.x,spriteRen.bounds.max.y));
//        //    obstacle.Add(new RVO.Vector2(spriteRen.bounds.min.x,spriteRen.bounds.min.y));
//        //    obstacle.Add(new RVO.Vector2(spriteRen.bounds.max.x,spriteRen.bounds.min.y));

//        //    obstacleAgent.polygon = Simulator.Instance.addObstacle(obstacle);
//        //    obstacleAgent.tf = child;
            
//        //    obstacleAgents.Add(obstacleAgent);
//        //    //Debug.Log("max:" + spriteRen.bounds.max + ",min:" + spriteRen.bounds.min);
//        //}

//        itemRoot = new GameObject("itemRoot").transform;
//        itemRoot.SetParent(transform);

//        int col = 20;
//        itemAgents = new List<Agent>();// ItemAgent[agentCnt];
//        Simulator.instance.setAgentDefaults((Fix64)15, 3, (Fix64)10.0, (Fix64)10.0, (Fix64)0.125, Fix64.One, FixVector2.Zero);
//        for (int i = 0; i < agentCnt; i++)
//        {
//            FixVector3 startPos = i % 2 == 0 ? starPos1 : starPos2;
//            itemAgents.Add(CreateAgent(new FixVector2(startPos.x + (Fix64)0.2 * (i % col), startPos.y - (Fix64)0.2 * (i / col)), i % 2));
//        }

//        //EnemyitemAgents = new List<ItemAgent>();// ItemAgent[agentCnt];
//        //Simulator.Instance.setAgentDefaults(15f, 10, 10.0f, 10.0f, 0.125f, 0.5f, new RVO.Vector2(0.0f, 0.0f));
//        //for (int i = 0; i < agentCnt; i++)
//        //{
//        //    EnemyitemAgents.Add(CreateImemAgent(new Vector2(starPos2.x + 0.2f * (i % col), starPos2.y - 0.2f * (i / col)), 1));
//        //}

//        //Simulator.Instance.processObstacles();
//    }

//    private Agent CreateAgent(FixVector2 pos, int group)
//    {
//        Agent agent = Simulator.instance.addAgent(new FixVector2(pos.x,pos.y));
//        GameObject go = Instantiate(group == 0 ? itemAgentGo : itemAgentGo1, itemRoot);
//        go.transform.position = new FixVector3(pos.x, pos.y, Fix64.Zero).ToUnityVector3();
//        return agent;
//    }

//    private void OnGUI()
//    {

//        GUI.skin.textField.fontSize = 30;
//        GUILayout.TextField($"{agentCnt}");
//        GUILayout.TextField($"{agentCnt}");

//        //if (GUILayout.Button("",GUILayout.Width(guiWidth),GUILayout.Height(guiHeight)))
//        //{
//        //    if (itemAgents.Count > 0)
//        //    {
//        //        int id = random.Next(0, itemAgents.Count);
//        //        if (id != itemAgents[id].agent.id_)
//        //        {
//        //            Debug.LogError("id !");
//        //        }

//        //        Object.Destroy(itemAgents[id].gameObject);
//        //        itemAgents.RemoveAt(id);
//        //        Simulator.Instance.delAgent(id);
//        //    }
//        //}

//        //if (GUILayout.Button("",GUILayout.Width(guiWidth),GUILayout.Height(guiHeight)))
//        //{
//        //    itemAgents.Add(CreateImemAgent(new Vector2(starPos.x, starPos.y)));
//        //}

//        //if (GUILayout.Button("",GUILayout.Width(guiWidth),GUILayout.Height(guiHeight)))
//        //{
//        //    if (itemAgents.Count > 0)
//        //    {
//        //        Simulator.Instance.setAgentPosition(itemAgents[0].agent.id_, new RVO.Vector2(starPos.x,starPos.y));
//        //    }
//        //}

//        //if (GUILayout.Button("",GUILayout.Width(guiWidth),GUILayout.Height(guiHeight)))
//        //{
//        //    int randomIndex = random.Next(0, obstacleAgents.Count);

//        //    if (obstacleAgents[randomIndex].polygon != null)
//        //    {
//        //        Simulator.Instance.delObstacle(obstacleAgents[randomIndex].polygon._id);
//        //        obstacleAgents[randomIndex].polygon = null;
//        //        obstacleAgents[randomIndex].tf.gameObject.SetActive(false);
//        //    }
//        //}

//        //if (GUILayout.Button("",GUILayout.Width(guiWidth),GUILayout.Height(guiHeight)))
//        //{
//        //    int randomIndex = random.Next(0, obstacleAgents.Count);

//        //    if (!obstacleAgents[randomIndex].tf.gameObject.activeSelf)
//        //    {
//        //        SpriteRenderer spriteRen = obstacleAgents[randomIndex].tf.GetComponent<SpriteRenderer>();

//        //        ObstacleAgent obstacleAgent = obstacleAgents[randomIndex];

//        //        IList<RVO.Vector2> obstacle = new List<RVO.Vector2>();
//        //        obstacle.Add(new RVO.Vector2(spriteRen.bounds.max.x,spriteRen.bounds.max.y));
//        //        obstacle.Add(new RVO.Vector2(spriteRen.bounds.min.x,spriteRen.bounds.max.y));
//        //        obstacle.Add(new RVO.Vector2(spriteRen.bounds.min.x,spriteRen.bounds.min.y));
//        //        obstacle.Add(new RVO.Vector2(spriteRen.bounds.max.x,spriteRen.bounds.min.y));

//        //        obstacleAgent.polygon = Simulator.Instance.addObstacle(obstacle);

//        //        obstacleAgents[randomIndex].tf.gameObject.SetActive(true);
//        //    }
//        //}
//    }

//    private void Update()
//    {
//        //if (Input.GetMouseButton(0))
//        //{
//        //    Simulator.Instance.setTimeStep(Time.deltaTime);
//        //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        //    RVO.Vector2 target = new RVO.Vector2(worldPos.x, worldPos.y);

//        //    for (int i = 0; i < itemAgents.Count; i++)
//        //    {
//        //        int id = itemAgents[i].agent.id_;
//        //        if (Simulator.Instance.isNeedDelete(id))
//        //        {
//        //            continue;
//        //        }
//        //        var goalVector = target - Simulator.Instance.getAgentPosition(id);
//        //        if (RVOMath.absSq(goalVector) > 0.01f)
//        //        {
//        //            goalVector = RVOMath.normalize(goalVector) * 0.5f;
//        //            Simulator.Instance.setAgentPrefVelocity(id, goalVector);

//        //            /* Perturb a little to avoid deadlocks due to perfect symmetry. */
//        //            float angle = (float) random.NextDouble()*2.0f*(float) Math.PI;
//        //            float dist = (float) random.NextDouble()*0.0001f;

//        //            Simulator.Instance.setAgentPrefVelocity(id, Simulator.Instance.getAgentPrefVelocity(id) +
//        //                                                        dist*
//        //                                                        new RVO.Vector2((float) Math.Cos(angle), (float) Math.Sin(angle)));
//        //        }
//        //        else
//        //        {
//        //            Simulator.Instance.setAgentPrefVelocity(id, new RVO.Vector2(0,0));
//        //        }
//        //    }

//        //    Simulator.Instance.doStep();

//        //    for (int i = 0; i < itemAgents.Count; i++)
//        //    {
//        //        var pos = Simulator.Instance.getAgentPosition(i);
//        //        itemAgents[i].transform.position = new Vector3(pos.x(), pos.y(), itemAgents[i].transform.position.z);

//        //        if (i != itemAgents[i].agent.id_)
//        //        {
//        //            Debug.LogError("id !");
//        //        }
//        //    }
//        //    //float dis = Vector3.Distance(itemAgents[0].transform.position, itemAgents[1].transform.position);
//        //    //Debug.Log(":" + dis);
//        //}
//        Move(itemAgents);
//        //Move(EnemyitemAgents, starPos1);

//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            for (int i = 0; i < itemAgents.Count; i++)
//            {
//                var agent = itemAgents[i];

//                if (i % 2 == 0)
//                {
//                    agent.isMove = false;
//                }
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            for (int i = 0; i < itemAgents.Count; i++)
//            {
//                var agent = itemAgents[i];

//                if (i % 2 == 0)
//                {
//                    agent.isMove = true;
//                }
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            for (int i = 0; i < itemAgents.Count; i++)
//            {
//                var agent = itemAgents[i];

//                if (i % 2 == 1)
//                {
//                    agent.isMove = false;
//                }
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.F))
//        {
//            for (int i = 0; i < itemAgents.Count; i++)
//            {
//                var agent = itemAgents[i];

//                if (i % 2 == 1)
//                {
//                    agent.isMove = true;
//                }
//            }
//        }

//    }

//    private void Move(List<Agent> itemAgents)
//    {
//        Simulator.instance.setTimeStep((Fix64)Time.deltaTime);
//        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        //RVO.Vector2 target = new RVO.Vector2(targetPos.x, targetPos.y);

//        for (int i = 0; i < itemAgents.Count; i++)
//        {
//            var agent = itemAgents[i];  

//            int id = agent.id_;
//            if (Simulator.instance.isNeedDelete(id))
//            {
//                continue;
//            }

//            FixVector3 targetPos = i % 2 == 0 ? starPos2 : starPos1;
//            FixVector2 target = new FixVector2(targetPos.x, targetPos.y);

//            var goalVector = target - Simulator.instance.getAgentPosition(id);
//            if (agent.isMove && FixVector2.SqrMagnitude(goalVector) > (Fix64)0.01)
//            {
//                goalVector = (goalVector).GetNormalized() * (Fix64)0.5;
//                Simulator.instance.setAgentPrefVelocity(id, goalVector);

//                /* Perturb a little to avoid deadlocks due to perfect symmetry. */
//                Fix64 angle = sRandom.Next() * (Fix64)2.0 * Fix64.PI;
//                Fix64 dist = sRandom.Next() * (Fix64)0.0001;

//                Simulator.instance.setAgentPrefVelocity(id, Simulator.instance.getAgentPrefVelocity(id) +
//                                                            dist *
//                                                            new FixVector2(Fix64.Cos(angle), Fix64.Sin(angle)));
//            }
//            else
//            {
//                //new FixVector2(0, 0)
//                Simulator.instance.setAgentPrefVelocity(id, new FixVector2(0, 0));
//            }
//        }

//        Simulator.instance.doStep();

//        for (int i = 0; i < itemAgents.Count; i++)
//        {
//            var pos = Simulator.instance.getAgentPosition(i);
//            //itemAgents[i].transform.position = new FixVector3(pos.x, pos.y, (Fix64)itemAgents[i].transform.position.z).ToUnityVector3();

//            //if (i != itemAgents[i].agent.id_)
//            //{
//            //    Debug.LogError("id !");
//            //}
//        }
//        //float dis = Vector3.Distance(itemAgents[0].transform.position, itemAgents[1].transform.position);
//        //Debug.Log(":" + dis);
//    }
//}
