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
        private Shape destructionShape;
        // Start is called before the first frame update
        void Start()
        {
            destructionShape = Shape.GenerateShapeCircle(radius);
        }

        // Update is called once per frame
        public void Destruction()
        {
            World world = FindObjectOfType<World>();
            if (Input.GetMouseButton(0))
            {
                Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - world.transform.position; 
                Vector2Int wPos = world.ScenePositionToWorldPosition(mPos); //Offset should be (0,0) for calcualting our World position thats why we substracted position.
                //Warning: use large power. Lower values are not supported and may cause weird looking texture.
                //You can expand on this idea.
                world.DestroyTerrainWithShape(wPos.x -radius, wPos.y-radius, destructionShape, 10f);
            }
        }
        void Update()
        {
            Destruction();
        }
    }
}


