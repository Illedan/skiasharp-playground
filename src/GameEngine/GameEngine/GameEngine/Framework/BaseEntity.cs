using System.Collections.Generic;

namespace GameEngine.Framework
{
    public class BaseEntity : IEntity
    {
        public BaseEntity(Position initialPosition, List<IComponent> components, IEntity parent) 
        {
            Position = initialPosition;
            Components = components;
            Parent = parent;
        }

        public List<IComponent> Components { get; }

        public Position Position { get; }

        public IEntity Parent { get; }

        public void Update(float dt)
        {
            foreach (var component in Components)
            {
                component.Update(dt);
            }
        }
    }
}
