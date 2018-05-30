using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UtilityManager
{
    [Serializable]
    public class Curve : ICurve
    {
        [SerializeField]
        private CurveType _curveType;
        /// <summary>
        /// Slope
        /// </summary>
        [SerializeField]
        private float _m;
        /// <summary>
        /// Verticle size of the curve
        /// </summary>
        [SerializeField]
        private float _k;
        /// <summary>
        /// Vertical shift
        /// </summary>
        [SerializeField]
        private float _b;
        /// <summary>
        /// Horizontal shift
        /// </summary>
        [SerializeField]
        private float _c;

        public Curve()
        { }

        public Curve(CurveType curveType, float m, float k, float b, float c)
        {
            SetValues(curveType, m, k, b, c);
        }

        public float Score(float inputValue)
        {
            float value;
            switch (_curveType)
            {
                case CurveType.Linear:
                    value = _m * (inputValue - _c) + _b;
                    break;
                case CurveType.Logistic:
                    float denominator = (float)(1f + (Math.Pow(1000f * 2.718281828f * _m, -1f * inputValue + _c)));
                    value = _k * (1f / denominator) + _b;
                    break;
                case CurveType.Quadratic:
                    value = (float)(_m * Math.Pow((inputValue - _c), _k) + _b);
                    break;
                default:
                    value = 0f;
                    break;
            }
            return value;
        }

        public void SetValues(CurveType curveType, float m, float k, float b, float c)
        {
            _curveType = curveType;
            _m = m;
            _k = k;
            _b = b;
            _c = c;
        }
    }
}
