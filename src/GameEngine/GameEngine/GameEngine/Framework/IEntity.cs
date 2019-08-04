using System;
using System.Collections.Generic;

namespace GameEngine.Framework
{
    public interface IEntity : IUpdatable
    {
        List<IComponent> Components { get; }
        Position Position { get; }
        IEntity Parent { get; }
    }
}
