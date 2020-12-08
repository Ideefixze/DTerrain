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
        private BasicCollidableLayer bcl;
        private BlankPaintableLayer bpl;
        public void Start()
        {
            bcl = FindObjectOfType<BasicCollidableLayer>();
            bpl = FindObjectOfType<BlankPaintableLayer>();

        }

        public void Update()
        {
            if (Input.GetMouseButton(0))
            {
                OnMouseClick();
            }
        }

        protected virtual void OnMouseClick()
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bcl.transform.position;
            bcl.Paint(new PaintingParameters() { Color = Color.clear, Position = new Vector2Int((int)(p.x * bcl.PPU) - sphereSize, (int)(p.y * bcl.PPU) - sphereSize), Shape = Shape.GenerateShapeCircle(sphereSize) });

            p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bpl.transform.position;
            bpl.Paint(new PaintingParameters() { Color = Color.black, Position = new Vector2Int((int)(p.x * bpl.PPU) - sphereSize - outlineSize, (int)(p.y * bpl.PPU) - sphereSize - outlineSize), Shape = Shape.GenerateShapeCircle(sphereSize + outlineSize) });
        }
    }
}
