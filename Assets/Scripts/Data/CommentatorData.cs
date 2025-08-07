using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = nameof(CommentatorData), menuName = "Misc/" + nameof(CommentatorData), order = 0)]
    public class CommentatorData : ScriptableObject
    {
        public List<string> comments = new List<string>();
    }
}