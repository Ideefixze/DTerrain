using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Example script: Spawining Unity gameobject with collider to show that DTerrain works with Unity Colliders 2D.
    /// </summary>
    public class MouseBallSpawn : MonoBehaviour
    {
        [SerializeField]
        private GameObject ball=null;

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