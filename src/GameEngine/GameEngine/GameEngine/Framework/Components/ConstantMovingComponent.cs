namespace GameEngine.Framework.Components
{
    public class ConstantMovingComponent : IComponent
    {
        private readonly float vx;
        private readonly float vy;
        private readonly IEntity entity;

        public ConstantMovingComponent(float vx, float vy, IEntity entity)
        {
            this.vx = vx;
            this.vy = vy;
            this.entity = entity;
        }

        public void Update(float dt)
        {
            entity.Position.X += vx * dt;
            entity.Position.Y += vy * dt;
        }
    }
}