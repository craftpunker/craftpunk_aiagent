
namespace Battle
{
    public class Obstacle
    {
        internal Obstacle next_;
        internal Obstacle previous_;
        internal FixVector2 direction_;
        internal FixVector2 point_;
        internal int id_;
        internal bool convex_;
    }
}