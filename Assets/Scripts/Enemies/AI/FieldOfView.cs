using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float discoveredViewAngle = 360f;

    public float edgeDistanceThreshold;
    public int edgeResolveIterations;
    [HideInInspector]
    public bool isChasing = false;
    public float meshResolution = 6f;

    public LayerMask obstacleMask;
    public LayerMask targetMask;

    public float playerFoundDetectionRadiusMultiplier = 3.0f;
    public bool showDebugText = false;
    public bool showConeOfShame = false;
    
    [Range(0, 360)]
    public float viewAngle = 90f;

    public MeshFilter viewMeshFilter;
    public float viewRadius = 10f;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private NavMeshAgent agent;
    private bool hadPlayer = false;
    private Vector3 initialPosition;
    private float previousViewAngle;
    private Rect textArea = new Rect(0, 0, Screen.width, Screen.height);
    private Mesh viewMesh;
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        viewPoints.Add(edge.pointA);
                    }

                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1; //number of vertexes + the origin
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3]; //number of vertexes - 2 * 3 vertexes in each triangle - not sure on the 2

        vertices[0] = Vector3.zero; //origin relative to player
        for (int i = 0; i < vertexCount - 1; i++) //start at +1 since we already set the origin
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                //store the points of the triangles in the following array format
                //[0,1,2,0,3,4,0,5,6] 0,1,2 being the first triangle 0,3,4 being the second
                //and 0,5,6 being the third, etc...
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInviewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //If we have no "targets", clean up and get outta here!
        if (targetsInviewRadius.Length == 0)
        {
            //Can't find player, turn off chasing
            isChasing = false;

            //We are no longer chasing the player so return to my original location
            if (hadPlayer)
            {
                //lost the enemy, reset viewRadius
                viewRadius = viewRadius / playerFoundDetectionRadiusMultiplier;
                viewAngle = previousViewAngle;
                hadPlayer = false;
                agent.SetDestination(initialPosition);
            }

            return;
        }

        for (int i = 0; i < targetsInviewRadius.Length; i++)
        {
            Transform target = targetsInviewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);

                    if (!hadPlayer) //found the target so increase focus!
                    {
                        //enemy is focused now, increase the viewRadius
                        viewRadius = viewRadius * playerFoundDetectionRadiusMultiplier;
                        previousViewAngle = viewAngle;
                        viewAngle = discoveredViewAngle;
                    }

                    isChasing = true;
                    hadPlayer = true;

                    var playerPosition = target.transform.position;
                    agent.SetDestination(playerPosition);
                    break;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (showConeOfShame)
            DrawFieldOfView();
    }

    void OnGUI()
    {
        if (showDebugText)
        {
            UpdateDebugGUIDisplay();
        }
    }

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        agent = GetComponent<NavMeshAgent>();
        //Save off initial position
        initialPosition = transform.position;
        StartCoroutine("FindTargetsWithDelay", .5f);

    }
    void UpdateDebugGUIDisplay()
    {

        GUI.Label(textArea, string.Format("{0} targets visible", visibleTargets.Count.ToString()));
    }
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    public struct ViewCastInfo
    {
        public float angle;
        public float dst;
        public bool hit;
        public Vector3 point;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
}
