using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSight : MonoBehaviour {

    [SerializeField]
    private string targetTag;

  
    private Collider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == targetTag)
        {
            //Debug.Log(" in if WallSight OnTriggerEnter2D " + other);
            //Debug.Log(" in if WallSight OnTriggerEnter2D " + Player.Instance.horizontal);
            Player.Instance.horizontal_temp = Player.Instance.horizontal;
            Player.Instance.horizontal = 0f;
            //Debug.Log(" in if WallSight OnTriggerEnter2D " + Player.Instance.horizontal);

        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            //if (!Player.Instance.Falling)
            {
                if (Player.Instance.horizontal == 0)
                {
                    //Player.Instance.transform.position += new Vector3(Player.Instance.horizontal_temp==1 ? 2:-2, 0.5f,0)*Time.deltaTime*50;
                    Player.Instance.horizontal = Player.Instance.horizontal_temp;
                    //Debug.Log(" OnTriggerExit2D " + Player.Instance.transform.position);
                    Player.Instance.horizontal_temp = 0f;
                }
            }
        }
    }
}
