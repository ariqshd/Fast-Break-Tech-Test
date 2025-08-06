using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct GameplayTag : IEquatable<GameplayTag>
    {
        [SerializeField]
        private string tagName;
        
        // Keep the hash for fast comparison (calculated, not serialized)
        [NonSerialized]
        private int tagHash;
        
        public string TagName 
        { 
            get => tagName;
            private set 
            { 
                tagName = value;
                tagHash = value?.GetHashCode() ?? 0;
            }
        }
    
        public int TagHash => tagHash;
    
        public GameplayTag(string tagName)
        {
            this.tagName = tagName ?? string.Empty;
            this.tagHash = tagName?.GetHashCode() ?? 0;
        }
        
        // Ensure hash is calculated when deserialized
        private void OnAfterDeserialize()
        {
            tagHash = tagName?.GetHashCode() ?? 0;
        }
    
        public bool Equals(GameplayTag other)
        {
            return TagHash == other.TagHash;
        }
    
        public override bool Equals(object obj)
        {
            return obj is GameplayTag other && Equals(other);
        }
    
        public override int GetHashCode()
        {
            return TagHash;
        }
    
        public static bool operator ==(GameplayTag left, GameplayTag right)
        {
            return left.Equals(right);
        }
    
        public static bool operator !=(GameplayTag left, GameplayTag right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        /// Checks if this tag is a child of another tag
        /// </summary>
        public bool IsChildOf(GameplayTag parentTag)
        {
            // Check if this tag starts with parentTag + "."
            return TagName.StartsWith(parentTag.TagName + ".");
        }
        
        public override string ToString()
        {
            return TagName;
        }
    }
}