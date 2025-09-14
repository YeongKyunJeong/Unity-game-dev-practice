using UnityEngine;

namespace TDP
{
    // [RequireComponent(typeof(Enemy))]
    public class EnemyMovement : MonoBehaviour
    {
        private Transform target;
        private Vector3 beforeTargetPos;
        private Vector3 dir;
        private float overDistance;

        [SerializeField] private Enemy enemy;
        public bool isAlive;

        [SerializeField] private int wayPointsNumber = 0; // which waypoints
        [SerializeField] private Transform[] myWaypoints;
        [Header("For Check In Inspector")]
        [SerializeField] private float speed = 10f;

        private int wayPointIndex = 0;

        public void Initialize(Enemy _enemy)
        {
            enemy = _enemy;
        }

        public void SetWaypointData(int newWayPointsNumber)
        {
            wayPointIndex = 0;
            wayPointIndex = newWayPointsNumber;
            myWaypoints = WayPoints.GetPoints(wayPointsNumber);

            isAlive = true;

            beforeTargetPos = myWaypoints[wayPointIndex].position;
            target = myWaypoints[++wayPointIndex];  // First waypoint after departure
            transform.position = beforeTargetPos;
        }


        //public void SetSpeed(float newSpeed)
        //{
        //    speed = newSpeed;
        //}

        private void Update()
        {
            if (isAlive)
            {
                dir = target.position - transform.position;
                dir = dir - new Vector3(0, dir.y, 0);
                speed = enemy.GetSpeed;
                overDistance = dir.magnitude - speed * Time.deltaTime;
                if (overDistance < 0) // Should go over target position in this frame
                {
                    beforeTargetPos = target.position;

                    GetNextWayPoiot();

                    transform.position = (target.position - beforeTargetPos).normalized * overDistance + beforeTargetPos;
                }
                else
                {
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                }
                enemy.RollBackSpeedCall();
            }
        }



        void GetNextWayPoiot()
        {
            wayPointIndex++;
            if (wayPointIndex >= myWaypoints.Length)
            {
                PassGoal();
            }
            else
            {
                target = myWaypoints[wayPointIndex];

            }
        }

        void PassGoal()
        {
            if (PlayerStats.Life > 0)
            {
                PlayerStats.Life--;
            }
            EnemySpawner.EnemyAliveCount--;
            gameObject.SetActive(false);
        }

    }



}