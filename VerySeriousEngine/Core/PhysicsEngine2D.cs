namespace VerySeriousEngine.Core
{
    public class PhysicsEngine2D
    {
        internal void Update(float frameTime, World world)
        {
            foreach (var physicsComponent in world.Physics2DComponents)
            {
                foreach (var other in world.Physics2DComponents)
                {
                    if (physicsComponent != other)
                        physicsComponent.UpdateOverlapWith(other);
                }
            }
        }
    }
}