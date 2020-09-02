using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class MouseDestruction : MonoBehaviour
    {
        [Tooltip("Radius of circle that will destroy world on click.")]
        [SerializeField]
        private int radius = 8;
        [Tooltip("Radius of a ring that will create outline on click.")]
        [SerializeField]
        private int outlineRadius = 12;
        [Tooltip("Outline is a color of an original texture multiplied by this value")]
        [SerializeField]
        [Range(0f,1f)]
        private float outlineDarkeningMultiplier = 0.25f;

        private World world;

        private IDestructor destructor;
        // Start is called before the first frame update
        void Start()
        {
            destructor = new WormsDestructor(radius,outlineRadius, outlineDarkeningMultiplier);
            world = FindObjectOfType<World>();
        }

        // Update is called once per frame
        public void Destruction()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - world.transform.position; 
                Vector2Int wPos = world.ScenePositionToWorldPosition(mPos); //Offset should be (0,0) for calcualting our World position thats why we substracted position.
                destructor.Destroy(wPos.x - radius, wPos.y - radius, world);
                
            }

        }
        void Update()
        {
            Destruction();
        }

    }
}


