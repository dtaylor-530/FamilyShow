using Abstractions;
using System;

namespace Microsoft.FamilyShowLib
{
    /// <summary>
    /// Representation for a Parent couple.  E.g. Bob and Sue
    /// </summary>
    public class ParentSet : IEquatable<IParentSet>, IParentSet
    {
        private INode firstParent;

        private INode secondParent;

        public INode FirstParent
        {
            get { return firstParent; }
            set { firstParent = value; }
        }

        public INode SecondParent
        {
            get { return secondParent; }
            set { secondParent = value; }
        }

        public ParentSet(INode firstParent, INode secondParent)
        {
            this.firstParent = firstParent;
            this.secondParent = secondParent;
        }

        public string Name
        {
            get
            {
                string name = string.Empty;
                name += $"{firstParent.Name} + {secondParent.Name}";
                return name;
            }
        }

        // Parameterless contstructor required for serialization
        public ParentSet() { }

        #region IEquatable<ParentSet> Members

        /// <summary>
        /// Determine equality between two ParentSet classes.  Note: Bob and Sue == Sue and Bob
        /// </summary>
        public bool Equals(IParentSet other)
        {
            if (other != null)
            {
                if (firstParent.Equals(other.FirstParent) && secondParent.Equals(other.SecondParent))
                {
                    return true;
                }

                if (firstParent.Equals(other.SecondParent) && secondParent.Equals(other.FirstParent))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}