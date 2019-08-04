using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.Framework
{
    public class EntityContainer : IEntityContainer
    {
        public EntityContainer(IEntity parent)
        {
            Parent = parent;
        }

        public List<IComponent> Components { get; } = new List<IComponent>();
        public List<IEntity> Entities { get; } = new List<IEntity>();
        public Position Position { get; } = new Position();
        public IEntity Parent { get; }

        public void Update(float dt)
        {
            foreach(var component in Components)
            {
                component.Update(dt);
            }

            foreach(var entity in Entities.OrderBy(e => e.Position.Z))
            {
                entity.Update(dt);
            }
        }
    }
}