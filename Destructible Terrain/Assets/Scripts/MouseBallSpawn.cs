using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class MouseBallSpawn : MonoBehaviour
    {
        [SerializeField]
        private GameObject ball;

        void Update()
        {
            CreateBall();
        }

        public void CreateBall()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mPos.z = 0;
                Instantiate(ball, mPos, new Quaternion(0, 0, 0, 0));
            }
        }
    }
}