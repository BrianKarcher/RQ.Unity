namespace UtilityManager
{
    public interface ICurve
    {
        float Score(float inputValue);
        void SetValues(CurveType curveType, float m, float k, float b, float c);
    }
}