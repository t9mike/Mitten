using System;

namespace Mitten.Mobile.Model
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets the current version number for the entity.
        /// </summary>
        /// <returns>The current version number.</returns>
        public abstract int GetVersion();

        /// <summary>
        /// Serves as a hash function for a Entity object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current Entity.
        /// </summary>
        /// <param name="obj">The object to compare with the current Entity.</param>
        /// <returns><c>true</c> if the specified object is equal to the current Entity; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Entity item = obj as Entity;
            if (item == null)
            {
                return false;
            }

            return this.GetId() == item.GetId();
        }

        public static bool operator ==(Entity lhs, Entity rhs)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (object.ReferenceEquals(lhs, null) || object.ReferenceEquals(rhs, null))
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Entity lhs, Entity rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Gets the identifier for this entity.
        /// </summary>
        /// <returns>The identifier.</returns>
        protected abstract Guid GetId();
    }
}