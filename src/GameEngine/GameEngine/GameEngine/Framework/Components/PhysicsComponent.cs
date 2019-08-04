using System;
namespace GameEngine.Framework.Components
{
    public class PhysicsComponent : IComponent
    {

        private readonly IEntity entity;

        public PhysicsComponent(float vx, float vy, float radius, IEntity entity)
        {
            this.vx = vx;
            this.vy = vy;
            Radius = radius;
            this.entity = entity;
        }

        public float vx { get; set; }
        public float vy { get; set; }
        public float Radius { get; }

        public void Update(float dt)
        {
            entity.Position.X += vx * dt;
            entity.Position.Y += vy * dt;
        }
    }
}