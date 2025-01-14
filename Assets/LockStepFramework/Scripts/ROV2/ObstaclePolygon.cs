
using System.Collections.Generic;

namespace Battle
{
    public class ObstaclePolygon
    {
        /// <summary>
        /// 顶点
        /// </summary>
        internal IList<FixVector2> _vertices;

        /// <summary>
        /// 是否需要删除
        /// </summary>
        internal bool _isNeedDelete;

        /// <summary>
        /// id
        /// </summary>
        public int _id;

        public ObstaclePolygon()
        {
            this._vertices = new List<FixVector2>();
        }
    }
}
