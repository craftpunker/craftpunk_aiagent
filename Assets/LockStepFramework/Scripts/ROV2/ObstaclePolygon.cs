
using System.Collections.Generic;

namespace Battle
{
    public class ObstaclePolygon
    {
        /// <summary>
        /// ����
        /// </summary>
        internal IList<FixVector2> _vertices;

        /// <summary>
        /// �Ƿ���Ҫɾ��
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
