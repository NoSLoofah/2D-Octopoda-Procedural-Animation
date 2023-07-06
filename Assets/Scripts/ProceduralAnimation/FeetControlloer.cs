using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace NoSLoofah.Animation
{
    public class FeetControlloer : MonoBehaviour
    {
        [Tooltip("触手根部位置")] [SerializeField] private Transform feetRoot;
        [Tooltip("触手着地点检测线数量")] [Range(1, 20)] [SerializeField] private int detectorCount;
        [Tooltip("触手着地点检测线长度")] [SerializeField] private float detectorLength;
        [Tooltip("触手允许落的层级")] [SerializeField] private LayerMask ground;
        private Tentacle[] tentacles;               //触手对象
        private Transform[] footPos;                //触手着地点
        private bool isWorking;                     //组件是否运行中        
        private int groundedFootCount;              //当前着地的触手数量

        /// <summary>
        /// 触手对象
        /// </summary>
        public Transform[] FootPos => footPos;

        /// <summary>
        /// 当前着地的触手数量
        /// </summary>
        public int GroundedFootCount => groundedFootCount;

        /// <summary>
        /// 触手的总数量
        /// </summary>
        public int FootCount => tentacles.Length;

        /// <summary>
        /// 重心相对中心的水平坐标（根据已经着地的触手坐标）
        /// </summary>
        public float GravityCenterX
        {
            get
            {
                float output = 0;
                foreach (var i in footPos)
                {

                    output += i.position.x - transform.position.x;
                }
                return output / footPos.Length;
            }
        }


        #region 缓存
        private Vector2 newVector = new Vector2();
        List<Vector2> points;
        #endregion

        private void Awake()
        {
            tentacles = new Tentacle[feetRoot.childCount];
            footPos = new Transform[feetRoot.childCount];
            for (int i = 0; i < feetRoot.childCount; i++)
            {
                tentacles[i] = feetRoot.GetChild(i).GetComponent<Tentacle>();
                footPos[i] = tentacles[i].EndPos;
                tentacles[i].InitialFoot(detectorLength);
            }
        }
        private void Update()
        {
            if (!isWorking) return;
            CheckFootTarget();
            MoveFoot();
        }

        private void OnEnable()
        {
            StartCoroutine(co());
            IEnumerator co()
            {
                yield return null;
                isWorking = true;
                groundedFootCount = 0;
            }
        }

        /// <summary>
        /// 根据着地检测线找到周围可以着地的点
        /// 将所有可着地点存入points
        /// </summary>
        private void CheckFootTarget()
        {
            points = new List<Vector2>();
            RaycastHit2D hitinfo;
            float increase = 360f / detectorCount;
            for (float i = 0; i < 360; i += increase)
            {
                hitinfo = Physics2D.Raycast(transform.position, Tool.GetUnitVectorFromAngle(i), detectorLength, ground);
                if (hitinfo.collider != null)
                {
                    //可选着地点存入points
                    points.Add(hitinfo.point);
                }
            }
        }

        /// <summary>
        /// 在CheckFootTarget后调用
        /// 让当前可移动的触手移动到最合适的可着地点
        /// </summary>
        private void MoveFoot()
        {
            groundedFootCount = 0;
            foreach (var t in tentacles)
            {
                //着地时突然撤销地面会不能恢复
                if (t.NeedStep())
                {
                    t.EvaluatePoints(ref points, GravityCenterX);
                    t.MoveToPoint(points.Count > 0);
                }
                else if (t.FootOnGround)
                {
                    groundedFootCount++;
                }
            }
        }

        private void OnDrawGizmos()
        {
            //绘制着地点检测线
            float increase = 360 / detectorCount;
            for (float i = 0; i < 360; i += increase)
            {
                Gizmos.DrawLine(transform.position, transform.position + Tool.GetUnitVectorFromAngle(i) * detectorLength);
            }
        }
    }
}