using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace NoSLoofah.Animation
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineDrawer : MonoBehaviour
    {
        [SerializeField] private LineType lineType;
        [Tooltip("是否启用")] [SerializeField] private bool active;
        [Tooltip("绘线结点数")] [SerializeField] private int posCount;
        [Tooltip("绘线终点位置")] [SerializeField] private Transform endPos;
        [Header("绘线参数")]
        [Tooltip("绘线拉伸最大长度")] [SerializeField] private float visualMaxLength;
        [SerializeField] private bool flip;
        [SerializeField] [Range(0, 1f)] private float p1Pos;
        [SerializeField] private float p1Offset;
        [SerializeField] [Range(0, 1f)] private float p2Pos;
        [SerializeField] private float p2Offset;
        private int currentPosCount;            //实际绘制的绘线结点数
        private Vector3[] v3positions;
        private LineRenderer line;
        [System.Serializable]
        enum LineType
        {
            Bezier3, Bezier4
        }
        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            active = !active;
            Activatactivate(!active);
        }
        private void DrawLine()
        {
            float lParam = 1 - Mathf.Min(1, Vector3.Distance(transform.position, endPos.position) / visualMaxLength);


            //画线
            switch (lineType)
            {
                case LineType.Bezier3:
                    {
                        Vector3 p1 = Vector3.Lerp(transform.position, endPos.position, p1Pos) + Vector3.up * p1Offset * lParam * (flip ? 1 : -1);
                        for (int i = 0; i < posCount; i++)
                        {
                            if (i >= currentPosCount) break;
                            float t = (float)i / (posCount - 1);
                            Vector3 position = Tool.CalculateBezierPoint(transform.position, p1, endPos.position, t);
                            position.Set(position.x, position.y, 0);
                            v3positions[i] = position;
                        }
                        break;
                    }
                case LineType.Bezier4:
                    {
                        Vector3 p1 = Vector3.Lerp(transform.position, endPos.position, p1Pos) + Vector3.up * p1Offset * lParam * (flip ? 1 : -1);
                        Vector3 p2 = Vector3.Lerp(transform.position, endPos.position, p2Pos) + Vector3.down * p2Offset * lParam * (flip ? 1 : -1);
                        for (int i = 0; i < posCount; i++)
                        {
                            if (i >= currentPosCount) break;
                            float t = (float)i / (posCount - 1);
                            Vector3 position = Tool.CalculateFourthOrderBezier(transform.position, p1, p2, endPos.position, t);
                            position.Set(position.x, position.y, 0);
                            v3positions[i] = position;
                        }
                        break;
                    }
                default:
                    break;
            }

            line.SetPositions(v3positions);
        }
        /// <summary>
        /// 激活画线
        /// </summary>
        /// <param name="to"></param>
        public void Activatactivate(bool to)
        {
            if (to == active) return;
            if (to)
            {
                line.enabled = true;
                line.positionCount = posCount;
                v3positions = new Vector3[posCount];
                currentPosCount = posCount;
            }
            else
            {
                line.enabled = false;
            }
            active = to;
        }
        private void Update()
        {
            if (active) DrawLine();
        }
    }
}