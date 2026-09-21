using UnityEngine;

namespace ColliderEventSystem
{
    public static class EaseUtility
    {
        /// <summary>
        /// Evaluates an eased 0-1 progress value. When ease is Custom, evaluates the given curve instead
        /// (the curve is expected to run from (0,0) to (1,1); its shape supplies the easing).
        /// </summary>
        public static float Evaluate(Ease ease, AnimationCurve customCurve, float t)
        {
            t = Mathf.Clamp01(t);

            switch (ease)
            {
                case Ease.Linear:
                    return t;

                case Ease.EaseIn:
                    return t * t;

                case Ease.EaseOut:
                    return 1f - (1f - t) * (1f - t);

                case Ease.EaseInOut:
                    return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

                case Ease.Custom:
                    return customCurve != null ? customCurve.Evaluate(t) : t;

                default:
                    return t;
            }
        }
    }
}
