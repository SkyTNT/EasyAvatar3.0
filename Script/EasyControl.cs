using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyControl : MonoBehaviour
    {
        public enum Type
        {
            Toggle,
            RadialPuppet
        }

        [SerializeField]
        public Texture2D icon;
        [SerializeField]
        public List<EasyBehavior> behaviors1, behaviors2, behaviors3, behaviors4;
        [SerializeField]
        public bool useAnimClip;
        [SerializeField]
        public List<AnimationClip> anims1, anims2, anims3, anims4;
        [SerializeField]
        public Type type;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
