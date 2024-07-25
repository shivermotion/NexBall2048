using UnityEngine;

namespace Extension_Methods
{
    public static class RectExtensions
    {
        public static Rect FirstLine(this Rect r) => new Rect(r) { height = 18 };
        public static Rect NextVertical(this Rect r) => new Rect(r) { y = r.y + r.height };
        public static Rect NextHorizontal(this Rect r) => new Rect(r) { x = r.x + r.width };

        public static Rect XPercent(this Rect r, float min, float max)
        {
            min = Mathf.Clamp01(min);
            max = Mathf.Clamp01(max);
            float xMin = r.x + r.width * min;
            float xMax = r.x + r.width * max;
            return new Rect(r) { xMin = xMin, xMax = xMax };
        }

        public static Rect XPixels(this Rect r, float width) => new Rect(r) { width = width };
        public static Rect XPixelsRemainder(this Rect r, float width) => new Rect(r) 
            { width = r.width-width, x = r.x + width };
    }
}