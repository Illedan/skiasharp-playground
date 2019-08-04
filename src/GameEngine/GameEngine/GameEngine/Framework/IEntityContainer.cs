using System;
using System.Collections.Generic;

namespace GameEngine.Framework
{
    public interface IEntityContainer : IEntity
    {
        List<IEntity> Entities { get; }
    }
}
