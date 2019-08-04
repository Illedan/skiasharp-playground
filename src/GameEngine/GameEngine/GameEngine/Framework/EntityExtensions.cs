using System;
namespace GameEngine.Framework
{
    public static class EntityExtensions
    {
        public static float GetRelativeX(this IEntity entity)
        {
            var x = entity.Position.X;
            entity = entity.Parent;
            while(entity != null)
            {
                x += entity.Position.X;
                entity = entity.Parent;
            }

            return x;
        }

        public static float GetRelativeY(this IEntity entity)
        {
            var y = entity.Position.Y;
            entity = entity.Parent;
            while (entity != null)
            {
                y += entity.Position.Y;
                entity = entity.Parent;
            }

            return y;
        }
    }
}