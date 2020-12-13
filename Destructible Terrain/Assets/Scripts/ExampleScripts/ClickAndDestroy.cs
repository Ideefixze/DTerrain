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
        private int circleSize = 16;
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
            collisionLayer.Paint(new PaintingParameters() 
            { 
                Color = Color.clear, 
                Position = new Vector2Int((int)(p.x * collisionLayer.PPU) - circleSize, (int)(p.y * collisionLayer.PPU) - circleSize), 
                Shape = Shape.GenerateShapeCircle(circleSize), 
                PaintingMode=PaintingMode.NONE,
                DestructionMode = DestructionMode.BUILD
            });

            outlineLayer.Paint(new PaintingParameters() 
            { 
                Color = new Color(0.0f,0.0f,0.0f,0.75f), 
                Position = new Vector2Int((int)(p.x * outlineLayer.PPU) - circleSize-outlineSize, (int)(p.y * outlineLayer.PPU) - circleSize-outlineSize), 
                Shape = Shape.GenerateShapeCircle(circleSize+outlineSize), 
                PaintingMode=PaintingMode.REPLACE_COLOR,
                DestructionMode = DestructionMode.NONE
            });
            
        }
    }
}
