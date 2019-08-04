using System;
namespace GameEngine.Framework.Components
{
    public class WallCollisionComponent : IComponent
    {
        private readonly IEntity entity;
        private readonly PhysicsComponent physicsComponent;
        private readonly float width;
        private readonly float height;

        public WallCollisionComponent(IEntity entity, PhysicsComponent physicsComponent, float width, float height)
        {
            this.entity = entity;
            this.physicsComponent = physicsComponent;
            this.width = width;
            this.height = height;
        }

        public void Update(float dt)
        {
            if(entity.Position.X < physicsComponent.Radius)
            {
                physicsComponent.vx = Math.Abs(physicsComponent.vx);
                entity.Position.X = physicsComponent.Radius;
            }
            else if(entity.Position.X >= width - physicsComponent.Radius)
            {
                physicsComponent.vx = Math.Abs(physicsComponent.vx) * -1;
                entity.Position.X = width - physicsComponent.Radius;
            }

            if(entity.Position.Y < physicsComponent.Radius)
            {
                physicsComponent.vy = Math.Abs(physicsComponent.vy);
                entity.Position.Y = physicsComponent.Radius;
            }
            else if(entity.Position.Y > height - physicsComponent.Radius)
            {
                physicsComponent.vy = Math.Abs(physicsComponent.vy) * -1;
                entity.Position.Y = height - physicsComponent.Radius;
            }
        }
    }
}
