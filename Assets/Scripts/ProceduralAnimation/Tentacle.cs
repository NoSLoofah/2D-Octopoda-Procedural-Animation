using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace NoSLoofah.Animation
{
    [RequireComponent(typeof(LineRenderer))]
    public class Tentacle : MonoBehaviour
    {
        [Header("静态外形参数")]
        [Tooltip("绘线结点数")] [SerializeField] private int posCount;
        [Range(0, 1f)] [SerializeField] private float p1Pos;
        [Range(0, 1f)] [SerializeField] private float p2Pos;
        [Tooltip("触手外形伸直的最短距离")] [SerializeField] private float visualMaxLength;
        [Tooltip("P1控制点偏移量（用于绘制贝塞尔曲线）")] [SerializeField] private float p1Offset;
        [Tooltip("P2相对P1控制点偏移量（用于绘制贝塞尔曲线）")] [SerializeField] private float p2OffsetMutiple;
        [Tooltip("控制点反转用时")] [SerializeField] private float reverseTime;
        [Tooltip("判断着地的接受距离")] [SerializeField] private float touchGroundOffset;

        private bool reverse;                               //是否翻折
        private bool inAir = false;                         //落点是否在半空
        private float realPOffset;                          //当前p1偏移量.在翻折过程动态修改
        private float poffsetRef;
        private float reverseTimer;                         //翻折过程计时器


        private Transform endPos;                           //腕足末端
        private Transform refPos;                           //着地点评价参考点
        private Transform airRefPos;                        //落点滞空参考点    
        private Vector3[] v3positions;                      //绘线点集
        private Vector2[] v2positions;                      //绘线点集

        private LineRenderer line;
        private Vector2 newVector = new Vector2();

        public Transform EndPos => endPos;
        public bool FootOnGround => footOnGround;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            endPos = transform.GetChild(0);
            refPos = transform.GetChild(1);
            airRefPos = transform.GetChild(2);
            line.positionCount = posCount;
            v3positions = new Vector3[posCount];
            v2positions = new Vector2[posCount];
            realPOffset = p1Offset;
            reverseTimer = 0;
            setInAirTimer = 0;
            moveTimeTTimer = 0;
            targetPos = endPos.position;
            lastTouchedPos = transform.position;
            footOnGround = false;
            SetInAir();
        }
        private void Update()
        {
            DetectTouchGround();
            SetVisualFoot();
            //滞空动画，单拉一个函数
            if (inAir) WhenInAir();
            if (moveTimeTTimer < moveTimeThreshold) moveTimeTTimer += Time.deltaTime;
            if (setInAirTimer < setInAirThrehold) setInAirTimer += Time.deltaTime;
        }
        [Tooltip("两次滞空位置改变的最短时间间隔")] [SerializeField] private float setInAirThrehold;
        private float setInAirTimer = 0;
        private void WhenInAir()
        {
            if (!(setInAirTimer >= setInAirThrehold)) return;
            Debug.Log("air anim");
            targetPos = (Vector2)airRefPos.position + Tool.GetRandomDir(inAirDistance);
            MoveToPoint(false);
            setInAirTimer = 0;
        }
        #region 触手检测和绘制
        /// <summary>
        /// 判断触手是否触地
        /// </summary>
        private void DetectTouchGround()
        {
            if (!inAir && Vector2.Distance(endPos.position, targetPos) < touchGroundOffset)
            {
                footOnGround = true;
                lastTouchedPos = endPos.position;
            }
            else
            {
                footOnGround = false;
            }
        }
        /// <summary>
        /// 绘制触手外观
        /// 调用后，触手的每个绘制结点会被存入v3positions
        /// </summary>
        private void SetVisualFoot()
        {
            float lParam = 1 - Mathf.Min(1, Vector3.Distance(transform.position, endPos.position) / visualMaxLength);

            //反转
            if (!reverse && (realPOffset < p1Offset))
            {
                reverseTimer += Time.deltaTime;
                realPOffset = Mathf.Lerp(poffsetRef, p1Offset, Mathf.Min(1f, reverseTimer / reverseTime));
            }
            else if (reverse && realPOffset > -p1Offset)
            {
                reverseTimer += Time.deltaTime;
                realPOffset = Mathf.Lerp(poffsetRef, -p1Offset, Mathf.Min(1f, reverseTimer / reverseTime));
            }

            //画线
            Vector3 p1 = Vector3.Lerp(transform.position, endPos.position, p1Pos) + Vector3.up * realPOffset * lParam;
            Vector3 p2 = Vector3.Lerp(transform.position, endPos.position, p2Pos) + Vector3.down * realPOffset * p2OffsetMutiple * lParam; ;
            for (int i = 0; i < posCount; i++)
            {
                float t = (float)i / (posCount - 1);
                Vector3 position = Tool.CalculateFourthOrderBezier(transform.position, p1, p2, endPos.position, t);
                position.Set(position.x, position.y, 0);
                v3positions[i] = position;
            }

            line.SetPositions(v3positions);
        }
        #endregion        
        #region 公共方法

        /// <summary>
        /// 将触手的曲线上下反转
        /// </summary>
        [ContextMenu("reverse")]
        public void ReverseCurve()
        {
            reverseTimer = 0;
            poffsetRef = realPOffset;
            reverse = !reverse;
        }

        [Header("腿的移动")]
        [Tooltip("落足点滞空范围")] [SerializeField] private float inAirDistance;
        [Tooltip("腿移动速度")] [SerializeField] private float moveFootTime;
        [Tooltip("着地动作曲线的P参考点（用于绘制贝塞尔曲线）")] [Range(0, 1)] [SerializeField] private float movePPos;
        [Tooltip("着地动作中腿部抬起高度")] [SerializeField] private float movePosOffset;
        private float moveMaxLength;                    //移动超过该距离迈步
        private Vector2 lastTouchedPos;                 //上一个落足点
        private Vector2 targetPos;                      //目标落足点
        private bool footOnGround = true;               //是否着地        

        /// <summary>
        /// 判断是否需要迈步
        /// </summary>
        /// <returns>是否需要迈步</returns>
        public bool NeedStep()
        {
            return ((((Vector2)transform.position - lastTouchedPos).sqrMagnitude > moveMaxLength * moveMaxLength) || inAir)
                && !(moveTimeTTimer < moveTimeThreshold);
        }

        /// <summary>
        /// 强制收回触手到悬空位置
        /// </summary>
        public void SetInAir()
        {
            if (inAir) return;
            targetPos = (Vector2)airRefPos.position;
            lastTouchedPos = endPos.position;
            setInAirTimer = 0;
            Debug.Log("setinAir");
            inAir = true;
            endPos.SetParent(transform);
        }
        /// <summary>
        /// 评估最优落足点。若没有任何落足点，则在空中寻找随机点
        /// 评分越低越优质
        /// </summary>
        /// <param name="points">潜在落足点</param>    
        public void EvaluatePoints(ref List<Vector2> points, float centerX)
        {
            if (points.Count <= 0)
            {
                if (!inAir)
                {
                    SetInAir();
                }
                return;
            }
            if (inAir)
            {
                inAir = false;
                endPos.SetParent(null);
            }

            float score = float.MaxValue;
            float c = 0;
            float angle;

            //候选着地点评分逻辑
            //可以根据需要修改优化
            foreach (var p in points)
            {
                angle = Vector2.Angle(refPos.position - transform.position, p - (Vector2)transform.position);
                c = angle;
                //if (Mathf.Sign(p.x - transform.position.x) != Mathf.Sign(centerX))
                //{
                //    c *= (p.x - centerX);
                //}
                if (c < score)
                {
                    score = c;
                    newVector = p;
                }
            }
            if (targetPos.Equals(newVector))
            {
                DOTween.KillAll();
                endPos.position = targetPos;
                moveTimeTTimer = 0;
            }
            //设定落足点
            points.Remove(newVector);
            targetPos = newVector;
        }

        [Tooltip("两次移动触手的最短时间间隔")] [SerializeField] private float moveTimeThreshold;
        private float moveTimeTTimer = 0;               //触手移动计时器
        private Tweener footTweenr;                     //触手移动的缓动对象
        private Vector2 moveVector;                     //移动的贝塞尔曲线参考点

        /// <summary>
        /// 将足末端按贝塞尔曲线轨迹移动到目标点
        /// </summary>
        /// <param name="curve">是否使用贝塞尔曲线的轨迹移动</param>
        public void MoveToPoint(bool curve)
        {
            if (moveTimeTTimer < moveTimeThreshold) return;
            moveTimeTTimer = 0;
            if (footTweenr != null && footTweenr.IsPlaying())
            {
                footTweenr.Kill();
            }
            int flag = (targetPos.y >= transform.position.y) ? -1 : 1;
            if (curve)
            {
                //移动的贝塞尔曲线参考点
                moveVector = Vector2.Lerp(endPos.position, targetPos, movePPos) + Vector2.up * movePosOffset * flag;
                //足缓动对象
                footTweenr = DOTween.To(() => 0f, t => endPos.position = Tool.CalculateBezierPoint(endPos.position, moveVector, targetPos, t)
                , 1f, moveFootTime).SetEase(Ease.Linear);
            }
            else
            {
                footTweenr = footTweenr = endPos.DOMove(targetPos, moveFootTime).SetEase(Ease.Linear);
            }
            footTweenr.onComplete += OnTouchGround;
            footTweenr.Play();
        }

        /// <summary>
        /// 足初始化
        /// </summary>
        /// <param name="length">迈步需要最短距离</param>
        public void InitialFoot(float length)
        {
            moveMaxLength = length;
        }
        #endregion        
        #region 内部事件
        [Header("音效")]
        [Tooltip("触手落地的音效")] [SerializeField] private string[] soundNameWhenTouchGround;
        private void OnTouchGround()
        {
            int i = soundNameWhenTouchGround.Length;
            if (i <= 0) return;
            i = Random.Range(0, i);
            AudioMgr.GetInstance().PlaySound(soundNameWhenTouchGround[i], false);
        }
        #endregion

        #region Gizmos
#if UNITY_EDITOR
        [Header("Gizmos")]
        [Tooltip("展示迈步距离")] [SerializeField] private bool moveMaxLengthGizmos;
        [Tooltip("展示目标着地位置")] [SerializeField] private bool targetPosGizmos;
        [Tooltip("展示足末端位置")] [SerializeField] private bool endPosGizmos;
        private void OnDrawGizmos()
        {
            //不换腿最长距离
            if (moveMaxLengthGizmos)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, moveMaxLength);
            }
            if (UnityEditor.EditorApplication.isPlaying)
            {
                Gizmos.color = Color.cyan;
                if (endPosGizmos)
                {
                    Gizmos.DrawWireSphere(endPos.position, 0.5f);
                    if (targetPosGizmos)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(targetPos, 0.5f);
                        Gizmos.DrawLine(targetPos, endPos.position);
                    }
                }
                else if (targetPosGizmos)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(targetPos, 0.5f);
                }
                //Gizmos.DrawWireSphere(p, 0.5f);
            }
        }
#endif
        #endregion        

    }
}