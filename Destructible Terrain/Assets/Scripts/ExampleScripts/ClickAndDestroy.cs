using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class ClickAndDestroy : MonoBehaviour
    {
        [SerializeField]
        private int sphereSize = 16;
        [SerializeField]
        private int outlineSize = 4;

        [SerializeField]
        private BasicPaintableLayer collisionLayer;
        [SerializeField]
        private BasicPaintableLayer outlineLayer;

        public void Update()
        {
            if (Input.GetMouseButton(0))
            {
                OnMouseClick();
            }
        }

        protected virtual void OnMouseClick()
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - collisionLayer.transform.position;
            collisionLayer.Paint(new PaintingParameters() { Color = Color.clear, Position = new Vector2Int((int)(p.x * collisionLayer.PPU) - sphereSize, (int)(p.y * collisionLayer.PPU) - sphereSize), Shape = Shape.GenerateShapeCircle(sphereSize), PaintingMode=PaintingMode.REPLACE_COLOR });


            /*
            outlineLayer.Paint(new PaintingParameters()
            {
                Color = new Color(0.0f,0.0f,0.0f,0.7f),
                Position = new Vector2Int((int)(p.x * outlineLayer.PPU) - sphereSize - outlineSize, (int)(p.y * outlineLayer.PPU) - sphereSize - outlineSize),
                Shape = Shape.GenerateShapeCircle(sphereSize + outlineSize),
                PaintingMode = PaintingMode.REPLACE_COLOR
            });
            */

            /*
            outlineLayer.Paint(new PaintingParameters() 
            { Color = new Color(-0.1f,-0.1f,-0.1f,0.0f), 
            Position = new Vector2Int((int)(p.x * outlineLayer.PPU) - sphereSize-outlineSize, (int)(p.y * outlineLayer.PPU) - sphereSize-outlineSize), 
            Shape = Shape.GenerateShapeCircle(sphereSize+outlineSize), 
            PaintingMode=PaintingMode.ADD_COLOR 
            });*/

            
            outlineLayer.Paint(new PaintingParameters() 
            { Color = Color.black, 
            Position = new Vector2Int((int)(p.x * outlineLayer.PPU) - sphereSize-outlineSize, (int)(p.y * outlineLayer.PPU) - sphereSize-outlineSize), 
            Shape = Shape.GenerateShapeCircle(sphereSize+outlineSize), 
            PaintingMode=PaintingMode.REPLACE_COLOR
            });
            

            //p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bpl.transform.position;
            //bpl.Paint(new PaintingParameters() { Color = Color.black, Position = new Vector2Int((int)(p.x * bpl.PPU) - sphereSize - outlineSize, (int)(p.y * bpl.PPU) - sphereSize - outlineSize), Shape = Shape.GenerateShapeCircle(sphereSize + outlineSize) });
        }
    }
}
