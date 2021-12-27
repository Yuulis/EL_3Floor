using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using TMPro;

public class AgentControl_3Floor_Lv4_2 : Agent
{
    // General
    private bool onGround;
    public GameObject Floor3;
    Material Floor3Material;
    Renderer Floor3Renderer;
    Floor3_Settings Floor3_Settings;

    float timer = 0f;

    // Agent
    Rigidbody rBody;
    public int nowFloor;
    public float AgentSpeed = 0.6f;

    // Water
    public Transform Water;

    // Obstacle
    public Transform StairObstacle1;
    public Transform StairObstacle2;
    public Transform StairObstacle3;

    private List<bool> StairObstaclesFlag;

    // Stairs
    public Transform Stair1_1;
    public Transform Stair1_2;
    public Transform Stair1_3;
    public Transform Stair1_4;
    public Transform Stair2_1;
    public Transform Stair2_2;
    public Transform Stair2_3;
    public Transform Stair2_4;

    private int ClosestStair_1F;
    private int ClosestStair_2F;
    private int ReachedStair_1F;
    private int ReachedStair_2F;

    private List<float> StairsDistance;

    // For results
    public bool ShowResults;
    public bool DataExport;

    [HideInInspector]
    public GameObject DataCounter;
    DataCounter DataCounterScript;

    // At Initializing
    public override void Initialize()
    {
        Floor3_Settings = FindObjectOfType<Floor3_Settings>();

        this.rBody = GetComponent<Rigidbody>();
        Floor3Renderer = Floor3.GetComponent<Renderer>();
        Floor3Material = Floor3Renderer.material;

        if (ShowResults) {
            DataCounter = GameObject.Find("DataCounter");
            DataCounterScript = DataCounter.GetComponent<DataCounter>();

            DataCounterScript.EpisodeCounter = 0;
            DataCounterScript.SuccessCounter = 0;
            DataCounterScript.Reach1FloorCounter = 0;
            DataCounterScript.Reach2FloorCounter = 0;
            DataCounterScript.Reach3FloorCounter = 0;
            DataCounterScript.SuccessAvarageWaterHeightCounter = 0f;
            DataCounterScript.FailAvarageWaterHeightCounter = 0f;

            Time.timeScale = 30;
        }
    }

    // Agent Spawn
    public void SpawnAgent()
    {
        this.transform.localPosition = new Vector3(Random.Range(-19.5f, 19.5f), 4.75f, Random.Range(-14.5f, 14.5f));
        CheckClosestStair();
    }

    // Last Floor check
    public void LastFloorCheck()
    {
        if (nowFloor == 1) DataCounterScript.Reach1FloorCounter++;
        if (nowFloor == 2) DataCounterScript.Reach2FloorCounter++;
        if (nowFloor == 3) DataCounterScript.Reach3FloorCounter++;
    }

    public void CheckClosestStair()
    {
        if (nowFloor == 1)
        {
            float DisToStair1 = Vector3.Distance(this.transform.localPosition, Stair1_1.localPosition);
            float DisToStair2 = Vector3.Distance(this.transform.localPosition, Stair1_2.localPosition);
            float DisToStair3 = Vector3.Distance(this.transform.localPosition, Stair1_3.localPosition);
            float DisToStair4 = Vector3.Distance(this.transform.localPosition, Stair1_4.localPosition);

            for (int i = 1; i <= 4; i++)
            {
                if (!StairObstaclesFlag[i])
                {
                    if (i == 1) StairsDistance[i] = DisToStair1;
                    if (i == 2) StairsDistance[i] = DisToStair2;
                    if (i == 3) StairsDistance[i] = DisToStair3;
                    if (i == 4) StairsDistance[i] = DisToStair4;
                }
            }
            StairsDistance.Sort();

            if (StairsDistance[0] == DisToStair1) ClosestStair_1F = 1;
            else if (StairsDistance[0] == DisToStair2) ClosestStair_1F = 2;
            else if (StairsDistance[0] == DisToStair3) ClosestStair_1F = 3;
            else if (StairsDistance[0] == DisToStair4) ClosestStair_1F = 4;
        }

        else if (nowFloor == 2)
        {
            float DisToStair1 = Vector3.Distance(this.transform.localPosition, Stair2_1.localPosition);
            float DisToStair2 = Vector3.Distance(this.transform.localPosition, Stair2_2.localPosition);
            float DisToStair3 = Vector3.Distance(this.transform.localPosition, Stair2_3.localPosition);
            float DisToStair4 = Vector3.Distance(this.transform.localPosition, Stair2_4.localPosition);

            for (int i = 5; i <= 8; i++)
            {
                if (!StairObstaclesFlag[i])
                {
                    if (i == 5) StairsDistance[i] = DisToStair1;
                    if (i == 6) StairsDistance[i] = DisToStair2;
                    if (i == 7) StairsDistance[i] = DisToStair3;
                    if (i == 8) StairsDistance[i] = DisToStair4;
                }
            }
            StairsDistance.Sort();

            if (StairsDistance[0] == DisToStair1) ClosestStair_2F = 1;
            else if (StairsDistance[0] == DisToStair2) ClosestStair_2F = 2;
            else if (StairsDistance[0] == DisToStair3) ClosestStair_2F = 3;
            else if (StairsDistance[0] == DisToStair4) ClosestStair_2F = 4;
        }
    }

    public void CheckReachedStair(string name)
    {
        if (nowFloor == 1)
        {
            if (name[9] == '1') ReachedStair_1F = 1;
            else if (name[9] == '2') ReachedStair_1F = 2;
            else if (name[9] == '3') ReachedStair_1F = 3;
            else if (name[9] == '4') ReachedStair_1F = 4;
        }

        else if (nowFloor == 2)
        {
            if (name[9] == '1') ReachedStair_2F = 1;
            else if (name[9] == '2') ReachedStair_2F = 2;
            else if (name[9] == '3') ReachedStair_2F = 3;
            else if (name[9] == '4') ReachedStair_2F = 4;
        }
    }

    public void SpawnStairObstacle()
    {
        StairObstacle1.rotation = Quaternion.identity;
        StairObstacle2.rotation = Quaternion.identity;
        StairObstacle3.rotation = Quaternion.identity;

        int RandomPosNum1 = Random.Range(1, 8 + 1);
        if (RandomPosNum1 == 1) StairObstacle1.localPosition = new Vector3(11f, 4f, 15f);
        if (RandomPosNum1 == 2) { StairObstacle1.localPosition = new Vector3(-11f, 4f, 8f); StairObstacle1.Rotate(0f, 180f, 0f); }
        if (RandomPosNum1 == 3) { StairObstacle1.localPosition = new Vector3(-11f, 4f, -15f); StairObstacle1.Rotate(0f, 180f, 0f); }
        if (RandomPosNum1 == 4) StairObstacle1.localPosition = new Vector3(11f, 4f, -8f);
        if (RandomPosNum1 == 5) StairObstacle1.localPosition = new Vector3(11f, 11f, 15f);
        if (RandomPosNum1 == 6) { StairObstacle1.localPosition = new Vector3(-11f, 11f, 8f); StairObstacle1.Rotate(0f, 180f, 0f); }
        if (RandomPosNum1 == 7) { StairObstacle1.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle1.Rotate(0f, 180f, 0f); }
        if (RandomPosNum1 == 8) StairObstacle1.localPosition = new Vector3(11f, 11f, -8f);

        int RandomPosNum2 = Random.Range(1, 8 + 1);
        while (RandomPosNum2 == RandomPosNum1) RandomPosNum2 = Random.Range(1, 8 + 1);

        if (RandomPosNum2 == 1) StairObstacle2.localPosition = new Vector3(11f, 4f, 15f);
        if (RandomPosNum2 == 2) { StairObstacle2.localPosition = new Vector3(-11f, 4f, 8f); StairObstacle2.Rotate(0f, 180f, 0f); }
        if (RandomPosNum2 == 3) { StairObstacle2.localPosition = new Vector3(-11f, 4f, -15f); StairObstacle2.Rotate(0f, 180f, 0f); }
        if (RandomPosNum2 == 4) StairObstacle2.localPosition = new Vector3(11f, 4f, -8f);
        if (RandomPosNum2 == 5) StairObstacle2.localPosition = new Vector3(11f, 11f, 15f);
        if (RandomPosNum2 == 6) { StairObstacle2.localPosition = new Vector3(-11f, 11f, 8f); StairObstacle2.Rotate(0f, 180f, 0f); }
        if (RandomPosNum2 == 7) { StairObstacle2.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle2.Rotate(0f, 180f, 0f); }
        if (RandomPosNum2 == 8) StairObstacle2.localPosition = new Vector3(11f, 11f, -8f);

        int RandomPosNum3 = Random.Range(1, 8 + 1);
        while (RandomPosNum3 == RandomPosNum1 || RandomPosNum3 == RandomPosNum2) RandomPosNum3 = Random.Range(1, 8 + 1);

        if (RandomPosNum3 == 1) StairObstacle3.localPosition = new Vector3(11f, 4f, 15f);
        if (RandomPosNum3 == 2) { StairObstacle3.localPosition = new Vector3(-11f, 4f, 8f); StairObstacle3.Rotate(0f, 180f, 0f); }
        if (RandomPosNum3 == 3) { StairObstacle3.localPosition = new Vector3(-11f, 4f, -15f); StairObstacle3.Rotate(0f, 180f, 0f); }
        if (RandomPosNum3 == 4) StairObstacle3.localPosition = new Vector3(11f, 4f, -8f);
        if (RandomPosNum3 == 5) StairObstacle3.localPosition = new Vector3(11f, 11f, 15f);
        if (RandomPosNum3 == 6) { StairObstacle3.localPosition = new Vector3(-11f, 11f, 8f); StairObstacle3.Rotate(0f, 180f, 0f); }
        if (RandomPosNum3 == 7) { StairObstacle3.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle3.Rotate(0f, 180f, 0f); }
        if (RandomPosNum3 == 8) StairObstacle3.localPosition = new Vector3(11f, 11f, -8f);

        StairObstaclesFlag[RandomPosNum1] = true;
        StairObstaclesFlag[RandomPosNum2] = true;
        StairObstaclesFlag[RandomPosNum3] = true;
    }

    // When Episode begins
    public override void OnEpisodeBegin()
    {
        onGround = false;

        // Reset Agent's status
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        nowFloor = 1;
        ClosestStair_1F = 0;
        ClosestStair_2F = 0;

        StairsDistance = new List<float>();
        StairObstaclesFlag = new List<bool>();
        for (int i = 0; i < 10; i++)
        {
            StairsDistance.Add(Mathf.Infinity);
            StairObstaclesFlag.Add(false);
        }

        SpawnAgent();
        SpawnStairObstacle();

        Water.localPosition = new Vector3(-25.0f, -10.0f, -25.0f);

        if (ShowResults)
        {
            DataCounterScript.TextSet();
        }
    }

    // Get observations (size = 4)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);  // Agent(x, y, z)
        sensor.AddObservation(Water.localPosition.y);  // Water(y)
    }

    // When Agent moves (size = 3)
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var rotateDirAction = act[1];

        if (dirToGoForwardAction == 1) dirToGo = 1.0f * this.transform.forward;  // Forward
        else if (dirToGoForwardAction == 2) dirToGo = -1.0f * this.transform.forward;  // Back

        if (rotateDirAction == 1) rotateDir = 1.0f * this.transform.up;  // Rotate right
        else if (rotateDirAction == 2) rotateDir = -1.0f * this.transform.up;  // Rotate left

        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);
        this.rBody.AddForce(dirToGo * AgentSpeed, ForceMode.VelocityChange);
    }

    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        Floor3Renderer.material = mat;
        yield return new WaitForSeconds(time);
        Floor3Renderer.material = Floor3Material;
    }

    // Agent's action
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (onGround)
        {
            MoveAgent(actions.DiscreteActions);
            if (Water.localPosition.y < 14.0f) Water.Translate(0f, 0.01f, 0f);

            // When Water comes
            if (this.transform.localPosition.y < Water.localPosition.y)
            {
                AddReward(-1.0f);

                if (ShowResults)
                {
                    LastFloorCheck();
                    DataCounterScript.EpisodeCounter++;
                    DataCounterScript.FailAvarageWaterHeightCounter += Water.localPosition.y;
                }

                EndEpisode();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));
            }

            // When Agent is on the 3rd floor
            if (nowFloor == 3)
            {
                timer += Time.deltaTime;
            }
            else timer = 0f;

            if (timer > 5.00f)
            {
                AddReward(0.2f);

                if (ShowResults)
                {
                    LastFloorCheck();
                    DataCounterScript.EpisodeCounter++;
                    DataCounterScript.SuccessCounter++;
                    DataCounterScript.SuccessAvarageWaterHeightCounter += Water.localPosition.y;
                }

                EndEpisode();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(Floor3_Settings.Success_Floor, 0.5f));
            }

            float DistanceToWater = this.transform.localPosition.y - Water.localPosition.y;
            AddReward(((DistanceToWater / 10f) + (0.15f * nowFloor)) / MaxStep);
        }

    }

    // Human Controling
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;  // Forward
        if (Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 2;  // Back
        if (Input.GetKey(KeyCode.D)) discreteActionsOut[1] = 1;  // Rotate right
        if (Input.GetKey(KeyCode.A)) discreteActionsOut[1] = 2;  // Rotate left
    }

    // When touching something
    void OnCollisionEnter(Collision col)
    {
        // Touching floor?
        if (onGround)
        {
            // Wall or Obstacle
            if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Obstacle"))
            {
                AddReward(-0.7f);

                if (ShowResults)
                {
                    LastFloorCheck();
                    DataCounterScript.EpisodeCounter++;
                    DataCounterScript.FailAvarageWaterHeightCounter += Water.localPosition.y;
                }

                EndEpisode();
                StartCoroutine(
                GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));
            }

            // UpStair
            else if (col.gameObject.CompareTag("UpStair"))
            {
                this.rBody.angularVelocity = Vector3.zero;
                this.rBody.velocity = Vector3.zero;
                this.transform.rotation = Quaternion.identity;

                CheckReachedStair(col.gameObject.name);

                if (this.transform.localPosition.x > 0 && this.transform.localPosition.z > 0) { this.transform.Translate(-1.0f, 7.0f, 4.0f); this.transform.Rotate(0f, -90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z > 0) { this.transform.Translate(1.0f, 7.0f, -4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z < 0) { this.transform.Translate(1.0f, 7.0f, -4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x > 0 && this.transform.localPosition.z < 0) { this.transform.Translate(-1.0f, 7.0f, 4.0f); this.transform.Rotate(0f, -90.0f, 0f); }

                nowFloor++;
                CheckClosestStair();

                if (nowFloor - 1 == 1)
                {
                    if (ClosestStair_1F == ReachedStair_1F) AddReward(0.6f);
                    else AddReward(0.3f);
                }

                else if (nowFloor - 1 == 2)
                {
                    if (ClosestStair_2F == ReachedStair_2F) AddReward(0.6f);
                    else AddReward(0.3f);
                }
            }

            // DownStair
            else if (col.gameObject.CompareTag("DownStair"))
            {

                this.rBody.angularVelocity = Vector3.zero;
                this.rBody.velocity = Vector3.zero;
                this.transform.rotation = Quaternion.identity;

                if (this.transform.localPosition.x > 0 && this.transform.localPosition.z > 0) { this.transform.Translate(-1.0f, -7.0f, -4.0f); this.transform.Rotate(0f, -90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z > 0) { this.transform.Translate(1.0f, -7.0f, 4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z < 0) { this.transform.Translate(1.0f, -7.0f, 4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x > 0 && this.transform.localPosition.z < 0) { this.transform.Translate(-1.0f, -7.0f, -4.0f); this.transform.Rotate(0f, -90.0f, 0f); }

                nowFloor--;
                AddReward(-0.5f);
            }
        }

        else
        {
            // Floor
            if (col.gameObject.CompareTag("Floor"))
            {
                onGround = true;
            }

            else if (this.transform.localPosition.y < Water.localPosition.y)
            {
                SpawnAgent();
            }

            // else
            else SpawnAgent();
        }
    }
}