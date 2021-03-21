using UnityEngine;

public class MoveCloud : MonoBehaviour {
    [SerializeField] float MoveSpeed;
    [SerializeField] float LifeTime;
    float timecount = 0;
    private void Update() {
        timecount += Time.deltaTime;
        //if(timecount>=LifeTime) Destroy(gameObject);
        transform.position += transform.right*-MoveSpeed*Time.deltaTime;
    }
}