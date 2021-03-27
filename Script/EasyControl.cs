using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAvatar
{
    public class EasyControl : MonoBehaviour
    {
        [SerializeField]
        public Texture2D icon;
        [SerializeField]
        public List<EasyBehavior> offBehaviors, onBehaviors;
        [SerializeField]
        public bool useAnimClip;
        [SerializeField]
        public List<AnimationClip> offAnims, onAnims;
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
