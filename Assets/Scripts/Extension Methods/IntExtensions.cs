using UnityEngine;

namespace Extension_Methods
{
    public static class IntExtensions
    {
        public static int IndexToPolySize(this int i) => (int)Mathf.Pow(2, i + 1);
    }
}