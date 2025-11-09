using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        int button = -1;

        if (Input.GetMouseButtonDown(0)) button = 0;
        else if (Input.GetMouseButtonDown(1)) button = 1;

        if (button != -1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                switch (button)
                {
                    case 0:
                        hit.collider.GetComponent<IClickable>()?.OnClickedLeft();
                        break;
                    case 1:
                        hit.collider.GetComponent<IClickable>()?.OnClickedRight();
                        break;
                    default:
                        break;
                }

            }

        }

    }

    
}
